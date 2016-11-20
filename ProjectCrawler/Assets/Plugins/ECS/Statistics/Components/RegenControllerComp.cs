namespace ECS.Statistics
{
    using ECS;

    [PartOfComponentSystem(typeof(RegenFuelSystem))]
    [RequireComponent(typeof(StatInfoComp), typeof(Statistic))]
    public class RegenControllerComp : CurrentValueControllerComp
    {
        public RegenControllerComp() : base()
        {
        }

        public override Component Clone()
        {
            return new RegenControllerComp();
        }

        public override void Update()
        {
        }
    }
}
