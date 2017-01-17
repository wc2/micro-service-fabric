using MicroServiceFabric.Bootstrap.Actors;

namespace $rootnamespace$
{
    internal static class Program
    {
        private static void Main()
        {
            Bootstrap<ServiceFabricHostModule>.Start<YOUR_ACTOR_HERE>();
        }
    }
}