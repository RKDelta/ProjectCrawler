namespace ECS.GameObjectECS
{
    using System;
    using ECS;
    using UnityEngine;
    using ECS.Controllers;
    
    [System.Serializable]
    public class GameObjectECSBridge : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        
        private Entity _entity;

        public Entity entity
        {
            get
            {
                return this._entity;
            }

            set
            {
                this._entity = value;

                if (this._entity.GetComponent<GameObjectComp>() == null)
                {
                    this._entity.AddComponent(new GameObjectComp(this));
                }

                this.entity.GetComponent<GameObjectComp>().SetBridge(this);
            }
        }

        public void Awake()
        {
            var prototype = ECSController.Instance.GetPrototypeByName(this.name);

            if (prototype != null)
            {
                this.entity = prototype.Instantiate(this);
            }
        }

        public void Update()
        {
            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        public void FixedUpdate()
        {
            if (this.OnFixedUpdate != null)
            {
                this.OnFixedUpdate();
            }
        }
    }
}
