using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data.Collections;

namespace MicroServiceFabric.Dispatcher
{
    public sealed class ReliableDispatcher<T> : IReliableDispatcher<T>
    {
        private readonly Lazy<IReliableQueue<T>> _reliableQueue;
        private readonly ITransactionFactory _transactionFactory;

        public ReliableDispatcher(Lazy<IReliableQueue<T>> reliableQueue, ITransactionFactory transactionFactory)
        {
            Contract.RequiresNotNull(reliableQueue, nameof(reliableQueue));
            Contract.RequiresNotNull(transactionFactory, nameof(transactionFactory));

            _reliableQueue = reliableQueue;
            _transactionFactory = transactionFactory;
        }

        async Task IReliableDispatcher<T>.EnqueueAsync(T item)
        {
            Contract.RequiresNotNull(item, nameof(item));

            using (var transaction = _transactionFactory.Create())
            {
                await _reliableQueue.Value.EnqueueAsync(transaction, item).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        Task IReliableDispatcher<T>.RunAsync(DispatcherTask<T> dispatcherTask, CancellationToken cancellationToken)
        {
            Contract.RequiresNotNull(dispatcherTask, nameof(dispatcherTask));

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
