using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using CliOptions.Exceptions;

namespace CliOptions
{
    /// <summary>Base class containing methods for parsing command-line arguments.</summary>
    public abstract class ArgumentsParser
    {
        protected ArgumentsParser(ArgumentsParserSettings parserSettings = null)
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

        /// <summary>Gets a formatted string representing the available options declared in this class.</summary>
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

        public ArgumentsParserSettings ParserSettings { get; } = new();

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
                else if (TryParseArgumentAsPropertyOption(ref i, args))
                {
                }
                else
                {
                    throw new UnknownOptionException(args[i]);
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

            return string.Format("  {0}--{1}{2}\t\t\t{3}\n",
                option.ShortName == default ? string.Empty : $"-{option.ShortName}, ",
                option.LongName,
                parametersStringBuilder,
                option.Description);
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

                    // Cast the arguments specified in `string[] args` so
                    // it can be passed into the method when invoking it.
                    for (int j = 0; j < methodParameters.Length; j++)
                    {
                        i++;
                        if (i >= args.Length)
                            throw new MissingParametersException(args[i - 1 - j], j, methodParameters.Length);

                        objectsForParameters.Add(
                            Convert.ChangeType(args[i], methodParameters[j].ParameterType));
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
                    if (i >= args.Length)
                        throw new MissingParametersException(args[i - 1], 0, 1);

                    property.SetValue(this, Convert.ChangeType(args[i], property.PropertyType));
                    return true;
                }
            }

            return false;
        }
    }
}
