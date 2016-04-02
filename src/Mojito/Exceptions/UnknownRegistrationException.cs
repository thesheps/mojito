using System;

namespace Mojito.Exceptions
{
    public class UnknownRegistrationException : Exception
    {
        public UnknownRegistrationException(Type type)
            : base(string.Format(Resources.Errors.UnknownRegistration, type.FullName))
        {
        }
    }
}