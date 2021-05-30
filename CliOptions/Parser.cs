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

            ActionOptions = TargetObject.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ActionOptionAttribute), false).Length > 0)
                .ToArray();

            PropertyOptions = TargetObject.GetType()
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
                    var attribute = method.GetCustomAttribute<ActionOptionAttribute>();
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

        private object TargetObject { get; }

        private MethodInfo[] ActionOptions { get; }

        private PropertyInfo[] PropertyOptions { get; }

        public void Parse(string[] args)
        {
            List<Action> actionsToInvoke = new();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                    throw new UnexpectedValueException($"Unexpected argument '{args[i]}' is not an option.");

                if (TryParseArgumentAsActionOption(args[i], out Action action))
                {
                    // Actions will be invoked at the end of this method.
                    actionsToInvoke.Add(action);
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

            foreach (var action in actionsToInvoke)
                action.Invoke();
        }

        private bool TryParseArgumentAsActionOption(string arg, out Action action)
        {
            foreach (MethodInfo method in ActionOptions)
            {
                var attribute = method.GetCustomAttribute<ActionOptionAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
                    return true;
                }
            }

            action = default;
            return false;
        }

        private bool TryParseArgumentAsPropertyOption(string arg, string value)
        {
            foreach (PropertyInfo property in PropertyOptions)
            {
                var attribute = property.GetCustomAttribute<PropertyOptionAttribute>();
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
