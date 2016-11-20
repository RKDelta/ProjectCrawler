namespace ECS.Statistics
{
    public class StatInfo
    {
        public string visualName = "Created as Default";
        public string description = "Created as Default";
        public float baseMax = 100;
        public float maxPerLevel = 5;

        public float defaultRegenPS = 5;

        // FIXME: Currently unused
        public string updateEarlyFuncName = null;
        public string updateFuncName = null;
        public string updateLateFuncName = null;

        #region Regen Value Fields
        private float? _idleRegenPS = null;
        private float? _activeRegenPS = null;
        private float? _heavyRegenPS = null;
        private float? _outOfFuelRegenPS = null;
        private float? _exhaustedRegenPS = null;
        private float? _unconsciousRegenPS = null;
        #endregion

        public StatInfo()
        {
        }

        #region Standard StatInfo
        public StatInfo(
            string visualName,
            string description,
            float baseMax,
            float maxPerLevel,
            float defaultRegenPS,
            float? _idleRegenPS = null,
            float? _activeRegenPS = null,
            float? _heavyRegenPS = null,
            float? _outOfFuelRegenPS = null,
            float? _exhaustedRegenPS = null,
            float? _unconsciousRegenPS = null)
        {
            this.visualName = visualName;
            this.description = description;
            this.baseMax = baseMax;
            this.maxPerLevel = maxPerLevel;

            this.defaultRegenPS = defaultRegenPS;

            this._idleRegenPS = _idleRegenPS;
            this._activeRegenPS = _activeRegenPS;
            this._heavyRegenPS = _heavyRegenPS;
            this._outOfFuelRegenPS = _outOfFuelRegenPS;
            this._exhaustedRegenPS = _exhaustedRegenPS;
            this._unconsciousRegenPS = _unconsciousRegenPS;
        }
        #endregion

        #region JSON Constructor
        public StatInfo(JSONObject obj)
        {
            for (int i = 0; i < obj.list.Count; i++)
            {
                string key = obj.keys[i].ToLower();
                switch (key)
                {
                    case "visualname":
                    case "visual_name":
                    case "name":
                        this.visualName = obj.list[i].str;
                        break;
                    case "description":
                        this.description = obj.list[i].str;
                        break;
                    case "basemax":
                    case "base_max":
                        this.baseMax = obj.list[i].n;
                        break;
                    case "maxperlevel":
                    case "max_per_level":
                        this.maxPerLevel = obj.list[i].n;
                        break;
                    case "default_regen":
                    case "defaultregen":
                        this.defaultRegenPS = obj.list[i].n;
                        break;
                    case "idleregen":
                    case "idle_regen":
                        this.idleRegenPS = obj.list[i].n;
                        break;
                    case "activeregen":
                    case "active_regen":
                        this.activeRegenPS = obj.list[i].n;
                        break;
                    case "heavyregen":
                    case "heavy_regen":
                        this.heavyRegenPS = obj.list[i].n;
                        break;
                    case "exhaustedregen":
                    case "exhausted_regen":
                        this.exhaustedRegenPS = obj.list[i].n;
                        break;
                    case "outoffuelregen":
                    case "outoffuel_regen":
                        this.outOfFuelRegenPS = obj.list[i].n;
                        break;
                    case "unconsciousregen":
                    case "unconscious_regen":
                        this.unconsciousRegenPS = obj.list[i].n;
                        break;
                    case "updateearlyfunc":
                    case "update_early_func":
                    case "updateearlyfunction":
                    case "update_early_function":
                        this.updateEarlyFuncName = obj.list[i].str;
                        break;
                    case "updatefunc":
                    case "update_func":
                    case "updatefunction":
                    case "update_function":
                    case "updatemainfunc":
                    case "update_main_func":
                    case "updatemainfunction":
                    case "update_main_function":
                        this.updateFuncName = obj.list[i].str;
                        break;
                    case "updatelatefunc":
                    case "update_late_func":
                    case "updatelatefunction":
                    case "update_late_function":
                        this.updateLateFuncName = obj.list[i].str;
                        break;
                }
            }
        }
        #endregion

        #region Regen Value Properties
        public float idleRegenPS
        {
            get
            {
                return this._idleRegenPS ?? this.defaultRegenPS;
            }

            protected set
            {
                this._idleRegenPS = value;
            }
        }

        public float activeRegenPS
        {
            get
            {
                return this._activeRegenPS ?? this.defaultRegenPS;
            }

            protected set
            {
                this._activeRegenPS = value;
            }
        }

        public float heavyRegenPS
        {
            get
            {
                return this._heavyRegenPS ?? this.defaultRegenPS;
            }

            protected set
            {
                this._heavyRegenPS = value;
            }
        }

        public float outOfFuelRegenPS
        {
            get
            {
                return this._outOfFuelRegenPS ?? -this.defaultRegenPS;
            }

            protected set
            {
                this._outOfFuelRegenPS = value;
            }
        }

        public float exhaustedRegenPS
        {
            get
            {
                return this._exhaustedRegenPS ?? this.defaultRegenPS;
            }

            protected set
            {
                this._exhaustedRegenPS = value;
            }
        }

        public float unconsciousRegenPS
        {
            get
            {
                return this._unconsciousRegenPS ?? this.defaultRegenPS;
            }

            protected set
            {
                this._unconsciousRegenPS = value;
            }
        }
        #endregion

        #region Clone
        public StatInfo Clone()
        {
            StatInfo clone = new StatInfo(
                this.visualName,
                this.description,
                this.baseMax,
                this.maxPerLevel,
                this.defaultRegenPS,
                this._idleRegenPS,
                this._activeRegenPS,
                this._heavyRegenPS,
                this._outOfFuelRegenPS,
                this._exhaustedRegenPS,
                this._unconsciousRegenPS);

            clone.updateEarlyFuncName = this.updateEarlyFuncName;
            clone.updateFuncName = this.updateFuncName;
            clone.updateLateFuncName = this.updateLateFuncName;

            return clone;
        }
        #endregion
    }
}