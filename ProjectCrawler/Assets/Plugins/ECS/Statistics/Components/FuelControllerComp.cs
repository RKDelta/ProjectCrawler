namespace ECS.Statistics
{
    using ECS;

    [PartOfComponentSystem(typeof(RegenFuelSystem))]
    [RequireComponent(typeof(StatInfoComp), typeof(Statistic))]
    public class FuelControllerComp : CurrentValueControllerComp
    {
        public StatInfo statInfo;

        public FuelControllerComp() : base()
        {
        }

        public override Component Clone()
        {
            return new FuelControllerComp();
        }

        public override void Update()
        {
        }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            this.statInfo = entity.GetComponent<StatInfoComp>().statInfo;
        }
    }
}
