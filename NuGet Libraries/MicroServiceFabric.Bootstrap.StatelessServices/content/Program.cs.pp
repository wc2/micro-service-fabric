using MicroServiceFabric.Bootstrap.StatelessServices;

namespace $rootnamespace$
{
    internal static class Program
    {
        private static void Main()
        {
            Bootstrap<StatelessServiceModule>.Start<YOUR_SERVICE_HERE>();
        }
    }
}