using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Bootstrap.Actors
{
    internal sealed class LazyActorStateManager : IActorStateManager
    {
        private readonly Lazy<IActorStateManager> _lazyActorStateManager;
        private IActorStateManager _wrappedInstance;

        public LazyActorStateManager()
        {
            _lazyActorStateManager = new Lazy<IActorStateManager>(GetWrappedValue);
        }

        private IActorStateManager GetWrappedValue()
        {
            if (_wrappedInstance == null)
            {
                throw new InvalidOperationException("State manager has not been initialised.");
            }

            return _wrappedInstance;
        }

        internal void SetActorStateManager(IActorStateManager actorStateManager)
        {
            _wrappedInstance = actorStateManager;
        }

        public Task AddStateAsync<T>(string stateName, T value,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.AddStateAsync(stateName, value, cancellationToken);
        }

        public Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.GetStateAsync<T>(stateName, cancellationToken);
        }

        public Task SetStateAsync<T>(string stateName, T value,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.SetStateAsync(stateName, value, cancellationToken);
        }

        public Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.RemoveStateAsync(stateName, cancellationToken);
        }

        public Task<bool> TryAddStateAsync<T>(string stateName, T value,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.TryAddStateAsync(stateName, value, cancellationToken);
        }

        public Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.TryGetStateAsync<T>(stateName, cancellationToken);
        }

        public Task<bool> TryRemoveStateAsync(string stateName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.TryRemoveStateAsync(stateName, cancellationToken);
        }

        public Task<bool> ContainsStateAsync(string stateName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.ContainsStateAsync(stateName, cancellationToken);
        }

        public Task<T> GetOrAddStateAsync<T>(string stateName, T value,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.GetOrAddStateAsync(stateName, value, cancellationToken);
        }

        public Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.AddOrUpdateStateAsync(stateName, addValue, updateValueFactory,
                cancellationToken);
        }

        public Task<IEnumerable<string>> GetStateNamesAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.GetStateNamesAsync(cancellationToken);
        }

        public Task ClearCacheAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.ClearCacheAsync(cancellationToken);
        }

        public Task SaveStateAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _lazyActorStateManager.Value.SaveStateAsync(cancellationToken);
        }
    }
}