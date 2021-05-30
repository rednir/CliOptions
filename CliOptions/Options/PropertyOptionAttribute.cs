using System;

namespace CliOptions.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyOptionAttribute : Option
    {
        public PropertyOptionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}