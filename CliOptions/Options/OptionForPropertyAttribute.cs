using System;

namespace CliOptions.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionForPropertyAttribute : Option
    {
        public OptionForPropertyAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}