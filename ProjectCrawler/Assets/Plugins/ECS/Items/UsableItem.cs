namespace ECS.Items
{
    using System;
    using System.Collections.Generic;
    using ECS;
    using ECS.EntityMonoBehaviours;
    using ECS.GameObjectECS;

    public delegate void ActionUseItem(Entity user, Entity target);

    [ECS.RequireComponent(typeof(Item))]
    public class UsableItem : Component
    {
        public bool consumeOnUse;

        private bool caught = false;

        public UsableItem()
        {
            this.consumeOnUse = false;
        }

        public UsableItem(bool consumeOnUse)
        {
            this.consumeOnUse = consumeOnUse;
        }

        public UsableItem(ActionUseItem use, bool consumeOnUse = false)
        {
            this.OnUse = use;
            this.consumeOnUse = consumeOnUse;
        }

        public event ActionUseItem OnUse;

        public void Use(Entity user, Entity target)
        {
            if (this.caught == false)
            {
                if (this.consumeOnUse == true)
                {
                    InventoryComp userInventory = user.GetComponent<InventoryComp>();

                    userInventory.TakeItem(this.GetComponent<Item>(), 1);
                }

                if (this.OnUse != null)
                {
                    this.OnUse(user, target);
                }
            }
        }

        public void CatchUse()
        {
            this.caught = true;
        }

        public void ReleaseUse()
        {
            this.caught = false;
        }
        
        public override Component Clone()
        {
            return new UsableItem(this.OnUse, this.consumeOnUse);
        }

        public override void SetEntity(Entity entity)
        {
            this.OnUse = null;

            base.SetEntity(entity);
        }
    }
}