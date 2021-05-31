using System;

namespace CliOptions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MethodOptionAttribute : Option
    {
        public MethodOptionAttribute(string longName, char shortName = default, string description = default)
            : base(longName, shortName, description)
        {
        }
    }
}