namespace ECS
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireComponentAttribute : System.Attribute
    {
        private List<Type> _required;

        public RequireComponentAttribute(params Type[] required)
        {
            this.required = new List<Type>();

            foreach (Type requiredComponent in required)
            {
                if (requiredComponent.IsSubclassOf(typeof(Component)))
                {
                    this.required.Add(requiredComponent);
                }
                else
                {
                    Debug.LogError(
                        "RequireComponentAttribute shoould be given Types derived from Component. "
                        + requiredComponent.ToString() + " is Invalid.");
                }
            }
        }

        public List<Type> required
        {
            get
            {
                return this._required;
            }

            set
            {
                this._required = value;
            }
        }
    }
}
