namespace ECS.EntityMonoBehaviours
{
    using ECS;
    using ECS.Statistics;
    using UnityEngine;

    public class EntityMB : MonoBehaviour
    {
        public EntityState state = EntityState.Alive;
        private OverallStatisticSystem _statisticSystem;
        private float _weight;

        public enum EntityState
        {
            Alive,
            Dead
        }

        public virtual OverallStatisticSystem statisticSystem
        {
            get { return this._statisticSystem; }
            protected set { this._statisticSystem = value; }
        }

        public virtual float weight
        {
            get
            {
                return this._weight;
            }

            protected set
            {
                this._weight = value;
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            this.statisticSystem = new OverallStatisticSystem(
                new Entity("health", new Statistic(0, 100)));
        }

        protected virtual void Update()
        {
            if (this.statisticSystem.GetStatistic("health").state == Statistic.StatisticState.NONE)
            {
                this.state = EntityState.Dead;
            }
        }
    }
}