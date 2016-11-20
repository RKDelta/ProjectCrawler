namespace ECS
{
    using UnityEngine;
    using System;
    
    [AllowMultiple]
    public abstract class Component
    {
        public Component() { }
        
        public Entity entity { get; protected set; }

        public string name
        {
            get
            {
                if (this.entity == null)
                {
                    return null;
                }

                return this.entity.name;
            }
        }

        public virtual void SetEntity(Entity entity)
        {
            this.entity = entity;
        }

        public virtual Component Clone()
        {
            return (Component)this.MemberwiseClone();
        }

        public virtual T GetComponent<T>() where T : Component
        {
            if (this.entity == null)
            {
                return null;
            }

            return this.entity.GetComponent<T>();
        }

        public virtual T[] GetComponents<T>() where T : Component
        {
            if (this.entity == null)
            {
                return null;
            }

            return this.entity.GetComponents<T>();
        }
    }
}