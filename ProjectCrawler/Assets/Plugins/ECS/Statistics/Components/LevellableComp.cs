namespace ECS.Statistics
{
    using ECS;

    [RequireComponent(typeof(Statistic))]
    public class LevellableStat : Component
    {
        #region _level and level accessor
        private int _level = 0;

        public int level
        {
            get
            {
                return this._level;
            }

            set
            {
                this._level = value;
                this.UpdateMax();
            }
        }

        public virtual float maxPerLevel { get; set; }

        public virtual float baseMax { get; set; }

        public void UpdateMax()
        {
            this.entity.GetComponent<Statistic>().max = (this.level * this.maxPerLevel) + this.baseMax;
        }
        #endregion

        public override Component Clone()
        {
            LevellableStat clone = new LevellableStat();
            clone.maxPerLevel = this.maxPerLevel;
            clone.baseMax = this.baseMax;

            return clone;
        }
    }
}
