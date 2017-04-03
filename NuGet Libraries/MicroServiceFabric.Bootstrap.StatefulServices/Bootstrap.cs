using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using SimpleInjector;
using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.StatefulServices
{
    public static class Bootstrap<TServiceFabricHostModule> where TServiceFabricHostModule : IModule, new()
    {
        public static void Start<TService>(string serviceTypeName = null) where TService : StatefulService
        {
            ServiceRuntime.RegisterServiceAsync(serviceTypeName ?? Naming.GetServiceTypeName<TService>(), CreateService<TService>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static TService CreateService<TService>(StatefulServiceContext context) where TService : StatefulService
        {
            TService service;
            IServiceEventSource eventSource = null;

            try
            {
                var container = GetContainer(context);

                eventSource = container.GetInstance<IServiceEventSource>();
                eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, Naming.GetServiceName<TService>());
                service = container.GetInstance<TService>();
            }
            catch (Exception e)
            {
                eventSource?.ServiceHostInitializationFailed(e.ToString());
                throw;
            }

            return service;
        }

        private static Container GetContainer(StatefulServiceContext context)
        {
            var container = new Container();
            var stateManager = new Lazy<ReliableStateManager>(() => new ReliableStateManager(context));

            container.RegisterModule<TServiceFabricHostModule>();
            container.Register<IReliableStateManagerReplica>(() => stateManager.Value, Lifestyle.Singleton);
            container.Register<IReliableStateManager>(() => stateManager.Value, Lifestyle.Singleton);
            container.Register<IGetSettings, GetSettings>(Lifestyle.Singleton);
            container.Register(() => context, Lifestyle.Singleton);
            container.Register(() => (ServiceContext)context, Lifestyle.Singleton);

            return container;
        }
    }
}
