using System;
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
using Microsoft.ServiceFabric.Services.Runtime;

namespace CounterService
{
    internal sealed class CounterService : StatefulService, IIncrementCounter, IQueryCounter
    {
        private readonly IReliableDispatcher<int> _reliableDispatcher;

        public CounterService(StatefulServiceContext context)
            : base(context)
        {
            _reliableDispatcher = new ReliableDispatcher<int>(
                new Lazy<IReliableQueue<int>>(
                    () => StateManager.GetOrAddAsync<IReliableQueue<int>>("CounterIncrements").Result),
                new TransactionFactory(StateManager));
        }

        Task IIncrementCounter.IncrementCounterAsync(int incrementBy)
        {
            return _reliableDispatcher.EnqueueAsync(incrementBy);
        }

        async Task<int> IQueryCounter.GetCounterAsync()
        {
            var counters = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Counters").ConfigureAwait(false);
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

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return _reliableDispatcher.RunAsync(IncrementCounterAsync, cancellationToken);
        }

        private async Task IncrementCounterAsync(ITransaction transaction, int incrementCounterBy, CancellationToken cancellationtoken)
        {
            var counters =
                await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Counters").ConfigureAwait(false);
            await counters.AddOrUpdateAsync(transaction, "Counter", 0, (key, value) => value + incrementCounterBy).ConfigureAwait(false);
            // Note: no need to commit the transaction as this will happen automatically in the ReliableDispatcher.
        }
    }
}