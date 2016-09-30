using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using NSubstitute;
using Xunit;

namespace MicroServiceFabric.Dispatcher.Tests
{
    public sealed class ReliableDispatcherTests
    {
        [Fact]
        public void ctor_ReliableQueueRequired()
        {
            Assert.Throws<ArgumentNullException>(
                "reliableQueue",
                () =>
                    new ReliableDispatcher<object>(
                        null,
                        Substitute.For<ITransactionFactory>()));
        }

        [Fact]
        public void ctor_TransactionFactoryRequired()
        {
            Assert.Throws<ArgumentNullException>(
                "transactionFactory",
                () =>
                    new ReliableDispatcher<object>(
                        Substitute.For<Lazy<IReliableQueue<object>>>(),
                        null));
        }

        [Fact]
        public async Task EnqueueAsync_ItemRequired()
        {
            var reliableDispatcher = CreateReliableDispatcher();

            await Assert.ThrowsAsync<ArgumentNullException>(
                "item",
                () => reliableDispatcher.EnqueueAsync(null));
        }

        public void EnqueueAsync_EnqueuesItemOnReliableQueue() { }

        public void EnqueueAsync_CommitsTransactionAfterEnqueuing() { }

        private static IReliableDispatcher<object> CreateReliableDispatcher()
        {
            return new ReliableDispatcher<object>(
                Substitute.For<Lazy<IReliableQueue<object>>>(),
                Substitute.For<ITransactionFactory>());
        }
    }
}
