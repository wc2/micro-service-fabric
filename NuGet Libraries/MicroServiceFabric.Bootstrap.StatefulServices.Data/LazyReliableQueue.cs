using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public sealed class LazyReliableQueue<T> : IReliableQueue<T>
    {
        private readonly Lazy<IReliableQueue<T>> _wrappedInstance;

        public LazyReliableQueue(Lazy<IReliableQueue<T>> wrappedInstance)
        {
            Requires.IsNotNull(wrappedInstance, nameof(wrappedInstance));

            _wrappedInstance = wrappedInstance;
        }

        public Uri Name => _wrappedInstance.Value.Name;

        public Task<long> GetCountAsync(ITransaction tx) => _wrappedInstance.Value.GetCountAsync(tx);

        public Task ClearAsync() => _wrappedInstance.Value.ClearAsync();

        public Task EnqueueAsync(ITransaction tx, T item) => _wrappedInstance.Value.EnqueueAsync(tx, item);

        public Task EnqueueAsync(ITransaction tx, T item, TimeSpan timeout, CancellationToken cancellationToken)
            => _wrappedInstance.Value.EnqueueAsync(tx, item, timeout, cancellationToken);

        public Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx)
            => _wrappedInstance.Value.TryDequeueAsync(tx);

        public Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _wrappedInstance.Value.TryDequeueAsync(tx, timeout, cancellationToken);

        public Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx) => _wrappedInstance.Value.TryPeekAsync(tx);

        public Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _wrappedInstance.Value.TryPeekAsync(tx, timeout, cancellationToken);

        public Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, LockMode lockMode)
            => _wrappedInstance.Value.TryPeekAsync(tx, lockMode);

        public Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, LockMode lockMode, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _wrappedInstance.Value.TryPeekAsync(tx, lockMode, timeout, cancellationToken);

        public Task<IAsyncEnumerable<T>> CreateEnumerableAsync(ITransaction tx)
            => _wrappedInstance.Value.CreateEnumerableAsync(tx);
    }
}