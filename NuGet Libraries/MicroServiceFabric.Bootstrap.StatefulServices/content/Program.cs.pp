using MicroServiceFabric.Bootstrap.StatefulServices;

namespace $rootnamespace$
{
    internal static class Program
    {
        private static void Main()
        {
            Bootstrap<StatefulServiceModule>.Start<YOUR_SERVICE_HERE>();
        }
    }
}