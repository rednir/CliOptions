using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace CliOptions
{
    public class Parser<T>
    {
        public Parser()
        {
            OptionMethods = typeof(T)
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OptionAttribute), false).Length > 0)
                .ToArray();
        }

        private MethodInfo[] OptionMethods { get; }

        public string HelpText
        {
            get
            {
                var stringBuilder = new StringBuilder("\nApplication options:\n");
                foreach (MethodInfo method in OptionMethods)
                {
                    var attribute = method.GetCustomAttribute<OptionAttribute>();
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

                if (TryParseArgumentAsAction(args[i], out Action action))
                {
                    actionsToInvoke.Add(action);
                }
                else
                {
                    throw new UnknownArgumentException(args[i]);
                }
            }

            foreach (var action in actionsToInvoke)
                action.Invoke();
        }

        private bool TryParseArgumentAsAction(string arg, out Action action)
        {
            foreach (MethodInfo method in OptionMethods)
            {
                var attribute = method.GetCustomAttribute<OptionAttribute>();
                if (arg == "--" + attribute.LongName || arg == "-" + attribute.ShortName)
                {
                    action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
                    return true;
                }
            }

            action = default;
            return false;
        }
    }
}
