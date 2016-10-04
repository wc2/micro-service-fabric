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
    public static class Bootstrap<TStatelessServiceModule> where TStatelessServiceModule : Module, new()
    {
        public static void Start<TService>() where TService : StatefulService
        {
            ServiceRuntime.RegisterServiceAsync(GetServiceTypeName<TService>(), CreateService<TService>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static TService CreateService<TService>(StatefulServiceContext context) where TService : StatefulService
        {
            TService service;
            IStatefulServiceEventSource eventSource = null;

            try
            {
                var container = ConfigureContainer(context);

                service = container.GetInstance<TService>();
                eventSource = container.GetInstance<IStatefulServiceEventSource>();
                eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, GetServiceName<TService>());
            }
            catch (Exception e)
            {
                eventSource?.ServiceHostInitializationFailed(e.ToString());
                throw;
            }

            return service;
        }

        private static string GetServiceName<TService>()
        {
            return typeof (TService).Name;
        }

        private static string GetServiceTypeName<TService>()
        {
            return $"{GetServiceName<TService>()}Type";
        }

        private static Container ConfigureContainer(StatefulServiceContext context)
        {
            var container = new Container();
            IReliableStateManagerReplica stateManager = new ReliableStateManager(context);

            container.Register(() => context);
            container.Register(() => stateManager);
            container.RegisterModule<TStatelessServiceModule>();
            container.Verify();

            return container;
        }
    }
}
