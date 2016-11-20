namespace ECS.Statistics
{
    using ECS;
    using UnityEngine;

    public class OverallStatisticSystem : EntitySystem
    {
        public OverallStatisticSystem(params Entity[] entities) : base(entities) { }

        public Statistic GetStatistic(string name)
        {
            return this.GetEntity(name).GetComponent<Statistic>();
        }
    }
}