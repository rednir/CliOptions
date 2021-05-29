using System;
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
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                    throw new UnexpectedValueException($"Unexpected argument '{args[i]}' is not an option.");

                // Try parse argument as method.
                foreach (MethodInfo method in OptionMethods)
                {
                    var attribute = method.GetCustomAttribute<OptionAttribute>();
                    if (args[i] == "--" + attribute.LongName || args[i] == "-" + attribute.ShortName)
                    {
                        var action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
                        action.Invoke();
                    }
                }
            }
        }
    }
}
