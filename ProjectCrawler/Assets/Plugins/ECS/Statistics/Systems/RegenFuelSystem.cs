namespace ECS.Statistics
{
    using System.Collections;
    using System.Diagnostics;
    using ECS;
    using UnityEngine;
    using ECS.Utilities;

    public class RegenFuelSystem : ComponentSystem
    {
        public RegenState regenState;

        public bool canUpdate = true;
        public bool isInUpdateLoop = false;

        private Stopwatch stopwatch;
        private double timeAtLastUpdate = 0;
        private float _idealUpdateDeltaTime = 0.1f;

        public RegenFuelSystem() : base()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();

            CoroutineController.Instance.StartCoroutine(this.UpdateLoop());
        }

        public enum RegenState
        {
            IDLE,
            ACTIVE,
            HEAVY,
            OUTOFFUEL,
            HALT,
            EXHAUSTED,
            UNCONSCIOUS,
            DEAD
        }

        public float idealUpdateDeltaTime
        {
            get
            {
                return this._idealUpdateDeltaTime;
            }

            set
            {
                this._idealUpdateDeltaTime = Mathf.Clamp01(value);

                if (this._idealUpdateDeltaTime != value)
                {
                    UnityEngine.Debug.LogError("StatisticSystem--idealUpdateDeltaTime must have a value between 0 and 1");
                }
            }
        }

        #region Update Function
        public void Update(float deltaTime)
        {
            this.BroadcastMessage("EarlyUpdate");

            RegenState internalRegenState = this.regenState;

            foreach (FuelControllerComp fuelComponent in this.GetComponents<FuelControllerComp>())
            {
                Statistic fuelComponentStatistic = fuelComponent.entity.GetComponent<Statistic>();

                if (internalRegenState != RegenState.OUTOFFUEL && fuelComponentStatistic.state == Statistic.StatisticState.LOW)
                {
                    internalRegenState = RegenState.HALT;
                }
                else if (fuelComponentStatistic.state == Statistic.StatisticState.NONE)
                {
                    internalRegenState = RegenState.OUTOFFUEL;
                }
            }

            this.BroadcastMessage("Update");

            float regenDone = 0.0f;

            foreach (RegenControllerComp regenComponent in this.GetComponents<RegenControllerComp>())
            {
                Statistic regenComponentStatistic = regenComponent.entity.GetComponent<Statistic>();

                float regen = 0.0f;

                switch (internalRegenState)
                {
                    // FIXME: Should we use GetComponent on every statistic, or cache the results somehow
                    // The results of the query are cached internally to the Entity class, but is something more needed?
                    case RegenState.IDLE:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.idleRegenPS * deltaTime;
                        break;
                    case RegenState.ACTIVE:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.activeRegenPS * deltaTime;
                        break;
                    case RegenState.HEAVY:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.heavyRegenPS * deltaTime;
                        break;
                    case RegenState.OUTOFFUEL:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.outOfFuelRegenPS * deltaTime;
                        break;
                    case RegenState.HALT:
                        regen = 0.0f;
                        break;
                    case RegenState.EXHAUSTED:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.exhaustedRegenPS * deltaTime;
                        break;
                    case RegenState.UNCONSCIOUS:
                        regen = regenComponent.entity.GetComponent<StatInfoComp>().statInfo.unconsciousRegenPS * deltaTime;
                        break;
                }

                if (regenComponentStatistic.current <= regenComponentStatistic.max - regen
                    && regenComponentStatistic.current >= 0.0f)
                {
                    regenComponentStatistic.current += regen;
                }
                else if (regenComponentStatistic.current + regen < 0.0f)
                {
                    regen -= regenComponentStatistic.current;
                    regenComponentStatistic.current = 0.0f;
                }
                else if (regenComponentStatistic.current < regenComponentStatistic.max)
                {
                    regen = regenComponentStatistic.max - regenComponentStatistic.current;
                    regenComponentStatistic.current = regenComponentStatistic.max;
                }
                else
                {
                    regen = 0;
                }

                if (regen > 0.0f)
                {
                    regenDone += regen;
                }
            }

            this.BroadcastMessage("LateUpdate");

            float fuelUsed = 1 + regenDone;

            foreach (FuelControllerComp fuelComponent in this.GetComponents<FuelControllerComp>())
            {
                Statistic fuelComponentStatistic = fuelComponent.entity.GetComponent<Statistic>();

                float statChange = 0;

                switch (this.regenState)
                {
                    case RegenState.IDLE:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.idleRegenPS * deltaTime * fuelUsed;
                        break;
                    case RegenState.ACTIVE:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.activeRegenPS * deltaTime * fuelUsed;
                        break;
                    case RegenState.HEAVY:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.heavyRegenPS * deltaTime * fuelUsed;
                        break;
                    case RegenState.OUTOFFUEL:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.outOfFuelRegenPS * deltaTime * fuelUsed;
                        break;
                    case RegenState.HALT:
                        statChange = 0.0f;
                        break;
                    case RegenState.EXHAUSTED:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.exhaustedRegenPS * deltaTime * fuelUsed;
                        break;
                    case RegenState.UNCONSCIOUS:
                        statChange = fuelComponent.entity.GetComponent<StatInfoComp>().statInfo.unconsciousRegenPS * deltaTime * fuelUsed;
                        break;
                }

                fuelComponentStatistic.current = Mathf.Clamp(
                    fuelComponentStatistic.current + statChange, 
                    0, 
                    Mathf.Infinity);
            }
        }
        #endregion

        private IEnumerator UpdateLoop()
        {
            if (this.isInUpdateLoop)
            {
                UnityEngine.Debug.LogError("Cannot have 2 simultaneous update loops running on a single RegenFuelSystem");
                yield break;
            }

            while (true)
            {
                yield return new WaitForSeconds(
                    this.idealUpdateDeltaTime
                    - (float)((this.stopwatch.ElapsedMilliseconds / 1000.0f)
                    - this.timeAtLastUpdate));

                double curr_time = (double)this.stopwatch.ElapsedMilliseconds / 1000.0f;

                float deltaTime = (float)(curr_time - this.timeAtLastUpdate);
                this.timeAtLastUpdate = curr_time;

                if (this.canUpdate)
                {
                    this.Update(deltaTime);
                }
            }
        }
    }
}