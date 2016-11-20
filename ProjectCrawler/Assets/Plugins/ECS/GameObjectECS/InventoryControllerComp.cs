namespace ECS.GameObjectECS
{
    using System;
    using ECS.Controllers;
    using ECS;
    using ECS.Items;
    using UnityEngine;

    [ECS.RequireComponent(typeof(GameObjectComp))]
    [ECS.RequireComponent(typeof(InventoryComp))]
    public class InventoryControllerComp : ECS.Component
    {
        private int _selectionIndex = 0;

        public int selectionIndex
        {
            get
            {
                return this._selectionIndex;
            }

            set
            {
                InventoryComp inventory = this.GetComponent<InventoryComp>();

                this._selectionIndex = value;
                if (this._selectionIndex < 0)
                {
                    this._selectionIndex = 0;
                }
                else if (this._selectionIndex > inventory.hotbar.Length - 1)
                {
                    this._selectionIndex = inventory.hotbar.Length - 1;
                }
            }
        }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            this.GetComponent<GameObjectComp>().bridge.OnUpdate += this.Update;
        }

        public void Update()
        {
            if (ECSController.Instance.active)
            {
                for (int i = 0; i <= 9; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        int index = i;

                        if (index == 0)
                        {
                            index = 9;
                        }
                        else
                        {
                            index -= 1;
                        }

                        this.selectionIndex = index;
                    }
                }

                float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

                if (scrollAmount >= 0.1f)
                {
                    this.selectionIndex -= 1;
                }
                else if (scrollAmount <= -0.1f)
                {
                    this.selectionIndex += 1;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    this.UseItemInHotbar(this.selectionIndex);
                }
            }
        }

        protected virtual void UseItemInHotbar(int index)
        {
            InventoryComp inventory = this.GetComponent<InventoryComp>();

            if (inventory.hotbar[index] != null)
            {
                UsableItem usableItem =
                    inventory.GetItemStackInHotbar(index).item.entity.GetComponent<UsableItem>();

                if (usableItem != null)
                {
                    usableItem.Use(this.entity, null);
                }
            }
        }
    }
}
