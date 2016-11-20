namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using ECS.Utilities;

    public class EntitySystem
    {
        private List<Entity> _entities;

        // Caches for searches (reset whenever the Statistics change)
        private Dictionary<string, Entity> nameQueryCache;
        private Dictionary<Type, Component[]> typeQueryCache;
        private Dictionary<Type[], Entity[]> entitiesQueryCache;

        private List<ComponentSystem> systems;

        private Dictionary<Type, ComponentSystem[]> systemTypeQueryCache;

        public EntitySystem(params Entity[] entities)
        {
            this.entities = new List<Entity>();
            this.ResetCaches();

            this.systems = new List<ComponentSystem>();
            this.ResetSystemCaches();

            this.AddEntities(entities);
        }

        public List<Entity> entities
        {
            get
            {
                return this._entities;
            }

            set
            {
                if (this._entities != null)
                {
                    foreach (Entity statistic in this._entities)
                    {
                        statistic.OnComponentsChanged -= this.ResetCaches;
                    }
                }

                this._entities = value;

                if (value != null)
                {
                    foreach (Entity statistic in this._entities)
                    {
                        statistic.OnComponentsChanged += this.ResetCaches;
                    }
                }
            }
        }

        public void BroadcastMessage(string methodName, params object[] parameters)
        {
            foreach (Entity statistic in this.entities)
            {
                statistic.BroadcastMessage(methodName, parameters);
            }
        }

        #region AddEntity and AddEntities definitions
        public virtual void AddEntity(Entity instance)
        {
            if (instance == null)
            {
                Debug.LogError("OverallEntitySystem--AddEntity should not be passed a null value");
                return;
            }

            if (this.GetEntity(instance.name) != null)
            {
                UnityEngine.Debug.LogError("StatComponentSystem -- You cannot add 2 statistics of the same name -- A statistic of name "
                                + instance.name + " already exists in this System");
                return;
            }

            instance.OnComponentsChanged += this.ResetCaches;
            this.entities.Add(instance);
            this.ResetCaches();

            foreach (Component component in instance.GetComponents<Component>())
            {
                foreach (object attribute in component.GetType().GetCustomAttributes(true))
                {
                    if (attribute is PartOfComponentSystemAttribute)
                    {
                        PartOfComponentSystemAttribute pocsa = (PartOfComponentSystemAttribute)attribute;

                        bool isGlobalSystem = false;

                        foreach (object parentSystemAttribute in pocsa.parentSystemType.GetCustomAttributes(true))
                        {
                            if (parentSystemAttribute is GlobalComponentSystemAttribute)
                            {
                                isGlobalSystem = true;
                            }
                        }

                        if (isGlobalSystem)
                        {
                            continue;
                        }

                        ComponentSystem system = (ComponentSystem)typeof(EntitySystem)
                            .GetMethod("GetSystem")
                            .MakeGenericMethod(pocsa.parentSystemType)
                            .Invoke(this, null);

                        if (system == null)
                        {
                            system = (ComponentSystem)pocsa.parentSystemType.GetConstructor(new Type[] { }).Invoke(null, null);

                            this.AddSystem(system);
                        }

                        system.AddComponent(component);
                    }
                }
            }

            instance.SetOverallSystem(this);
        }

        public void AddEntities(params Entity[] instances)
        {
            foreach (Entity instance in instances)
            {
                this.AddEntity(instance);
            }
        }
        #endregion

        #region GetEntity (by name) definition
        public Entity GetEntity(string name)
        {
            name = name.ToSlug();

            if (this.nameQueryCache != null && this.nameQueryCache.ContainsKey(name))
            {
                return this.nameQueryCache[name];
            }

            if (this.entities != null)
            {
                foreach (Entity statistic in this.entities)
                {
                    if (statistic.name.ToLower() == name)
                    {
                        this.nameQueryCache.Add(statistic.name.ToLower(), statistic);
                        return statistic;
                    }
                }
            }

            return null;
        }
        #endregion

        #region GetComponent and GetComponents definitions
        public T GetComponent<T>() where T : Component
        {
            T[] components = this.GetComponents<T>();

            if (components == null || components.Count() <= 0)
            {
                return null;
            }

            return components[0];

            /*if (this.typeQueryCache != null && this.typeQueryCache.Keys.Contains(typeof(T)))
            {
                return (T)this.typeQueryCache[typeof(T)][0];
            }

            if (this.entities != null)
            {
                foreach (Entity statistic in this.entities)
                {
                    Component component = statistic.GetComponent<T>();
                    if (component != null)
                    {
                        return (T)component;
                    }
                }
            }

            return null;*/
        }

        public T[] GetComponents<T>() where T : Component
        {
            if (this.typeQueryCache != null && this.typeQueryCache.Keys.Contains(typeof(T)))
            {
                return (T[])this.typeQueryCache[typeof(T)];
            }

            List<T> outputList = new List<T>();

            if (this.entities != null)
            {
                foreach (Entity statistic in this.entities)
                {
                    T[] components = statistic.GetComponents<T>();
                    if (components != null)
                    {
                        outputList.AddRange(components);
                    }
                }
            }

            T[] outputArray = outputList.ToArray();
            if (this.typeQueryCache != null)
            {
                this.typeQueryCache[typeof(T)] = outputArray;
            }

            return outputArray;
        }
        #endregion

        #region GetEntities WithComponents definitions
        public Entity[] GetEntitiesWithComponents<T1>() 
            where T1 : Component
        {
            return this.GetEntitiesWithComponents(typeof(T1));
        }

        public Entity[] GetEntitiesWithComponents<T1, T2>() 
            where T1 : Component                             
            where T2 : Component
        {
            return this.GetEntitiesWithComponents(typeof(T1), typeof(T2));
        }

        public Entity[] GetEntitiesWithComponents<T1, T2, T3>() 
            where T1 : Component 
            where T2 : Component
            where T3 : Component
        {
            return this.GetEntitiesWithComponents(typeof(T1), typeof(T2), typeof(T3));
        }

        public Entity[] GetEntitiesWithComponents<T1, T2, T3, T4>() 
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
        {
            return this.GetEntitiesWithComponents(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }
        #endregion

        #region AddSystem and AddSystems definition
        public void AddSystem(ComponentSystem instance)
        {
            bool allowInstanceTypeSearch = true;
            Type instanceType = instance.GetType();
            while (allowInstanceTypeSearch)
            {
                if (instanceType == typeof(ComponentSystem))
                {
                    break;
                }

                foreach (object attribute in instanceType.GetCustomAttributes(false))
                {
                    if (attribute is AllowMultipleAttribute)
                    {
                        allowInstanceTypeSearch = false;
                        break;
                    }
                }

                if (typeof(EntitySystem)
                    .GetMethod("GetSystem")
                    .MakeGenericMethod(instanceType)
                    .Invoke(this, null) != null)
                {
                    UnityEngine.Debug.LogError(string.Format(
                        "OverallEntitySystem--AddSystem--Cannot have two instances of a ComponentSystem--{1}",
                        instance.GetType().ToString()));
                    return;
                }

                instanceType = instanceType.BaseType;
            }

            this.systems.Add(instance);
            this.ResetSystemCaches();
        }

        public void AddSystems(params ComponentSystem[] instances)
        {
            foreach (ComponentSystem instance in instances)
            {
                this.AddSystem(instance);
            }
        }
        #endregion

        #region GetSystem and GetSystems definitions
        public T GetSystem<T>() where T : ComponentSystem
        {
            T[] systems = this.GetSystems<T>();

            if (systems == null || systems.Count() <= 0)
            {
                return null;
            }

            return systems[0];

            /*if (this.systemTypeQueryCache.ContainsKey(typeof(T)))
            {
                return (T)this.systemTypeQueryCache[typeof(T)][0];
            }

            foreach (ComponentSystem system in this.systems)
            {
                if (system is T)
                {
                    return (T)system;
                }
            }

            return null;*/
        }

        public T[] GetSystems<T>() where T : ComponentSystem
        {
            if (this.systemTypeQueryCache.ContainsKey(typeof(T)))
            {
                return (T[])this.systemTypeQueryCache[typeof(T)];
            }

            List<T> results = new List<T>();

            foreach (ComponentSystem system in this.systems)
            {
                if (system is T)
                {
                    results.Add((T)system);
                }
            }

            this.systemTypeQueryCache.Add(typeof(T), results.ToArray());

            return results.ToArray();
        }
        #endregion

        protected virtual void ResetCaches()
        {
            this.nameQueryCache = new Dictionary<string, Entity>();
            this.typeQueryCache = new Dictionary<Type, Component[]>();
            this.entitiesQueryCache = new Dictionary<Type[], Entity[]>();
        }

        /// <summary>
        /// This function is to be used ONLY where it is not possible to use the generic alternatives to the StatisticSystem class. 
        /// This function should NEVER be directly exposed to other classes.
        /// </summary>
        /// <param name="types">The StatComponent types to search for.</param>
        /// <returns>All of the statistics with all of the inputted StatComponents</returns>
        private Entity[] GetEntitiesWithComponents(params Type[] types)
        {
            // Something here isn't working. The game crashes with a KeyNotFoundException
            if (this.entitiesQueryCache != null || this.entitiesQueryCache.Keys.Contains(types))
            {
                return this.entitiesQueryCache[types];
            }

            List<Entity> outputList = new List<Entity>();

            MethodInfo method = typeof(Entity).GetMethod("GetComponent");

            MethodInfo[] generics = new MethodInfo[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                generics[i] = method.MakeGenericMethod(types[i]);
            }

            foreach (Entity entities in this.entities)
            {
                foreach (MethodInfo generic in generics)
                {
                    if (generic.Invoke(entities, null) == null)
                    {
                        break;
                    }
                }

                outputList.Add(entities);
            }

            Entity[] outputArray = outputList.ToArray();

            if (this.entitiesQueryCache != null)
            {
                this.entitiesQueryCache.Add(types, outputArray);
            }

            return outputArray;
        }

        private void ResetSystemCaches()
        {
            this.systemTypeQueryCache = new Dictionary<Type, ComponentSystem[]>();
        }
    }
}