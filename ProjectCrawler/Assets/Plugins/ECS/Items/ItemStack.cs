namespace ECS.Items
{
    using System;
    using ECS;

    public class ItemStack
    {
        public int _amount = 1;

        public ItemStack(Item item)
        {
            this.item = ((Entity)item.entity.Clone()).GetComponent<Item>();
            if (this.OnStackChanged != null)
            {
                this.OnStackChanged();
            }
        }

        public event Action OnStackChanged;

        public Item item { get; protected set; }

        public int amount
        {
            get
            {
                return this._amount;
            }

            set
            {
                this._amount = value;
                if (this.OnStackChanged != null)
                {
                    this.OnStackChanged();
                }
            }
        }

        public float weight
        {
            get
            {
                return this.item.weight * this.amount;
            }
        }
    }
}