using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections.Preview;
using MicroServiceFabric.CodeContracts;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public sealed class LazyReliableConcurrentQueue<T> : IReliableConcurrentQueue<T>
    {
        private readonly Lazy<IReliableConcurrentQueue<T>> _wrappedInstance;

        public LazyReliableConcurrentQueue(Func<IReliableConcurrentQueue<T>> queueFactory)
        {
            Requires.IsNotNull(queueFactory, nameof(queueFactory));

            _wrappedInstance = new Lazy<IReliableConcurrentQueue<T>>(queueFactory);
        }

        public Uri Name => _wrappedInstance.Value.Name;

        public Task EnqueueAsync(ITransaction tx, T value, CancellationToken cancellationToken = new CancellationToken(),
                TimeSpan? timeout = null)
            => _wrappedInstance.Value.EnqueueAsync(tx, value, cancellationToken, timeout);

        public Task<T> DequeueAsync(ITransaction tx, CancellationToken cancellationToken = new CancellationToken(),
                TimeSpan? timeout = null)
            => _wrappedInstance.Value.DequeueAsync(tx, cancellationToken, timeout);

        public long Count => _wrappedInstance.Value.Count;
    }
}