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
    public static class Bootstrap<TStatefulServiceModule> where TStatefulServiceModule : Module, new()
    {
        public static void Start<TService>() where TService : StatefulService
        {
            ServiceRuntime.RegisterServiceAsync(Naming.GetServiceTypeName<TService>(), CreateService<TService>)
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
                var container = ConfigureContainer(context);

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

        private static Container ConfigureContainer(StatefulServiceContext context)
        {
            var container = new Container();
            IReliableStateManagerReplica stateManager = new ReliableStateManager(context);

            container.Register(() => context, Lifestyle.Singleton);
            container.Register(() => stateManager, Lifestyle.Singleton);
            container.RegisterModule<TStatefulServiceModule>();
            container.Verify();

            return container;
        }
    }
}
