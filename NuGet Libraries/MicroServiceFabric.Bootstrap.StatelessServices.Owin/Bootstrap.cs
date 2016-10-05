using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.StatelessServices.Owin
{
    public static class Bootstrap<TStatelessServiceModule> where TStatelessServiceModule : Module, new()
    {
        public static void Start(string serviceTypeName)
        {
            StatelessServices.Bootstrap<TStatelessServiceModule>.Start<Owin>(serviceTypeName);
        }
    }
}
