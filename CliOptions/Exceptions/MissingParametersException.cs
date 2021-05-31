using System;

namespace CliOptions.Exceptions
{
    public class MissingParametersException : Exception
    {
        public MissingParametersException()
        {
        }

        public MissingParametersException(string message) : base(message)
        {
        }

        public MissingParametersException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MissingParametersException(string arg, int parametersGiven, int parametersNeeded)
            : base($"Missing parameters for argument '{arg}'. Only {parametersGiven} out of {parametersNeeded} parameter(s) were given.")
        {
        }
    }
}