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
        public Parser(object obj, ParserSettings parserSettings = null)
        {
            TargetObject = obj;

            if (parserSettings != null)
                ParserSettings = parserSettings;

            OptionMethods = TargetObject.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OptionForActionAttribute), false).Length > 0)
                .ToArray();

            OptionProperties = TargetObject.GetType()
                .GetProperties()
                .Where(m => m.GetCustomAttributes(typeof(OptionForPropertyAttribute), false).Length > 0)
                .ToArray();
        }

        public ParserSettings ParserSettings { get; } = new();

        private object TargetObject { get; }

        private MethodInfo[] OptionMethods { get; }

        private PropertyInfo[] OptionProperties { get; }

        public string HelpText
        {
            get
            {
                var stringBuilder = new StringBuilder("\nApplication options:\n");
                foreach (MethodInfo method in OptionMethods)
                {
                    var attribute = method.GetCustomAttribute<OptionForActionAttribute>();
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

        public void Parse(string[] args)
        {
            List<Action> actionsToInvoke = new();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                    throw new UnexpectedValueException($"Unexpected argument '{args[i]}' is not an option.");

                if (TryParseArgumentForAction(args[i], out Action action))
                {
                    // Actions will be invoked at the end of this method.
                    actionsToInvoke.Add(action);
                }
                else if (i + 1 < args.Length && TryParseArgumentForProperty(args[i], args[i + 1]))
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

            foreach (var action in actionsToInvoke)
                action.Invoke();
        }

        private bool TryParseArgumentForAction(string arg, out Action action)
        {
            foreach (MethodInfo method in OptionMethods)
            {
                var attribute = method.GetCustomAttribute<OptionForActionAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
                    return true;
                }
            }

            action = default;
            return false;
        }

        private bool TryParseArgumentForProperty(string arg, string value)
        {
            foreach (PropertyInfo property in OptionProperties)
            {
                var attribute = property.GetCustomAttribute<OptionForPropertyAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    property.SetValue(TargetObject, value);
                    return true;
                }
            }

            return false;
        }
    }
}
