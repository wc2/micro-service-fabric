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
        public static void Start<TActor>() where TActor : Actor
        {
            ActorRuntime.RegisterActorAsync<TActor>(CreateActorService<TActor>)
                .GetAwaiter()
                .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        private static ActorService CreateActorService<TActor>(StatefulServiceContext context,
            ActorTypeInformation actorType) where TActor : Actor
        {
            return new ActorService(context, actorType, CreateActor<TActor>);
        }

        private static TActor CreateActor<TActor>(ActorService service, ActorId id) where TActor : Actor
        {
            TActor actor;
            IServiceEventSource eventSource = null;

            try
            {
                var container = ConfigureContainer(service, id);

                eventSource = container.GetInstance<IServiceEventSource>();
                eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, Naming.GetServiceName<TActor>());

                actor = container.GetInstance<TActor>();

                var stateManager = (LazyActorStateManager) container.GetInstance<IActorStateManager>();
                stateManager.SetActorStateManager(actor.StateManager);
            }
            catch (Exception e)
            {
                eventSource?.ServiceHostInitializationFailed(e.ToString());
                throw;
            }

            return actor;
        }

        private static Container ConfigureContainer(ActorService service, ActorId id)
        {
            var container = new Container();

            container.RegisterModule<TServiceFabricHostModule>();
            container.Register<IActorStateManager, LazyActorStateManager>(Lifestyle.Singleton);
            container.Register(() => service.StateProvider);
            container.Register(() => service);
            container.Register(() => id);

            return container;
        }
    }
}
