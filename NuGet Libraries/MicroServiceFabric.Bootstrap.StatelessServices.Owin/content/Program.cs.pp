using MicroServiceFabric.Bootstrap.StatelessServices.Owin;

namespace $rootnamespace$
{
    internal static class Program
    {
        private static void Main()
        {
            Bootstrap<ServiceFabricHostModule>.Start(YOUR-SERVICE-TYPE-NAME);
        }
    }
}