using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;
using SimpleInjector;
using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.StatelessServices
{
    public static class Bootstrap<TStatelessServiceModule> where TStatelessServiceModule : Module, new()
    {
        public static void Start<TService>() where TService : StatelessService
        {
            ServiceRuntime.RegisterServiceAsync(GetServiceTypeName<TService>(), CreateService<TService>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static TService CreateService<TService>(StatelessServiceContext context) where TService : StatelessService
        {
            TService service;
            IStatelessServiceEventSource eventSource = null;

            try
            {
                var container = ConfigureContainer(context);

                eventSource = container.GetInstance<IStatelessServiceEventSource>();
                eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, GetServiceName<TService>());
                service = container.GetInstance<TService>();
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

        private static Container ConfigureContainer(StatelessServiceContext context)
        {
            var container = new Container();

            container.Register(() => context);
            container.RegisterModule<TStatelessServiceModule>();
            container.Verify();

            return container;
        }
    }
}
