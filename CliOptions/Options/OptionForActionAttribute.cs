using System;

namespace CliOptions.Options
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OptionForActionAttribute : Option
    {
        public OptionForActionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}