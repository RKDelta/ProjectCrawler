namespace ECS.Items
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ECS;
    using ECS.GameObjectECS;
    using ECS.Statistics;
    using UnityEngine;
    using ECS.Utilities;

    public struct StatToEffect
    {
        public string statName;

        public float effectAmount;
    } 

    [ECS.RequireComponent(typeof(Item))]
    [ECS.RequireComponent(typeof(UsableItem))]
    public class StatChangeComp : ECS.Component
    {
        private float _timeToTakeEffect = 1.0f;
        private List<StatToEffect> statsToEffect;

        public StatChangeComp(
            float timeToTakeEffect,
            params StatToEffect[] statsToEffect)
        {
            this.statsToEffect = statsToEffect.ToList();
            this.timeToTakeEffect = timeToTakeEffect;
        }

        public float timeToTakeEffect
        {
            get
            {
                return this._timeToTakeEffect;
            }

            protected set
            {
                this._timeToTakeEffect = value;
            }
        }

        public void Use(Entity user, Entity target)
        {
            CoroutineController.Instance.StartCoroutine(this.UseCoroutine(user));
        }

        public override ECS.Component Clone()
        {
            return new StatChangeComp(this.timeToTakeEffect, this.statsToEffect.ToArray());
        }

        public void AddStatToEffect(StatToEffect instance)
        {
            this.statsToEffect.Add(instance);
        }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            entity.GetComponent<UsableItem>().OnUse += this.Use;
        }

        protected virtual IEnumerator UseCoroutine(Entity user)
        {
            float timeRemaining = this.timeToTakeEffect;

            while (timeRemaining > 0)
            {
                yield return new WaitForEndOfFrame();

                foreach (StatToEffect statToEffect in this.statsToEffect)
                {
                    Entity statEntity = user.GetComponent<StatisticSystemComp>().system.GetEntity(statToEffect.statName);

                    if (statEntity == null)
                    {
                        Debug.LogError("The player doesn't have a statistic called " + statToEffect.statName);
                        continue;
                    }

                    Statistic stat = statEntity.GetComponent<Statistic>();

                    float effectAmountThisFrame = Time.deltaTime * (statToEffect.effectAmount / this.timeToTakeEffect);

                    if (stat.current < 0)
                    {
                        continue;
                    }

                    if (stat.current + effectAmountThisFrame < 0)
                    {
                        stat.current = 0;
                        continue;
                    }

                    if (stat.current > stat.max)
                    {
                        continue;
                    }

                    if (stat.current + effectAmountThisFrame > stat.max)
                    {
                        stat.current = stat.max;
                        continue;
                    }

                    stat.current += effectAmountThisFrame;
                }

                timeRemaining -= Time.deltaTime;
            }
        }
    }
}