namespace ECS
{
    using UnityEngine;
    using System.Collections.Generic;

    public class EntitySystemPrototype
    {
        public List<EntityPrototype> entityPrototypes;

        public void AddEntity(EntityPrototype instance)
        {
            this.entityPrototypes.Add(instance);
        }

        public EntitySystem Instantiate()
        {
            EntitySystem system = new EntitySystem();

            foreach (EntityPrototype prototype in this.entityPrototypes)
            {
                system.AddEntity(prototype.Instantiate());
            }

            return system;
        }

        public void RemoveEntity(EntityPrototype instance)
        {
            if (entityPrototypes.Contains(instance))
            {
                this.entityPrototypes.Remove(instance);
                return;
            }
        }
    }
}