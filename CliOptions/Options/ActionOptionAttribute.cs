using System;

namespace CliOptions.Options
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodOptionAttribute : Option
    {
        public MethodOptionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}