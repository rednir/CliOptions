using System;

namespace CliOptions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(
            string longName,
            char shortName = default,
            string description = default,
            string valueName = default)
        {
            LongName = longName;
            ShortName = shortName;
            Description = description;
            ValueName = valueName;
        }

        public string LongName { get; set; }

        public char ShortName { get; set; }

        public string Description { get; set; }

        public string ValueName { get; set; }
    }
}