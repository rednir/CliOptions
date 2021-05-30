using System;

namespace CliOptions.Exceptions
{
    public class UnknownArgumentException : Exception
    {
        public UnknownArgumentException()
        {
        }

        public UnknownArgumentException(string arg)
            : base($"Unknown argument '{arg}'")
        {
        }

        public UnknownArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}