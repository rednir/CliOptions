using System;

namespace CliOptions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyOptionAttribute : Option
    {
        public PropertyOptionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}