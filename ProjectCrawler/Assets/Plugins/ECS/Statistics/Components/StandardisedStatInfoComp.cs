namespace ECS.Statistics
{
    using ECS;
    
    public class StandardisedStatInfoComp : StatInfoComp
    {
        private string statInfoName;

        public StandardisedStatInfoComp(string statInfoName)
        {
            this.statInfoName = statInfoName;
            this.statInfo = StandardisedStatInfo.GetStatInfo(statInfoName);
        }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            this.entity.GetComponent<Statistic>().max = this.statInfo.baseMax; // FIXME: Find somewhere better to do this
        }

        public override Component Clone()
        {
            StandardisedStatInfoComp clone = new StandardisedStatInfoComp(this.statInfoName);
            clone.statInfo = this.statInfo;

            return clone;
        }
    }
}
