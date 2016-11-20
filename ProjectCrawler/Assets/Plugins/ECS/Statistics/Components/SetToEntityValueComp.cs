namespace ECS.Statistics
{
    using System;
    using ECS;

    [RequireComponent(typeof(Statistic))]
    public class SetToEntityValueComp : CurrentValueControllerComp
    {
        public StatInfo statInfo;

        public SetToEntityValueComp() : base()
        {
        }

        public SetToEntityValueComp(Action<Action<float>> registerUpdateFunc) : base()
        {
            registerUpdateFunc(this.SetToValue);
        }

        public override Component Clone()
        {
            return new SetToEntityValueComp();
        }

        public override void Update()
        {
        }

        public void SetToValue(float newValue)
        {
            Statistic statistic = this.entity.GetComponent<Statistic>();

            if (statistic != null)
            {
                statistic.current = newValue;
            }
        }
    }
}
