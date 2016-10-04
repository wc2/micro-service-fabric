using MicroServiceFabric.Bootstrap.StatelessServices;
using SimpleInjector;
using SimpleInjector.Modules;

namespace $rootnamespace$
{
    internal sealed class StatelessServiceModule : Module
    {
        public override void Load(Container container)
        {
            container.Register<IStatelessServiceEventSource, ServiceEventSource>(Lifestyle.Singleton);

            // TODO: Register any service dependencies that your service might need here.
        }
    }
}
