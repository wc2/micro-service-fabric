using MicroServiceFabric.Bootstrap.StatelessServices.Owin;

namespace $rootnamespace$
{
    internal static class Program
    {
        private static void Main()
        {
            Bootstrap<StatelessServiceModule>.Start(YOUR-SERVICE-TYPE-NAME);
        }
    }
}