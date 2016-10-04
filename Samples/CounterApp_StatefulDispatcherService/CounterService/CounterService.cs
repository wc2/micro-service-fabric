using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CounterService.Api;
using MicroServiceFabric.Dispatcher;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CounterService
{
    internal sealed class CounterService : StatefulDispatcherService<int>, IIncrementCounter, IQueryCounter
    {
        public CounterService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica, IReliableDispatcher<int> reliableDispatcher)
            : base(serviceContext, reliableStateManagerReplica, reliableDispatcher)
        {
        }

        Task IIncrementCounter.IncrementCounterAsync(int incrementBy)
        {
            return ReliableDispatcher.EnqueueAsync(incrementBy);
        }

        async Task<int> IQueryCounter.GetCounterAsync()
        {
            var counters =
                await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Counters").ConfigureAwait(false);
            int counter;

            using (var transaction = StateManager.CreateTransaction())
            {
                var counterValue = await counters.TryGetValueAsync(transaction, "Counter").ConfigureAwait(false);
                counter = counterValue.HasValue
                    ? counterValue.Value
                    : 0;
                await transaction.CommitAsync().ConfigureAwait(false);
            }

            return counter;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {new ServiceReplicaListener(this.CreateServiceRemotingListener)};
        }

        public override async Task OnItemDispatchedAsync(ITransaction transaction, int incrementCounterBy,
            CancellationToken cancellationToken)
        {
            var counters =
                await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Counters").ConfigureAwait(false);
            await
                counters.AddOrUpdateAsync(transaction, "Counter", incrementCounterBy, (key, value) => value + incrementCounterBy)
                    .ConfigureAwait(false);
            // Note: no need to commit the transaction as this will happen automatically in the ReliableDispatcher.
        }
    }
}