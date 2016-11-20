namespace ECS
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PartOfComponentSystemAttribute : System.Attribute
    {
        public PartOfComponentSystemAttribute(Type parentSystemType)
        {
            if (parentSystemType.IsSubclassOf(typeof(ComponentSystem)))
            {
                this.parentSystemType = parentSystemType;
            }
            else
            {
                Debug.LogError(string.Format(
                    "PartOfComponentSystemAttribute--"
                    + "Requires a type to be passed that is derived from StatComponentSystem not {0}",
                    parentSystemType.ToString()));
            }
        }

        public Type parentSystemType { get; protected set; }
    }
}