using System;
using Mojito.Resources;

namespace Mojito.Exceptions
{
    public class DuplicateRegistrationException : Exception
    {
        public DuplicateRegistrationException(string baseType)
            : base(string.Format(Errors.DuplicateRegistration, baseType))
        {
        }
    }
}