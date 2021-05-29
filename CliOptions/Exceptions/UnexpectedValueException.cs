using System;

namespace CliOptions
{
    public class UnexpectedValueException : Exception
    {
        public UnexpectedValueException()
        {
        }

        public UnexpectedValueException(string message) : base(message)
        {
        }

        public UnexpectedValueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}