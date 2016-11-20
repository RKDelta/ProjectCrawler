namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ECS.GameObjectECS;
    using ECS.Utilities;
    using UnityEngine;

    [Serializable]
    public class EntityPrototype
    {
        public string _name;
        public string visualName;
        public string description;

        [SerializeField]
        public List<Component> components;

        private Dictionary<Type, Component[]> queryCache;

        public EntityPrototype()
        {
            this.components = new List<Component>();

            this.ResetCaches();
        }

        public EntityPrototype(string name, JSONObject jsonObj)
        {
            this.name = name;
            this.components = new List<Component>();

            if (jsonObj.type != JSONObject.Type.OBJECT)
            {
                Debug.LogError("EntityTemplate was not passed a valid JSONObject to convert to a Prototype");
            }

            for(int i = 0; i < jsonObj.keys.Count; i++)
            {
                switch (jsonObj.keys[i].ToSlug())
                {
                    case "name":
                        this.name = jsonObj.list[i].str;
                        break;
                    case "visual_name":
                        this.visualName = jsonObj.list[i].str;
                        break;
                    case "description":
                        this.description = jsonObj.list[i].str;
                        break;
                    default:
                        bool foundValue = false;

                        foreach (Type componentType in typeof(ECS.Component).Assembly.GetTypes())
                        {
                            if (componentType.Name == jsonObj.keys[i]
                                && componentType.IsSubclassOf(typeof(Component))
                                && componentType.IsAbstract == false)
                            {
                                foundValue = true;

                                ConstructorInfo constructor = componentType.GetConstructor(new Type[] { typeof(JSONObject) });

                                if (constructor != null)
                                {
                                    this.AddComponent((Component)constructor.Invoke(new object[] { jsonObj.list[i] }));
                                    break;
                                }

                                constructor = componentType.GetConstructor(Type.EmptyTypes);

                                if (constructor != null)
                                {
                                    Component component = (Component)constructor.Invoke(new object[] { });

                                    if (jsonObj.list[i].type == JSONObject.Type.OBJECT)
                                    {
                                        for (int x = 0; x < jsonObj.list[i].keys.Count; x++)
                                        {
                                            FieldInfo field = componentType.GetField(jsonObj.list[i].keys[x]);

                                            if (field == null)
                                            {
                                                continue;
                                            }

                                            if (field.FieldType == typeof(string)
                                                && jsonObj.list[i].list[x].type == JSONObject.Type.STRING)
                                            {
                                                field.SetValue(component, jsonObj.list[i].list[x].str);
                                            }
                                            else if (field.FieldType == typeof(int)
                                                && jsonObj.list[i].list[x].type == JSONObject.Type.NUMBER)
                                            {
                                                field.SetValue(component, (int)jsonObj.list[i].list[x].n);
                                            }
                                            else if (field.FieldType == typeof(float)
                                                && jsonObj.list[i].list[x].type == JSONObject.Type.NUMBER)
                                            {
                                                field.SetValue(component, jsonObj.list[i].list[x].n);
                                            }
                                            else if (field.FieldType == typeof(bool)
                                                && jsonObj.list[i].list[x].type == JSONObject.Type.BOOL)
                                            {
                                                field.SetValue(component, jsonObj.list[i].list[x].b);
                                            }
                                        }
                                    }

                                    this.AddComponent(component);

                                    break;
                                }
                                else
                                {
                                    Debug.Log(componentType.ToString() 
                                        + "does not have a valid constructor to be constructed programmatically from JSON");
                                }
                            }
                        }

                        if (foundValue == false)
                        {
                            Debug.Log(string.Format(
                                "The key \"{0}\" in the JSONObject was invalid",
                                jsonObj.keys[i]));
                        }
                        
                        break;
                }
            }
        }

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

        public void AddComponent(Component instance)
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

                if (allowSearch && typeof(EntityPrototype).GetMethod("GetComponent").MakeGenericMethod(instanceType).Invoke(this, null) != null)
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
                        if (typeof(EntityPrototype).GetMethod("GetComponent").MakeGenericMethod(type).Invoke(this, null) == null)
                        {
                            this.AddComponent((Component)type.GetConstructor(new Type[] { }).Invoke(null, null));
                        }
                    }
                }
            }

            this.components.Add(instance);
        }

        #region RemoveComponent definition
        public void RemoveComponent(Component instance)
        {
            foreach (Component component in this.components)
            {
                if (component == instance)
                {
                    this.components.Remove(component);
                    return;
                }
            }
        }
        #endregion

        public void ResetCaches()
        {
            this.queryCache = new Dictionary<Type, Component[]>();
        }

        public Entity Instantiate()
        {
            Entity entity = new Entity(this.name, this.visualName, this.description);

            foreach (Component component in this.components)
            {
                entity.AddComponent(component.Clone());
            }

            return entity;
        }
        
        public Entity Instantiate(GameObjectECSBridge bridge)
        {
            Entity entity = new Entity(this.name, this.visualName, this.description);

            foreach (Component component in this.components)
            {
                Component componentClone = component.Clone();

                if (componentClone is GameObjectComp)
                {
                    ((GameObjectComp)componentClone).SetBridge(bridge);
                }

                entity.AddComponent(componentClone);
            }

            return entity;
        }
    }
}