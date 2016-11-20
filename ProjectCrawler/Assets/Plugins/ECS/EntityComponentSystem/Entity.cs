namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using ECS.Utilities;
    
    [Serializable]
    public class Entity : ICloneable
    {
        private List<Component> _components;
        private Dictionary<Type, Component[]> queryCache = new Dictionary<Type, Component[]>();

        #region Constructors
        public Entity(string name, params Component[] components)
        {
            this.name = name;
            this.components = new List<Component>();
            this.AddComponents(components);
        }

        public Entity(string name, string visualName, params Component[] components) 
            : this(name, components)
        {
            this.visualName = visualName;
        }

        public Entity(string name, string visualName, string description, params Component[] components)
            : this(name, visualName, components)
        {
            this.description = description;
        }

        public Entity(EntityPrototype template)
        {
            this.name = template.name;
            this.visualName = template.visualName;
            this.description = template.description;

            this.components = new List<Component>();

            this.AddComponents(template.components.ToArray());
        }
        #endregion
        
        public event Action OnComponentsChanged;
        
        public EntitySystem overallEntitySystem { get; protected set; }

        public List<Component> components
        {
            get
            {
                return this._components;
            }

            set
            {
                this._components = value;
                if (this.OnComponentsChanged != null)
                {
                    this.OnComponentsChanged();
                }

                foreach (Component component in this._components)
                {
                    component.SetEntity(this);
                }
            }
        }

        #region name, visualName, and description
        private string _name;

        public string name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value.ToSlug();
            }
        }
        
        private string _visualName = null;

        public string visualName
        {
            get
            {
                return this._visualName ?? this.name ?? "NoName";
            }
            set
            {
                this._visualName = value;
            }
        }
        
        private string _description = null;

        public string description
        {
            get
            {
                return this._description ?? this.visualName ?? "NoDescription";
            }
            set
            {
                this._description = value;
            }
        }
        #endregion

        public void SetOverallSystem(EntitySystem overallEntitySystem)
        {
            if (this.overallEntitySystem != overallEntitySystem)
            {
                this.overallEntitySystem = overallEntitySystem;

                this.OnComponentsChanged = null;
            }
        }

        public void BroadcastMessage(string methodName, params object[] parameters)
        {
            foreach (Component component in this.components)
            {
                MethodInfo method = component.GetType().GetMethod(methodName);

                if (method != null)
                {
                    method.Invoke(component, parameters);
                }
            }
        }

        #region AddComponents and AddComponents definitions
        public virtual void AddComponent(Component instance)
        {
            bool allowSearch = true;
            Type instanceType = instance.GetType();
            while (allowSearch)
            {
                if (instanceType == typeof(Component))
                {
                    break;
                }

                foreach (object attribute in instanceType.GetCustomAttributes(false))
                {
                    if (attribute is AllowMultipleAttribute)
                    {
                        allowSearch = false;
                        break;
                    }
                }

                if (allowSearch && typeof(Entity).GetMethod("GetComponent").MakeGenericMethod(instanceType).Invoke(this, null) != null)
                {
                    Debug.LogError(string.Format(
                        "Entity--{0}--AddComponent--Cannot have two instances of a Component--{1}--on a single Entity",
                        this.name,
                        instanceType.ToString()));
                    return;
                }

                instanceType = instanceType.BaseType;
            }

            // TODO: Merge these two searches of the attributes of the Instance's Type
            foreach (object attribute in instance.GetType().GetCustomAttributes(true))
            {
                if (attribute is RequireComponentAttribute)
                {
                    foreach (Type type in ((RequireComponentAttribute)attribute).required)
                    {
                        if (typeof(Entity).GetMethod("GetComponent").MakeGenericMethod(type).Invoke(this, null) == null)
                        {
                            this.AddComponent((Component)type.GetConstructor(new Type[] { }).Invoke(null, null));
                        }
                    }
                }
            }

            this.components.Add(instance);

            instance.SetEntity(this);

            foreach (object attribute in instance.GetType().GetCustomAttributes(true))
            {
                if (attribute is PartOfComponentSystemAttribute)
                {
                    PartOfComponentSystemAttribute pocsa = (PartOfComponentSystemAttribute)attribute;

                    foreach (
                        object parentSystemAttribute
                        in pocsa.parentSystemType.GetCustomAttributes(true))
                    {
                        if (parentSystemAttribute is GlobalComponentSystemAttribute)
                        {
                            ((ComponentSystem)pocsa
                                .parentSystemType
                                .GetProperty("Instance")
                                .GetValue(null, null))
                                .AddComponent(instance);
                        }
                    }
                }
            }

            if (this.OnComponentsChanged != null)
            {
                this.OnComponentsChanged();
            }

            this.ResetCaches();
        }

        public void AddComponents(params Component[] instances)
        {
            foreach (Component instance in instances)
            {
                this.AddComponent(instance);
            }
        }
        #endregion

        #region GetComponent and GetComponents definitions
        public T GetComponent<T>() where T : Component
        {
            if (this.queryCache == null)
            {
                this.ResetCaches();
            }

            if (this.queryCache.Keys.Contains(typeof(T)))
            {
                return (T)this.queryCache[typeof(T)][0];
            }

            T[] components = this.GetComponents<T>();

            if (components == null || components.Length <= 0)
            {
                return null;
            }

            return components[0];
        }

        public T[] GetComponents<T>() where T : Component
        {
            if (this.queryCache == null)
            {
                this.ResetCaches();
            }

            if (this.queryCache.Keys.Contains(typeof(T)))
            {
                return (T[])this.queryCache[typeof(T)];
            }

            List<T> output = new List<T>();

            foreach (Component component in this.components)
            {
                if (component as T != null)
                {
                    output.Add((T)component);
                }
            }

            return output.ToArray();
        }
        #endregion

        public void ResetCaches()
        {
            this.queryCache = new Dictionary<Type, Component[]>();
        }

        public object Clone()
        {
            Entity clone = (Entity)this.MemberwiseClone();

            clone.components = new List<Component>();

            for (int i = 0; i < this.components.Count; i++)
            {
                clone.AddComponent(this.components[i].Clone());
            }

            clone.ResetCaches();

            return clone;
        }
    }
}