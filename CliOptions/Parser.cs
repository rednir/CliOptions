using System;
using System.Linq;
using System.Reflection;

namespace CliOptions
{
    public class Parser<T>
    {
        private MethodInfo[] OptionMethods { get; }

        public Parser()
        {
            OptionMethods = typeof(T)
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OptionAttribute), false).Length > 0)
                .ToArray();
        }

        public void Parse(string[] args)
        {
            foreach (MethodInfo method in OptionMethods)
            {
                var attribute = (OptionAttribute)method.GetCustomAttribute(typeof(OptionAttribute));
                if (args[0] == "--" + attribute.LongName || args[0] == "-" + attribute.ShortName)
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
                    action.Invoke();
                }
            }
        }
    }
}
