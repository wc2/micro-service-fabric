using System;
using System.Collections.Generic;
using System.Linq;
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
                () => reliableDispatcher.EnqueueAsync(null))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task EnqueueAsync_EnqueuesItemOnReliableQueue()
        {
            var reliableQueue = Substitute.For<IReliableQueue<object>>();
            var transaction = Substitute.For<ITransaction>();
            var item = Substitute.For<object>();
            var reliableDispatcher = CreateReliableDispatcher(reliableQueue, CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(item).ConfigureAwait(false);

            await reliableQueue
                .Received()
                .EnqueueAsync(transaction, item)
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task EnqueueAsync_CommitsTransactionAfterEnqueuing()
        {
            var transaction = Substitute.For<ITransaction>();
            var reliableDispatcher = CreateReliableDispatcher(transactionFactory: CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(Substitute.For<object>()).ConfigureAwait(false);

            await transaction
                .Received()
                .CommitAsync()
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task EnqueueAsync_DisposesTransaction()
        {
            var transaction = Substitute.For<ITransaction>();
            var reliableDispatcher = CreateReliableDispatcher(transactionFactory: CreateTransactionFactory(transaction));

            await reliableDispatcher.EnqueueAsync(Substitute.For<object>()).ConfigureAwait(false);

            transaction
                .Received()
                .Dispose();
        }

        [Fact]
        public async Task Dispose_CancelsRunAsync()
        {
            var reliableDispatcher = CreateReliableDispatcher();
            var task = reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), default(CancellationToken));

            await Assert.ThrowsAsync<TaskCanceledException>(
                () =>
                {
                    reliableDispatcher.Dispose();
                    return task;
                }).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_DispatcherTask()
        {
            var reliableDispatcher = CreateReliableDispatcher();

            await Assert.ThrowsAsync<ArgumentNullException>(
                "dispatcherTask",
                () => reliableDispatcher.RunAsync(null, default(CancellationToken)))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_CancelsWhenCancellationTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            var reliableDispatcher = CreateReliableDispatcher();
            var task = reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), tokenSource.Token);

            tokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() => task).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_DoesNotDequeueWhenQueueIsEmpty()
        {
            Task task;
            var reliableQueue = Substitute.For<IReliableQueue<object>>();

            using (var reliableDispatcher = CreateReliableDispatcher())
            {
                task = reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), CancellationToken.None);

                await reliableQueue
                    .DidNotReceive()
                    .TryDequeueAsync(Arg.Any<ITransaction>())
                    .ConfigureAwait(false); 
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task RunAsync_DispatchesItemsAlreadyInQueue(int numberOfItemsAlreadyInQueue)
        {
            Task task;
            var dispatcherTask = Substitute.For<DispatcherTask<object>>();
            var items = Enumerable.Range(1, numberOfItemsAlreadyInQueue).Select(i => Substitute.For<object>()).ToList();

            using (var reliableDispatcher = CreateReliableDispatcher(CreateReliableQueue(items)))
            {
                task = reliableDispatcher.RunAsync(dispatcherTask, CancellationToken.None);

                items
                    .ForEach(item =>
                        dispatcherTask
                            .Received()
                            .Invoke(Arg.Any<ITransaction>(), item));

                await dispatcherTask
                    .Received(numberOfItemsAlreadyInQueue)
                    .Invoke(Arg.Any<ITransaction>(), Arg.Any<object>())
                    .ConfigureAwait(false);
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);
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

        private static IReliableQueue<object> CreateReliableQueue(List<object> items)
        {
            var reliableQueue = Substitute.For<IReliableQueue<object>>();

            reliableQueue
                .GetCountAsync(Arg.Any<ITransaction>())
                .Returns(
                    returnThis: Task.FromResult(1L),
                    returnThese: items.Skip(1).Select(i => 1L).Concat(new[] {0L}).Select(Task.FromResult).ToArray());

            reliableQueue
                .TryDequeueAsync(Arg.Any<ITransaction>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
                .Returns(
                    returnThis: ConditionalItem(items.First()),
                    returnThese: items.Skip(1).Select(ConditionalItem).ToArray());

            return reliableQueue;
        }

        private static Task<ConditionalValue<object>> ConditionalItem(object item)
        {
            return Task.FromResult(new ConditionalValue<object>(true, item));
        }

        private static async Task DispatcherCompletionAsync(Task test)
        {
            try
            {
                await test.ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }
        }
    }
}
