using System;

namespace CliOptions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(string longName, char shortName, string description)
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