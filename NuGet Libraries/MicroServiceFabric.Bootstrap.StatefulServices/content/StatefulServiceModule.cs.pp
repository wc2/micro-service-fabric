using MicroServiceFabric.Bootstrap.StatefulServices;
using SimpleInjector;
using SimpleInjector.Modules;

namespace $rootnamespace$
{
    internal sealed class StatefulServiceModule : Module
    {
        public override void Load(Container container)
        {
            container.Register<IStatefulServiceEventSource, ServiceEventSource>(Lifestyle.Singleton);

            // TODO: Register any service dependencies that your service might need here.
        }
    }
}
