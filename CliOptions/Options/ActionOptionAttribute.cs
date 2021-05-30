using System;

namespace CliOptions.Options
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActionOptionAttribute : Option
    {
        public ActionOptionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}