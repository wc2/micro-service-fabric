using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data.Notifications;
using MicroServiceFabric.CodeContracts;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public sealed class LazyReliableDictionary<TKey, TValue> : IReliableDictionary<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        private readonly Lazy<IReliableDictionary<TKey, TValue>> _lazyReliableDictionary;

        public LazyReliableDictionary(Func<IReliableDictionary<TKey, TValue>> dictionaryFactory)
        {
            Requires.IsNotNull(dictionaryFactory, nameof(dictionaryFactory));

            _lazyReliableDictionary = new Lazy<IReliableDictionary<TKey, TValue>>(dictionaryFactory);
        }

        Uri IReliableState.Name
            => _lazyReliableDictionary.Value.Name;

        Task<long> IReliableCollection<KeyValuePair<TKey, TValue>>.GetCountAsync(ITransaction tx)
            => _lazyReliableDictionary.Value.GetCountAsync(tx);

        Task IReliableCollection<KeyValuePair<TKey, TValue>>.ClearAsync()
            => _lazyReliableDictionary.Value.ClearAsync();

        Task IReliableDictionary<TKey, TValue>.AddAsync(ITransaction tx, TKey key, TValue value)
            => _lazyReliableDictionary.Value.AddAsync(tx, key, value);

        Task IReliableDictionary<TKey, TValue>.AddAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.AddAsync(tx, key, value, timeout, cancellationToken);

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key,
                Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
            => _lazyReliableDictionary.Value.AddOrUpdateAsync(tx, key, addValueFactory, updateValueFactory);

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, TValue addValue,
                Func<TKey, TValue, TValue> updateValueFactory)
            => _lazyReliableDictionary.Value.AddOrUpdateAsync(tx, key, addValue, updateValueFactory);

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key,
                Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, TimeSpan timeout,
                CancellationToken cancellationToken)
            =>
            _lazyReliableDictionary.Value.AddOrUpdateAsync(tx, key, addValueFactory, updateValueFactory, timeout,
                cancellationToken);

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, TValue addValue,
                Func<TKey, TValue, TValue> updateValueFactory, TimeSpan timeout,
                CancellationToken cancellationToken)
            =>
            _lazyReliableDictionary.Value.AddOrUpdateAsync(tx, key, addValue, updateValueFactory, timeout,
                cancellationToken);

        Task IReliableDictionary<TKey, TValue>.ClearAsync(TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.ClearAsync(timeout, cancellationToken);

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key)
            => _lazyReliableDictionary.Value.ContainsKeyAsync(tx, key);

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, LockMode lockMode)
            => _lazyReliableDictionary.Value.ContainsKeyAsync(tx, key, lockMode);

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.ContainsKeyAsync(tx, key, timeout, cancellationToken);

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, LockMode lockMode,
                TimeSpan timeout,
                CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.ContainsKeyAsync(tx, key, lockMode, timeout, cancellationToken);

        Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(
            ITransaction txn) => _lazyReliableDictionary.Value.CreateEnumerableAsync(txn);

        Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(
                ITransaction txn, EnumerationMode enumerationMode) =>
            _lazyReliableDictionary.Value.CreateEnumerableAsync(txn, enumerationMode);

        Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(
                ITransaction txn, Func<TKey, bool> filter, EnumerationMode enumerationMode)
            => _lazyReliableDictionary.Value.CreateEnumerableAsync(txn, filter, enumerationMode);

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key,
            Func<TKey, TValue> valueFactory) => _lazyReliableDictionary.Value.GetOrAddAsync(tx, key, valueFactory);

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, TValue value)
            => _lazyReliableDictionary.Value.GetOrAddAsync(tx, key, value);

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key,
                Func<TKey, TValue> valueFactory, TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.GetOrAddAsync(tx, key, valueFactory, timeout, cancellationToken);

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, TValue value,
                TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.GetOrAddAsync(tx, key, value, timeout, cancellationToken);

        Task<bool> IReliableDictionary<TKey, TValue>.TryAddAsync(ITransaction tx, TKey key, TValue value)
            => _lazyReliableDictionary.Value.TryAddAsync(tx, key, value);

        Task<bool> IReliableDictionary<TKey, TValue>.TryAddAsync(ITransaction tx, TKey key, TValue value,
                TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.TryAddAsync(tx, key, value, timeout, cancellationToken);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key)
            => _lazyReliableDictionary.Value.TryGetValueAsync(tx, key);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key,
            LockMode lockMode) => _lazyReliableDictionary.Value.TryGetValueAsync(tx, key, lockMode);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key,
                TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.TryGetValueAsync(tx, key, timeout, cancellationToken);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key,
                LockMode lockMode, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.TryGetValueAsync(tx, key, lockMode, timeout, cancellationToken);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryRemoveAsync(ITransaction tx, TKey key)
            => _lazyReliableDictionary.Value.TryRemoveAsync(tx, key);

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryRemoveAsync(ITransaction tx, TKey key,
                TimeSpan timeout, CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.TryRemoveAsync(tx, key, timeout, cancellationToken);

        Task<bool> IReliableDictionary<TKey, TValue>.TryUpdateAsync(ITransaction tx, TKey key, TValue newValue,
                TValue comparisonValue)
            => _lazyReliableDictionary.Value.TryUpdateAsync(tx, key, newValue, comparisonValue);

        Task<bool> IReliableDictionary<TKey, TValue>.TryUpdateAsync(ITransaction tx, TKey key, TValue newValue,
                TValue comparisonValue, TimeSpan timeout,
                CancellationToken cancellationToken)
            =>
            _lazyReliableDictionary.Value.TryUpdateAsync(tx, key, newValue, comparisonValue, timeout, cancellationToken);

        Task IReliableDictionary<TKey, TValue>.SetAsync(ITransaction tx, TKey key, TValue value)
            => _lazyReliableDictionary.Value.SetAsync(tx, key, value);

        Task IReliableDictionary<TKey, TValue>.SetAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout,
                CancellationToken cancellationToken)
            => _lazyReliableDictionary.Value.SetAsync(tx, key, value, timeout, cancellationToken);

        Func<IReliableDictionary<TKey, TValue>, NotifyDictionaryRebuildEventArgs<TKey, TValue>, Task>
            IReliableDictionary<TKey, TValue>.RebuildNotificationAsyncCallback
        {
            set { _lazyReliableDictionary.Value.RebuildNotificationAsyncCallback = value; }
        }

        event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> IReliableDictionary<TKey, TValue>.
            DictionaryChanged
        {
            add { _lazyReliableDictionary.Value.DictionaryChanged += value; }
            remove { _lazyReliableDictionary.Value.DictionaryChanged -= value; }
        }
    }
}