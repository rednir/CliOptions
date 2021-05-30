using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using CliOptions.Exceptions;

namespace CliOptions
{
    public class Parser
    {
        public Parser(ParserSettings parserSettings = null)
        {
            if (parserSettings != null)
                ParserSettings = parserSettings;

            MethodOptions = GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OptionAttribute), false).Length > 0)
                .ToArray();

            PropertyOptions = GetType()
                .GetProperties()
                .Where(m => m.GetCustomAttributes(typeof(OptionAttribute), false).Length > 0)
                .ToArray();
        }

        public string HelpText
        {
            get
            {
                var stringBuilder = new StringBuilder();
                foreach (OptionAttribute option in OrderedOptionAttributes)
                {
                    stringBuilder
                        .Append("  ")
                        .Append(option.ShortName == default ? string.Empty : $"-{option.ShortName}, ")
                        .Append("--")
                        .Append(option.LongName)
                        .Append(option.ValueName == default ? string.Empty : $" [{option.ValueName}]")
                        .Append("\t\t\t")
                        .AppendLine(option.Description);
                }

                return stringBuilder.ToString();
            }
        }

        public OptionAttribute[] OrderedOptionAttributes
        {
            get
            {
                List<OptionAttribute> result = new();

                foreach (MethodInfo method in MethodOptions)
                    result.Add(method.GetCustomAttribute<OptionAttribute>());
                foreach (PropertyInfo property in PropertyOptions)
                    result.Add(property.GetCustomAttribute<OptionAttribute>());

                return result.OrderBy(o => o.LongName).ToArray();
            }
        }

        public ParserSettings ParserSettings { get; } = new();

        private MethodInfo[] MethodOptions { get; }

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
                    // Skip the next argument as we know it's the
                    // value for the option we just parsed.
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
            foreach (MethodInfo method in MethodOptions)
            {
                var attribute = method.GetCustomAttribute<OptionAttribute>();
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
                var attribute = property.GetCustomAttribute<OptionAttribute>();
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
