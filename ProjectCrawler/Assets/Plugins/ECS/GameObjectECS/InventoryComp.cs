namespace ECS.GameObjectECS
{
    using System;
    using System.Collections.Generic;
    using ECS.Items;
    using UnityEngine;

    public class InventoryComp : ECS.Component
    {
        /// <summary>
        /// Create an inventory with a default hotbar size of 10
        /// </summary>
        public InventoryComp()
        {
            this.items = new Dictionary<string, ItemStack>();
            this.hotbar = new string[10];

            if (this.OnInventoryChanged != null)
            {
                this.OnInventoryChanged();
            }
        }

        /// <summary>
        /// Create an inventory
        /// </summary>
        /// <param name="maxHotbarSize">The number of stacks the hotbar can hold.</param>
        public InventoryComp(int maxHotbarSize)
        {
            this.items = new Dictionary<string, ItemStack>();
            this.hotbar = new string[maxHotbarSize];

            if (this.OnInventoryChanged != null)
            {
                this.OnInventoryChanged();
            }
        }

        public event Action OnInventoryChanged;

        public Dictionary<string, ItemStack> items { get; protected set; }

        public string[] hotbar { get; protected set; }

        /// <summary>
        /// Get the total weight of the inventory.
        /// </summary>
        public float totalWeight
        {
            get
            {
                float sum = 0;
                foreach (ItemStack itemStack in this.items.Values)
                {
                    sum += itemStack.weight;
                }

                return sum;
            }
        }

        /// <summary>
        /// Get the total size of the inventory.
        /// </summary>
        public int currSize
        {
            get
            {
                return this.items.Count;
            }
        }

        /// <summary>
        /// Try to remove an item from the inventory -- 
        /// returns false if cannot remove from inventory (because there aren't enough of the item in it).
        /// </summary>
        /// <param name="item">The Prototype of the Item you want to remove from the inventory.</param>
        /// <param name="amount">The amount of the item you want to add to the inventory.</param>
        /// <returns>Returns the number of items actually removed inventory.</returns>
        public int TakeItem(Item item, int amount = 1)
        {
            if (this.items.ContainsKey(item.entity.name) == true)
            {
                int removed = amount;

                this.items[item.entity.name].amount -= amount;

                if (this.items[item.entity.name].amount == 0)
                {
                    this.items.Remove(item.entity.name);
                    this.hotbar[Array.IndexOf(this.hotbar, item.entity.name)] = null;
                }
                else if (this.items[item.entity.name].amount < 0)
                {
                    removed += this.items[item.entity.name].amount;
                    this.items[item.entity.name].amount = 0;
                }

                if (this.OnInventoryChanged != null)
                {
                    this.OnInventoryChanged();
                }

                return removed;
            }
            else
            {
                Debug.LogError("InventoryComp -- TakeItemFromInventory -- The InventoryComp did not contain that item");

                return 0;
            }
        }

        /// <summary>
        /// Try to add an item to the inventory -- returns false if there are already too many stacks in the inventory to add to it.
        /// </summary>
        /// <param name="itemPrototype">The prototype of the item you want to add to the inventory.</param>
        /// <param name="amount">The amount of the item you want to add to the inventory.</param>
        /// <returns>Returns false if there are already too many stacks in the inventory to add to it.</returns>
        public void AddItem(Item itemPrototype, int amount = 1)
        {
            if (amount < 0)
            {
                Debug.LogError("InventoryComp -- AddItemToInventory -- amount must be greater than 0 -- " + amount + " is invalid");
            }

            if (this.items.ContainsKey(itemPrototype.entity.name))
            {
                this.items[itemPrototype.entity.name].amount += amount;
            }
            else
            {
                this.items.Add(itemPrototype.entity.name, new ItemStack(itemPrototype));
                this.items[itemPrototype.entity.name].amount = amount;
            }

            if (this.OnInventoryChanged != null)
            {
                this.OnInventoryChanged();
            }
        }

        /// <summary>
        /// Get the item prototype of the item at a position in the hotbar, starting from position 0.
        /// </summary>
        /// <param name="index">The index of the item, starting from 0.</param>
        /// <returns>The item prototype.</returns>
        public ItemStack GetItemStackInHotbar(int index)
        {
            return this.items[this.hotbar[index]];
        }

        public void MoveItemToHotbar(Item item, int index)
        {
            if (this.items.ContainsKey(item.entity.name) && index < this.hotbar.Length)
            {
                this.hotbar[index] = item.entity.name;
            }
            else if (this.items.ContainsKey(item.entity.name))
            {
                Debug.LogError("InventoryComp -- MoveItemToHotbar -- Could Not Move Item to Hotbar -- Invalid Hotbar Index");
            }
            else
            {
                Debug.LogError("InventoryComp -- MoveItemToHotbar -- Could Not Move Item to Hotbar -- The InventoryComp does not contain that item");
            }
        }
    }
}