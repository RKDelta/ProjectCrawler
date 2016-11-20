/*namespace ProjectSpell.EntityMonoBehaviours
{
    using System;
    using ProjectSpell.Controllers;
    using ProjectSpell.ECS;
    using ProjectSpell.GameObjectECS;
    using ProjectSpell.Models;
    using ProjectSpell.Models.Items;
    using ProjectSpell.Models.Statistics;
    using UnityEngine;

    /// <summary>
    /// Controls the player.
    /// </summary>
    [Serializable]
    public class Player : EntityMB
    {
        private int _selectionIndex = 0;

        public event Action<float> OnWeightChanged;

        public InventoryComp inventory { get; protected set; }

        public int selectionIndex
        {
            get
            {
                return this._selectionIndex;
            }

            set
            {
                this._selectionIndex = value;
                if (this._selectionIndex < 0)
                {
                    this._selectionIndex = 0;
                }
                else if (this._selectionIndex > this.inventory.hotbar.Length - 1)
                {
                    this._selectionIndex = this.inventory.hotbar.Length - 1;
                }
            }
        }

        protected override void Start()
        {


            this.inventory = new InventoryComp(10);
            this.inventory.OnInventoryChanged += () =>
            {
                this.OnWeightChanged(inventory.totalWeight);
            };

            foreach (Item item in WorldController.Instance.itemPrototypes.Values)
            {
                this.inventory.AddItem(item, 1000);
            }
        }

        protected override void Update()
        {
            if (GameController.paused == false)
            {
                if (this.statisticSystem.GetStatistic("Vigor").current <= 0.0f)
                {
                    this.statisticSystem.GetSystem<RegenFuelSystem>().regenState = RegenFuelSystem.RegenState.UNCONSCIOUS;
                }

                if (this.statisticSystem.GetStatistic("Hunger").current >= 5.0f
                    && this.statisticSystem.GetSystem<RegenFuelSystem>().regenState == RegenFuelSystem.RegenState.UNCONSCIOUS)
                {
                    this.statisticSystem.GetSystem<RegenFuelSystem>().regenState = RegenFuelSystem.RegenState.IDLE;
                }

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
            if (this.inventory.hotbar[index] != null)
            {
                UsableItem usableItem = 
                    this.inventory.GetItemStackInHotbar(index).item.entity.GetComponent<UsableItem>();

                if (usableItem != null)
                {
                    usableItem.Use(this, null);
                }
            }
        }
    }
}*/