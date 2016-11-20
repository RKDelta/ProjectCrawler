namespace ECS
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GlobalComponentSystemAttribute : System.Attribute
    {
    }
}
