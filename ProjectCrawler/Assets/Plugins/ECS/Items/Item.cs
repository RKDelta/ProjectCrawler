namespace ECS.Items
{
    using ECS;

    public class Item : Component
    {
        // public bool craftable;
        // public int minimumLevel;
        public float weight = 0.1f;

        public Item() { }

        public Item(float weight)
        {
            this.weight = weight;
        }

        public override Component Clone()
        {
            return new Item(this.weight);
        }
    }
}