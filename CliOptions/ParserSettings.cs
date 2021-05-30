using System;

namespace CliOptions
{
    public class ParserSettings
    {
        public string HelpOptionLongName { get; set; } = "--help";

        public char HelpOptionShortName { get; set; } = 'h';

        public string HelpOptionDescription { get; set; } = "Gets a list of available options.";
    }
}