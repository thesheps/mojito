using System;
using Mojito.Resources;

namespace Mojito.Exceptions
{
    public class DuplicateRegistrationException : Exception
    {
        public DuplicateRegistrationException(Type type)
            : base(string.Format(Errors.DuplicateRegistration, type.FullName))
        {
        }
    }
}