namespace ECS.Statistics
{
    using ECS;

    [RequireComponent(typeof(Statistic))]
    public abstract class CurrentValueControllerComp : Component
    {
        public CurrentValueControllerComp() : base()
        {
        }

        public abstract void Update();
    }
}