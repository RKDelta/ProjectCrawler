/*namespace ECS.GameObjectECS
{
    using System.Collections.Generic;
    using ECS;
    using UnityEngine;
    using ECS.Utilities;

    [GlobalComponentSystem]
    public class GameObjectCompSystem : ComponentSystem
    {
        private static GameObjectCompSystem _instance;

        protected GameObjectCompSystem()
        {
            this.availableBridges = new Dictionary<string, GameObjectECSBridge>();
        }

        public static GameObjectCompSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObjectCompSystem();
                }

                return _instance;
            }
        }

        public Dictionary<string, GameObjectECSBridge> availableBridges { get; protected set; }

        public void AddBridge(GameObjectECSBridge bridge)
        {
            if (bridge.gameObject == null)
            {
                Debug.LogError("GameObjectComponentSystem--AddBridge "
                    + "Should be passed a GameObjectECSBridge with a non-null gameObject");

                return;
            }

            this.availableBridges.Add(bridge.gameObject.name.ToSlug(), bridge);
        }

        public override void AddComponent(ECS.Component instance)
        {
            if (instance is GameObjectComp == false)
            {
                Debug.Log(
                    "GameObjectComponentSystem--AddComponent--"
                    + string.Format(
                        "Should be passed a component of type GameObjectComponent not {0}",
                        instance.GetType().ToString()));

                return;
            }

            if (((GameObjectComp)instance).bridge == null)
            {
                if (this.availableBridges.ContainsKey(instance.name))
                {
                    ((GameObjectComp)instance).SetBridge(this.availableBridges[instance.name]);
                    this.availableBridges[instance.name].entity = instance.entity;

                    this.availableBridges.Remove(instance.name);
                }
                else
                {
                    Debug.LogError(
                        "GameObjectComponentSystem--AddComponent--"
                        + string.Format(
                            "There is no available GameObject for the entity of name {0}",
                            instance.name));

                    return;
                }
            }

            base.AddComponent(instance);
        }
    }
}
*/