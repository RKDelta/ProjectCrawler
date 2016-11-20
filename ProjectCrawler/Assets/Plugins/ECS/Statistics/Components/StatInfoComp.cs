namespace ECS.Statistics
{
    using ECS;
    
    public class StatInfoComp : Component
    {
        private StatInfo _statInfo;

        public virtual StatInfo statInfo
        {
            get
            {
                return this._statInfo;
            }

            protected set
            {
                this._statInfo = value;
            }
        }

        public override Component Clone()
        {
            StatInfoComp clone = new StatInfoComp();
            clone.statInfo = this.statInfo.Clone();

            return clone;
        }

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            this.entity.visualName = this.statInfo.visualName;
        }
    }
}
