using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace MicroServiceFabric.Dispatcher
{
    public sealed class ReliableDispatcher<T> : IReliableDispatcher<T>
    {
        private readonly Lazy<IReliableQueue<T>> _reliableQueue;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly ITransactionFactory _transactionFactory;

        public ReliableDispatcher(Lazy<IReliableQueue<T>> reliableQueue, ITransactionFactory transactionFactory)
        {
            Contract.RequiresNotNull(reliableQueue, nameof(reliableQueue));
            Contract.RequiresNotNull(transactionFactory, nameof(transactionFactory));

            _reliableQueue = reliableQueue;
            _transactionFactory = transactionFactory;
        }

        void IDisposable.Dispose()
        {
            StopDispatcher();
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
            cancellationToken.Register(StopDispatcher, false);

            return StartDispatcherAsync(dispatcherTask);
        }

        private async Task StartDispatcherAsync(DispatcherTask<T> dispatcherTask)
        {
            while (true)
            {
                await MessageIsEnqueuedAsync().ConfigureAwait(false);
                _tokenSource.Token.ThrowIfCancellationRequested();

                using (var transaction = _transactionFactory.Create())
                {
                    var item = await GetNextItem(transaction).ConfigureAwait(false);

                    await dispatcherTask(transaction, item).ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false); 
                }
            }
        }

        private async Task MessageIsEnqueuedAsync()
        {
            bool isQueueEmpty;

            using (var transaction = _transactionFactory.Create())
            {
                isQueueEmpty = await _reliableQueue.Value.GetCountAsync(transaction).ConfigureAwait(false) == 0;

                await transaction.CommitAsync().ConfigureAwait(false);
            }

            if (isQueueEmpty)
            {
                await Task.Delay(-1, _tokenSource.Token).ConfigureAwait(false);
            }
        }

        private async Task<T> GetNextItem(ITransaction transaction)
        {
            return
                (await
                    _reliableQueue.Value.TryDequeueAsync(transaction, default(TimeSpan), _tokenSource.Token)
                        .ConfigureAwait(false)).Value;
        }

        private void StopDispatcher()
        {
            _tokenSource.Cancel();
        }
    }
}