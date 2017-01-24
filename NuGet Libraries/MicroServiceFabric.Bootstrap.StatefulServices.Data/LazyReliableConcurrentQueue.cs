using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections.Preview;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public sealed class LazyReliableConcurrentQueue<T> : IReliableConcurrentQueue<T>
    {
        private readonly Lazy<IReliableConcurrentQueue<T>> _wrappedInstance;

        public LazyReliableConcurrentQueue(Lazy<IReliableConcurrentQueue<T>> wrappedInstance)
        {
            _wrappedInstance = wrappedInstance;
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