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
                StringBuilder stringBuilder = new();

                foreach (MethodInfo method in MethodOptions)
                {
                    stringBuilder.Append(
                        MakeStringForOption(
                            option: method.GetCustomAttribute<MethodOptionAttribute>(),
                            parameterTypes: method.GetParameters().Select(p => p.ParameterType).ToArray()));
                }

                foreach (PropertyInfo property in PropertyOptions)
                {
                    stringBuilder.Append(
                        MakeStringForOption(
                            option: property.GetCustomAttribute<PropertyOptionAttribute>(),
                            parameterTypes: new[] { property.PropertyType }));
                }

                return stringBuilder.ToString();
            }
        }

        /*public Option[] OrderedOptionAttributes
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
        }*/

        public ParserSettings ParserSettings { get; } = new();

        private MethodInfo[] MethodOptions { get; }

        private PropertyInfo[] PropertyOptions { get; }

        public void Parse(string[] args)
        {
            // `object[]` represents the parameters to pass into the method.
            Dictionary<MethodInfo, object[]> methodsToInvoke = new();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                    throw new UnexpectedValueException($"Unexpected argument '{args[i]}' is not an option.");

                if (TryParseArgumentAsMethodOption(ref i, args, out MethodInfo method, out object[] parameters))
                {
                    // Methods will be invoked after all arguments have been parsed.
                    methodsToInvoke.Add(method, parameters);
                }
                else if (i + 1 < args.Length && TryParseArgumentAsPropertyOption(ref i, args))
                {
                }
                else
                {
                    throw new UnknownArgumentException(args[i]);
                }
            }

            foreach (KeyValuePair<MethodInfo, object[]> pair in methodsToInvoke)
                pair.Key.Invoke(this, pair.Value);
        }

        private static string MakeStringForOption(Option option, Type[] parameterTypes)
        {
            StringBuilder parametersStringBuilder = new(" ");
            foreach (Type type in parameterTypes)
            {
                parametersStringBuilder
                    .Append('[')
                    .Append(type.Name.ToUpper())
                    .Append("] ");
            }

            StringBuilder stringBuilder = new();
            stringBuilder
                .Append("  ")
                .Append(option.ShortName == default ? string.Empty : $"-{option.ShortName}, ")
                .Append("--")
                .Append(option.LongName)
                .Append(parametersStringBuilder)
                .Append("\t\t\t")
                .AppendLine(option.Description);

            return stringBuilder.ToString();
        }

        private bool TryParseArgumentAsMethodOption(ref int i, string[] args, out MethodInfo methodOut, out object[] parametersOut)
        {
            foreach (MethodInfo method in MethodOptions)
            {
                var attribute = method.GetCustomAttribute<MethodOptionAttribute>();
                if (args[i] == "--" + attribute.LongName || args[i] == "-" + attribute.ShortName)
                {
                    ParameterInfo[] methodParameters = method.GetParameters();
                    List<object> objectsForParameters = new();
                    for (int j = i; j < methodParameters.Length; j++)
                    {
                        // Cast the arguments specified in `string[] args` so
                        // it can be passed into the method when invoking it.
                        i++;
                        objectsForParameters.Add(args[i]); // todo cast
                    }

                    methodOut = method;
                    parametersOut = objectsForParameters.ToArray();
                    return true;
                }
            }

            methodOut = null;
            parametersOut = Array.Empty<ParameterInfo>();
            return false;
        }

        private bool TryParseArgumentAsPropertyOption(ref int i, string[] args)
        {
            foreach (PropertyInfo property in PropertyOptions)
            {
                var attribute = property.GetCustomAttribute<PropertyOptionAttribute>();
                if (args[i] == "--" + attribute.LongName || args[i] == "-" + attribute.ShortName)
                {
                    // Next argument is the value to this option.
                    i++;
                    property.SetValue(this, args[i]);
                    return true;
                }
            }

            return false;
        }
    }
}
