using System;
using System.Collections.Generic;

namespace MicroServiceFabric.CodeContracts
{
    public static class Requires
    {
        public static void IsNotNull<T>(T value, string paramName)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
