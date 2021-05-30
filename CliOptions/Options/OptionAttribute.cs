using System;

namespace CliOptions.Options
{
    public abstract class Option : Attribute
    {
        protected Option(string longName, char shortName, string description)
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