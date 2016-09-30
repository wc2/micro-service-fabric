using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
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

        [Fact]
        public async Task EnqueueAsync_EnqueuesItemOnReliableQueue()
        {
            var reliableQueue = Substitute.For<IReliableQueue<object>>();
            var transaction = Substitute.For<ITransaction>();
            var item = Substitute.For<object>();
            var reliableDispatcher = CreateReliableDispatcher(reliableQueue, CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(item);

            await reliableQueue
                .Received()
                .EnqueueAsync(transaction, item);
        }

        [Fact]
        public async Task EnqueueAsync_CommitsTransactionAfterEnqueuing()
        {
            var transaction = Substitute.For<ITransaction>();
            var reliableDispatcher = CreateReliableDispatcher(transactionFactory: CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(Substitute.For<object>());

            await transaction
                .Received()
                .CommitAsync();
        }

        [Fact]
        public async Task EnqueueAsync_DisposesTransaction()
        {
            var transaction = Substitute.For<ITransaction>();
            var reliableDispatcher = CreateReliableDispatcher(transactionFactory: CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(Substitute.For<object>());

            transaction
                .Received()
                .Dispose();
        }

        [Fact]
        public async Task RunAsync_DispatcherTask()
        {
            var reliableDispatcher = CreateReliableDispatcher();

            await Assert.ThrowsAsync<ArgumentNullException>(
                "dispatcherTask",
                () => reliableDispatcher.RunAsync(null, default(CancellationToken)));
        }

        [Fact]
        public async Task RunAsync_CancelsWhenCancellationTokensIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            var reliableDispatcher = CreateReliableDispatcher();
            var task = Task.Factory.StartNew(
                () => reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), tokenSource.Token),
                default(CancellationToken));

            await Assert.ThrowsAsync<OperationCanceledException>(
                () =>
                {
                    tokenSource.Cancel();
                    return task;
                });
        }

        private static IReliableDispatcher<object> CreateReliableDispatcher(IReliableQueue<object> reliableQueue = null,
            ITransactionFactory transactionFactory = null)
        {
            return new ReliableDispatcher<object>(
                new Lazy<IReliableQueue<object>>(() => reliableQueue ?? Substitute.For<IReliableQueue<object>>()),
                transactionFactory ?? Substitute.For<ITransactionFactory>());
        }

        private static ITransactionFactory CreateTransactionFactory(ITransaction transaction)
        {
            var transactionFactory = Substitute.For<ITransactionFactory>();

            transactionFactory
                .Create()
                .Returns(transaction);

            return transactionFactory;
        }
    }
}
