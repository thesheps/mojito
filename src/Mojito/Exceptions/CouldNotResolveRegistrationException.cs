using System;

namespace Mojito.Exceptions
{
    public class CouldNotResolveRegistrationException : Exception
    {
        public CouldNotResolveRegistrationException(Type type)
            : base(string.Format(Resources.Errors.CouldNotResolveRegistration, type.FullName))
        {
        }
    }
}