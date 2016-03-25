using System;

namespace Mojito.Exceptions
{
    public class UnknownRegistrationException : Exception
    {
        public UnknownRegistrationException(string type)
            : base(string.Format(Resources.Errors.UnknownRegistration, type))
        {
        }
    }
}