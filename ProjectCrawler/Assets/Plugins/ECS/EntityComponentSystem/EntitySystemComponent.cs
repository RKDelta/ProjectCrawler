namespace ECS
{
    using System;

    public abstract class EntitySystemComp : ECS.Component
    {
        public EntitySystemComp() { }

        public EntitySystemComp(EntitySystem system)
        {
            this.system = system;
        }

        public EntitySystem system { get; protected set; }
    }
}
