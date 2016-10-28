using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;
using SimpleInjector;
using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.StatelessServices
{
    public static class Bootstrap<TStatelessServiceModule> where TStatelessServiceModule : IModule, new()
    {
        public static void Start<TService>(string serviceTypeName = null) where TService : StatelessService
        {
            ServiceRuntime.RegisterServiceAsync(serviceTypeName ?? Naming.GetServiceTypeName<TService>(), CreateService<TService>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static TService CreateService<TService>(StatelessServiceContext context) where TService : StatelessService
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

        private static Container ConfigureContainer(StatelessServiceContext context)
        {
            var container = new Container();

            container.RegisterModule<TStatelessServiceModule>();
            container.Register<IGetStatelessContext, GetStatelessContext>(Lifestyle.Singleton);
            container.Register<IGetSettings, GetSettings>(Lifestyle.Singleton);
            container.Register(() => context, Lifestyle.Singleton);
            container.Register(() => (ServiceContext)context, Lifestyle.Singleton);

            return container;
        }
    }
}
