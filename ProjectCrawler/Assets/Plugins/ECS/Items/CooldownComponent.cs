namespace ECS.Items
{
    using System.Collections;
    using ECS;
    using ECS.Utilities;
    using UnityEngine;

    [ECS.RequireComponent(typeof(Item))]
    [ECS.RequireComponent(typeof(UsableItem))]
    public class CooldownComp : ECS.Component
    {
        public float cooldownTime;

        public CooldownComp(float cooldownTime)
        {
            this.cooldownTime = cooldownTime;
            this.cooldownRemaining = 0.0f;
        }

        public float cooldownRemaining { get; protected set; }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            this.GetComponent<UsableItem>().OnUse += this.Use;
        }

        public void Use(Entity user, Entity target)
        {
            CoroutineController.Instance.StartCoroutine(this.WaitForCooldown());
        }

        public IEnumerator WaitForCooldown()
        {
            this.cooldownRemaining = this.cooldownTime;

            UsableItem usableItem = this.GetComponent<UsableItem>();

            usableItem.CatchUse();

            while (this.cooldownRemaining > 0.0f)
            {
                yield return new WaitForEndOfFrame();

                this.cooldownRemaining -= Time.deltaTime;
            }

            usableItem.ReleaseUse();

            this.cooldownRemaining = 0.0f;
        }

        public override ECS.Component Clone()
        {
            return new CooldownComp(this.cooldownTime);
        }
    }
}