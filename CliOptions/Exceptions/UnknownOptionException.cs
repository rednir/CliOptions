using System;

namespace CliOptions.Exceptions
{
    public class UnknownOptionException : Exception
    {
        public UnknownOptionException()
        {
        }

        public UnknownOptionException(string arg)
            : base($"Unknown option '{arg}'")
        {
        }

        public UnknownOptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}