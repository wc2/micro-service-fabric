using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.StatelessServices.Owin
{
    public static class Bootstrap<TServiceFabricHostModule> where TServiceFabricHostModule : Module, new()
    {
        public static void Start(string serviceTypeName)
        {
            StatelessServices.Bootstrap<TServiceFabricHostModule>.Start<Owin>(serviceTypeName);
        }
    }
}
