using System;
using System.Collections.Generic;

namespace MicroServiceFabric.CodeContracts
{
    public static class Contract
    {
        public static void RequiresNotNull<T>(T value, string paramName)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
