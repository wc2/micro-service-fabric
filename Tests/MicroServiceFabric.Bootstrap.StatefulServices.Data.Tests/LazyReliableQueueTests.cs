using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data.Tests
{
    public sealed class LazyReliableQueueTests
    {

        [Fact]
        public void ctor_WrappedInstanceRequired()
        {
            Assert.Throws<ArgumentNullException>(
                "wrappedInstance",
                () => new LazyReliableQueue<object>(null));
        }

        [Theory]
        [AutoData]
        public void Name_ReturnsWrappedInstanceName(Uri expectedName)
        {
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .Name
                .Returns(expectedName);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            lazyReliableQueue.Name
                .Should()
                .Be(wrappedInstance.Name);
        }

        [Theory]
        [AutoData]
        public async Task GetCountAsync_ReturnsWrappedInstanceGetCount(long expectedCount)
        {
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .GetCountAsync(transaction)
                .Returns(expectedCount);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);
            var count = await lazyReliableQueue.GetCountAsync(transaction).ConfigureAwait(false);

            count
                .Should()
                .Be(count);
        }

        [Fact]
        public async Task ClearAsync_ClearsWrappedInstance()
        {
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            await lazyReliableQueue.ClearAsync().ConfigureAwait(false);

            await wrappedInstance
                .Received()
                .ClearAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task EnqueueAsync_Transaction_Item_EnqueuesOnWrappedInstance()
        {
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);
            var item = Substitute.For<object>();

            await lazyReliableQueue.EnqueueAsync(transaction, item).ConfigureAwait(false);

            await wrappedInstance
                .Received()
                .EnqueueAsync(transaction, item).ConfigureAwait(false);
        }

        [Theory]
        [AutoData]
        public async Task EnqueueAsync_Transaction_Item_Timeout_CancellationToken_EnqueuesOnWrappedInstance(
            TimeSpan timeout, CancellationToken cancellationToken)
        {
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);
            var item = Substitute.For<object>();

            await lazyReliableQueue.EnqueueAsync(transaction, item, timeout, cancellationToken).ConfigureAwait(false);

            await wrappedInstance
                .Received()
                .EnqueueAsync(transaction, item, timeout, cancellationToken).ConfigureAwait(false);
        }

        [Fact]
        public async Task TryDequeueAsync_Transaction_TriesDequeuesFromWrappedInstance()
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryDequeueAsync(transaction)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response = await lazyReliableQueue.TryDequeueAsync(transaction).ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Theory]
        [AutoData]
        public async Task TryDequeueAsync_Transaction_Timeout_CancellationToken_TriesDequeueFromWrappedInstance(
            TimeSpan timeout, CancellationToken cancellationToken)
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryDequeueAsync(transaction, timeout, cancellationToken)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response =
                await lazyReliableQueue.TryDequeueAsync(transaction, timeout, cancellationToken).ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Fact]
        public async Task TryPeekAsync_Transaction_TriesPeekOnWrappedInstance()
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryPeekAsync(transaction)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response = await lazyReliableQueue.TryPeekAsync(transaction).ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Theory]
        [AutoData]
        public async Task TryPeekAsync_Transaction_Timeout_CancellationToken_TriesPeekOnWrappedInstance(
            TimeSpan timeout, CancellationToken cancellationToken)
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryPeekAsync(transaction, timeout, cancellationToken)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response =
                await lazyReliableQueue.TryPeekAsync(transaction, timeout, cancellationToken).ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Theory]
        [InlineData(LockMode.Default)]
        [InlineData(LockMode.Update)]
        public async Task TryPeekAsync_Transaction_LockMode_TriesPeekOnWrappedInstance(LockMode lockMode)
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryPeekAsync(transaction, lockMode)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response = await lazyReliableQueue.TryPeekAsync(transaction, lockMode).ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Theory]
        [AutoData]
        public async Task TryPeekAsync_Transaction_LockMode_Timeout_CancellationToken_TriesPeekOnWrappedInstance(
            LockMode lockMode, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var expectedValue = Substitute.For<object>();
            var expectedResponse = new ConditionalValue<object>(true, expectedValue);
            var transaction = Substitute.For<ITransaction>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .TryPeekAsync(transaction, lockMode, timeout, cancellationToken)
                .Returns(expectedResponse);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var response =
                await
                    lazyReliableQueue.TryPeekAsync(transaction, lockMode, timeout, cancellationToken)
                        .ConfigureAwait(false);

            response.Value
                .Should()
                .Be(expectedValue);
        }

        [Fact]
        public async Task CreateEnumerableAsync_ReturnsCreateEnumerableOnWrappedInstance()
        {
            var transaction = Substitute.For<ITransaction>();
            var expectedEnumerable = Substitute.For<IAsyncEnumerable<object>>();
            var wrappedInstance = Substitute.For<IReliableQueue<object>>();
            wrappedInstance
                .CreateEnumerableAsync(transaction)
                .Returns(expectedEnumerable);

            var lazyReliableQueue = CreateLazyReliableQueue(wrappedInstance);

            var enumerable = await lazyReliableQueue.CreateEnumerableAsync(transaction).ConfigureAwait(false);

            enumerable
                .Should()
                .Be(expectedEnumerable);
        }

        private static IReliableQueue<T> CreateLazyReliableQueue<T>(IReliableQueue<T> wrappedInstance)
        {
            return new LazyReliableQueue<T>(() => wrappedInstance);
        }
    }
}