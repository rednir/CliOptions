using System;

namespace CliOptions
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(string longName, char shortName = default, string description = default)
        {
            LongName = longName;
            ShortName = shortName;
            Description = description;
        }

        public string LongName { get; }

        public char ShortName { get; }

        public string Description { get; set; }
    }
}