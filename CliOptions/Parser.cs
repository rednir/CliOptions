using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using CliOptions.Exceptions;
using CliOptions.Options;

namespace CliOptions
{
    public class Parser
    {
        public Parser(ParserSettings parserSettings = null)
        {
            if (parserSettings != null)
                ParserSettings = parserSettings;

            ActionOptions = GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(MethodOptionAttribute), false).Length > 0)
                .ToArray();

            PropertyOptions = GetType()
                .GetProperties()
                .Where(m => m.GetCustomAttributes(typeof(PropertyOptionAttribute), false).Length > 0)
                .ToArray();
        }

        public string HelpText
        {
            get
            {
                var stringBuilder = new StringBuilder("\nApplication options:\n");
                foreach (MethodInfo method in ActionOptions)
                {
                    var attribute = method.GetCustomAttribute<MethodOptionAttribute>();
                    stringBuilder
                        .Append("  ")
                        .Append(attribute.ShortName == default ? string.Empty : $"-{attribute.ShortName}, ")
                        .Append("--")
                        .Append(attribute.LongName)
                        .Append("\t\t\t")
                        .AppendLine(attribute.Description);
                }

                return stringBuilder.ToString();
            }
        }

        public ParserSettings ParserSettings { get; } = new();

        private MethodInfo[] ActionOptions { get; }

        private PropertyInfo[] PropertyOptions { get; }

        public void Parse(string[] args)
        {
            List<MethodInfo> methodsToInvoke = new();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                    throw new UnexpectedValueException($"Unexpected argument '{args[i]}' is not an option.");

                if (TryParseArgumentAsMethodOption(args[i], out MethodInfo action))
                {
                    // Actions will be invoked at the end of this method.
                    methodsToInvoke.Add(action);
                }
                else if (i + 1 < args.Length && TryParseArgumentAsPropertyOption(args[i], args[i + 1]))
                {
                    // Skip the next argument as we know this the
                    // value for the one we just parsed.
                    i++;
                }
                else
                {
                    throw new UnknownArgumentException(args[i]);
                }
            }

            foreach (MethodInfo method in methodsToInvoke)
                method.Invoke(this, null);
        }

        private bool TryParseArgumentAsMethodOption(string arg, out MethodInfo methodInfo)
        {
            foreach (MethodInfo method in ActionOptions)
            {
                var attribute = method.GetCustomAttribute<MethodOptionAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    methodInfo = method;
                    return true;
                }
            }

            methodInfo = default;
            return false;
        }

        private bool TryParseArgumentAsPropertyOption(string arg, string value)
        {
            foreach (PropertyInfo property in PropertyOptions)
            {
                var attribute = property.GetCustomAttribute<PropertyOptionAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    property.SetValue(this, value);
                    return true;
                }
            }

            return false;
        }
    }
}
