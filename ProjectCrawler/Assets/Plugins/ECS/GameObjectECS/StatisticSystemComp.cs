namespace ECS.GameObjectECS
{
    using System;
    using ECS.Statistics;
    using UnityEngine;

    public class StatisticSystemComp : EntitySystemComp
    {
        public StatisticSystemComp()
        {
            this.system = new OverallStatisticSystem();
        }

        public StatisticSystemComp(OverallStatisticSystem statisticSystem)
        {
            this.system = system;
        }

        public StatisticSystemComp(JSONObject jsonObj)
        {
            this.system = new OverallStatisticSystem();

            for (int i = 0; i < jsonObj.keys.Count; i++)
            {
                if (jsonObj.type != JSONObject.Type.OBJECT)
                {
                    Debug.LogError("StatisticSystemComp was not passed a valid JSONObject to convert to a Prototype");
                    return;
                }

                Entity entity = new EntityPrototype(jsonObj.keys[i], jsonObj.list[i]).Instantiate();

                if (entity.GetComponent<Statistic>() == null)
                {
                    Debug.Log(
                        "StatisticSystemComp was passed a JSONObject for the Statistics"
                        + "which did not give the statistics Statistic components. "
                        + "Adding a Statistic automattically.");
                    entity.AddComponent(new Statistic());
                }

                this.system.AddEntity(entity);
            }
        }
    }
}
