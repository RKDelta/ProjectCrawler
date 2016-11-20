namespace ECS.Controllers
{
    using System.Collections.Generic;
    using ECS.Items;
    using UnityEngine;
    using ECS.Utilities;

    public static class ItemController
    {
        public static List<EntityPrototype> itemPrototypes { get; private set; }

        private static Dictionary<string, EntityPrototype> queryCache;

        public static void AddPrototype(EntityPrototype template)
        {
            if (template.GetComponent<Item>() == null)
            {
                Debug.LogError("Any template added to the ItemController should have a Item Component attached.");
            }

            itemPrototypes.Add(template);
        }

        public static EntityPrototype GetPrototype(string name)
        {
            name = name.ToSlug();

            if (queryCache == null)
            {
                ClearCaches();
            }

            if (queryCache.ContainsKey(name))
            {
                return queryCache[name];
            }

            foreach (EntityPrototype prototype in itemPrototypes)
            {
                if (prototype.name == name)
                {
                    queryCache.Add(name, prototype);

                    return prototype;
                }
            }

            return null;
        }

        private static void ClearCaches()
        {
            queryCache = new Dictionary<string, EntityPrototype>();
        }
    }
}