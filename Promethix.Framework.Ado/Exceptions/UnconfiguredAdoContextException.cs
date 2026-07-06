using System;

namespace Promethix.Framework.Ado.Exceptions
{
    public class UnconfiguredAdoContextException : AdoScopeException
    {
        public UnconfiguredAdoContextException(Type contextType)
            : base($"""
            AdoContext `{contextType?.Name ?? "UnknownContext"}` is not configured.

            Register the context in `IAdoContextOptionsRegistry` or configure it in the AdoContext constructor before using it within an AdoScope.
            """)
        {
            if (contextType == null)
            {
                throw new ArgumentNullException(nameof(contextType));
            }
        }

        public UnconfiguredAdoContextException()
        {
        }

        public UnconfiguredAdoContextException(string message) : base(message)
        {
        }

        public UnconfiguredAdoContextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
