using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SimpleInjector;
using SimpleInjector.Modules;

namespace MicroServiceFabric.Bootstrap.Actors
{
    public static class Bootstrap<TServiceFabricHostModule> where TServiceFabricHostModule : IModule, new()
    {
        public static void Start<TActor>()
            where TActor : Actor
        {
            Start<TActor, ActorService>();
        }

        public static void Start<TActor, TActorService>()
            where TActor : Actor
            where TActorService : ActorService
        {
            ActorRuntime.RegisterActorAsync<TActor>(CreateActorService<TActor, TActorService>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static ActorService CreateActorService<TActor, TActorService>(
            StatefulServiceContext context,
            ActorTypeInformation actorType)
            where TActor : Actor
            where TActorService : ActorService
        {
            using (var serviceContainer = GetServiceContainer<TActor>(context, actorType))
            {
                return serviceContainer.GetInstance<TActorService>();
            }
        }

        private static TActor CreateActor<TActor>(
            ActorService service,
            ActorId id)
            where TActor : Actor
        {
            TActor actor;
            IServiceEventSource eventSource = null;

            try
            {
                var actorContainer = GetActorContainer(service, id);

                eventSource = actorContainer.GetInstance<IServiceEventSource>();
                eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, Naming.GetServiceName<TActor>());

                actor = actorContainer.GetInstance<TActor>();

                var stateManager = (LazyActorStateManager) actorContainer.GetInstance<IActorStateManager>();
                stateManager.SetActorStateManager(actor.StateManager);
            }
            catch (Exception e)
            {
                eventSource?.ServiceHostInitializationFailed(e.ToString());
                throw;
            }

            return actor;
        }

        private static Container GetServiceContainer<TActor>(
            StatefulServiceContext context,
            ActorTypeInformation actorType)
            where TActor : Actor
        {
            var container = GetContainer(context);

            container.Register(() => actorType);
            container.Register<Func<ActorService, ActorId, TActor>>(() => CreateActor<TActor>);

            return container;
        }

        private static Container GetActorContainer(
            ActorService service,
            ActorId id)
        {
            var context = service.Context;
            var container = GetContainer(context);

            container.Register(() => service.StateProvider);
            container.Register(() => service);
            container.Register(() => id);

            return container;
        }

        private static Container GetContainer(
            StatefulServiceContext context)
        {
            var container = new Container();

            container.RegisterModule<TServiceFabricHostModule>();
            container.Register<IActorStateManager, LazyActorStateManager>(Lifestyle.Singleton);
            container.Register<IGetSettings, GetSettings>(Lifestyle.Singleton);
            container.Register(() => context, Lifestyle.Singleton);
            container.Register(() => (ServiceContext) context, Lifestyle.Singleton);

            return container;
        }
    }
}