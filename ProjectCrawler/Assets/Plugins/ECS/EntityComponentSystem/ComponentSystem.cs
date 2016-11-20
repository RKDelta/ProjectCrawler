namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public abstract class ComponentSystem
    {
        private List<Component> components;

        private Dictionary<Type, Component[]> componentQueryCache;

        public ComponentSystem()
        {
            this.components = new List<Component>();
        }

        public ComponentSystem(params Component[] instances)
        {
            this.components = new List<Component>();

            this.AddComponents(instances);

            this.ResetCaches();
        }

        public virtual void AddComponent(Component instance)
        {
            foreach (object attribute in instance.GetType().GetCustomAttributes(true))
            {
                if (attribute is PartOfComponentSystemAttribute)
                {
                    if (((PartOfComponentSystemAttribute)attribute).parentSystemType == this.GetType())
                    {
                        this.components.Add(instance);
                        this.ResetCaches();

                        return;
                    }
                }
            }

            Debug.LogError(string.Format(
                "The Component is of the type {0}, which is not part of this ComponentSystem, {1}",
                instance.GetType().ToString(),
                this.GetType()));
        }

        public void AddComponents(params Component[] instances)
        {
            foreach (Component instance in instances)
            {
                this.AddComponent(instance);
            }
        }

        public virtual T GetComponent<T>() where T : Component
        {
            T[] components = this.GetComponents<T>();

            if (components == null || components.Length <= 0)
            {
                return null;
            }

            return components[0];

            /*if (this.componentQueryCache.ContainsKey(typeof(T)))
            {
                return (T)this.componentQueryCache[typeof(T)][0];
            }

            foreach (Component component in this.components)
            {
                if (component is T)
                {
                    return (T)component;
                }
            }

            return null;*/
        }

        public T[] GetComponents<T>() where T : Component
        {
            if (this.componentQueryCache.ContainsKey(typeof(T)))
            {
                return (T[])this.componentQueryCache[typeof(T)];
            }

            List<T> resultsList = new List<T>();

            foreach (Component component in this.components)
            {
                if (component is T)
                {
                    resultsList.Add((T)component);
                }
            }

            if (resultsList.Count <= 0)
            {
                return null;
            }

            T[] results = resultsList.ToArray();

            this.componentQueryCache.Add(typeof(T), results);

            return results;
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

        private void ResetCaches()
        {
            this.componentQueryCache = new Dictionary<Type, Component[]>();
        }
    }
}
