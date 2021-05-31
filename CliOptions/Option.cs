using System;

namespace CliOptions
{
    public abstract class Option : Attribute
    {
        protected Option(string longName, char shortName, string description)
        {
            LongName = longName;
            ShortName = shortName;
            Description = description;
        }

        public string LongName { get; set; }

        public char ShortName { get; set; }

        public string Description { get; set; }
    }
}