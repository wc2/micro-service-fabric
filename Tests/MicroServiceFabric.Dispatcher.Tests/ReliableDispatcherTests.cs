using System;
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

        public void ctor_TransactionFactoryRequired() { }

        public void EnqueueAsync_ItemRequired() { }

        public void EnqueueAsync_EnqueuesItemOnReliableQueue() { }

        public void EnqueueAsync_CommitsTransactionAfterEnqueuing() { }
    }
}
