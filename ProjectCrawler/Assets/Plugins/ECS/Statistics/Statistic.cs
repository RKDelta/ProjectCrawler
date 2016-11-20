namespace ECS.Statistics
{
    using System;
    using ECS;

    public class Statistic : Component
    {
        private float _current = 0;

        public Statistic() : base() { }

        public Statistic(float max) : base()
        {
            this.max = max;
        }

        public Statistic(float current, float max) : base()
        {
            this.current = current;
            this.max = max;
        }

        public enum StatisticState
        {
            FULL,
            PLENTY,
            LOW,
            NONE,
            OVERLOADED
        }

        public float current
        {
            get
            {
                return this._current;
            }

            set
            {
                this._current = value;

                if (this.current > this.max)
                {
                    this.state = StatisticState.OVERLOADED;
                }
                else if (this.current == this.max)
                {
                    this.state = StatisticState.FULL;
                }
                else if (this.proportional > 0.25f)
                {
                    this.state = StatisticState.PLENTY;
                }
                else if (this.current > 0.0f)
                {
                    this.state = StatisticState.LOW;
                }
                else
                {
                    this.state = StatisticState.NONE;
                }
            }
        }

        public float max { get; set; }

        public float proportional
        {
            get
            {
                return this.current / this.max;
            }

            set
            {
                this.current = value * this.max;
            }
        }

        public StatisticState state { get; protected set; }

        public override Component Clone()
        {
            return new Statistic(this.current, this.max);
        }
    }
}
