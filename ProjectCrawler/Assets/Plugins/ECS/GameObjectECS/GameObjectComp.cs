namespace ECS.GameObjectECS
{
    using System;
    using ECS;
    using UnityEngine;

    [Serializable]
    //[PartOfComponentSystem(typeof(GameObjectCompSystem))]
    public class GameObjectComp : ECS.Component
    {
        public GameObjectECSBridge bridge { get; protected set; }

        public GameObject gameObject
        {
            get
            {
                return this.bridge.gameObject;
            }
        }

        public Transform transform
        {
            get
            {
                return this.bridge.transform;
            }
        }

        public GameObjectComp() { }

        public GameObjectComp(GameObjectECSBridge bridge)
        {
            this.SetBridge(bridge);
        }

        public void SetBridge(GameObjectECSBridge bridge)
        {
            if (bridge.gameObject == null)
            {
                Debug.LogError("GameObjectComponent--SetGameObject "
                    + "Should be passed a GameObjectECSBridge that is attached to a gameObject");

                return;
            }

            this.bridge = bridge;
        }
    }
}