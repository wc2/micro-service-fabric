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
        private readonly IReliableDispatcher<T> _reliableDispatcher;

        protected StatefulDispatcherService(StatefulServiceContext serviceContext,
            IReliableDispatcher<T> reliableDispatcher) : base(serviceContext)
        {
            _reliableDispatcher = reliableDispatcher;
            Contract.RequiresNotNull(reliableDispatcher, nameof(reliableDispatcher));
        }

        void IDisposable.Dispose()
        {
            _reliableDispatcher.Dispose();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return RunDispatcherAsync(cancellationToken);
        }

        internal Task RunDispatcherAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected internal abstract Task OnItemDispatched(ITransaction transaction, T item,
            CancellationToken cancellationToken);
    }
}