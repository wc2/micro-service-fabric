using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;

namespace MicroServiceFabric.Dispatcher
{
    public abstract class StatefulDispatcherService<T> : StatefulService, IDisposable
    {
        protected readonly IReliableDispatcher<T> ReliableDispatcher;

        protected StatefulDispatcherService(StatefulServiceContext serviceContext,
            IReliableStateManagerReplica reliableStateManagerReplica, IReliableDispatcher<T> reliableDispatcher)
            : base(serviceContext, reliableStateManagerReplica)
        {
            Contract.RequiresNotNull(reliableDispatcher, nameof(reliableDispatcher));

            ReliableDispatcher = reliableDispatcher;
        }

        void IDisposable.Dispose()
        {
            ReliableDispatcher.Dispose();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return RunDispatcherAsync(cancellationToken);
        }

        internal Task RunDispatcherAsync(CancellationToken cancellationToken)
        {
            return ReliableDispatcher.RunAsync(OnItemDispatchedAsync, cancellationToken);
        }

        public abstract Task OnItemDispatchedAsync(ITransaction transaction, T item,
            CancellationToken cancellationToken);
    }
}