using MicroServiceFabric.Bootstrap;
using SimpleInjector;
using SimpleInjector.Modules;

namespace $rootnamespace$
{
    internal sealed class ServiceFabricHostModule : IModule
    {
        void IModule.Load(Container container)
        {
            container.Register<IServiceEventSource, ServiceEventSource>(Lifestyle.Singleton);

            // TODO: Register any service dependencies that your service might need here.
        }
    }
}
