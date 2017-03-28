using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.Bootstrap.StatefulServices.Data;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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

            await Assert.ThrowsAsync<OperationCanceledException>(
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

            await Assert.ThrowsAsync<OperationCanceledException>(() => task).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_DoesNotDequeueWhenQueueIsEmpty()
        {
            var reliableQueue = Substitute.For<IReliableQueue<object>>();

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(reliableQueue))
            {
                task = reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), CancellationToken.None);

                await reliableQueue
                    .DidNotReceive()
                    .TryDequeueAsync(Arg.Any<ITransaction>())
                    .ConfigureAwait(false); 
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_AbortsTransaction_WhenExecutionOfDispatcherTaskFailed()
        {
            var item = Substitute.For<object>();
            var items = new[] {item};
            var transaction = Substitute.For<ITransaction>();

            var dispatcherTask = Substitute.For<DispatcherTask<object>>();
            dispatcherTask
                .Invoke(transaction, item, Arg.Any<CancellationToken>())
                .Throws(new Exception("An inner random exception was thrown"));

            var transactionFactory = Substitute.For<ITransactionFactory>();
            transactionFactory
                .Create()
                .Returns(Substitute.For<ITransaction>(), transaction, Substitute.For<ITransaction>());

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(CreateReliableQueue(items), transactionFactory))
            {
                task = reliableDispatcher.RunAsync(dispatcherTask, CancellationToken.None);
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);

            transaction
                .Received(1)
                .Abort();

            await transaction
                .DidNotReceive()
                .CommitAsync()
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_RemovesItemFromQueue_WhenExecutionOfDispatcherTaskFailed()
        {
            var items = new[] {Substitute.For<object>()};
            var transaction = Substitute.For<ITransaction>();

            var dispatcherTask = Substitute.For<DispatcherTask<object>>();
            dispatcherTask
                .Invoke(Arg.Any<ITransaction>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
                .Throws(new Exception("An inner random exception was thrown"));

            var transactionFactory = Substitute.For<ITransactionFactory>();
            transactionFactory
                .Create()
                .Returns(Substitute.For<ITransaction>(), Substitute.For<ITransaction>(), transaction, Substitute.For<ITransaction>());

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(CreateReliableQueue(items), transactionFactory))
            {
                task = reliableDispatcher.RunAsync(dispatcherTask, CancellationToken.None);
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);

            await transaction
                .Received(1)
                .CommitAsync()
                .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task RunAsync_DispatchesItemsAlreadyInQueue(int numberOfItemsAlreadyInQueue)
        {
            var dispatcherTask = Substitute.For<DispatcherTask<object>>();
            var items = Enumerable
                .Range(1, numberOfItemsAlreadyInQueue)
                .Select(i => Substitute.For<object>())
                .ToArray();

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(CreateReliableQueue(items)))
            {
                task = reliableDispatcher.RunAsync(dispatcherTask, CancellationToken.None);

                items
                    .ToList()
                    .ForEach(item =>
                        dispatcherTask
                            .Received()
                            .Invoke(
                                Arg.Is<ITransaction>(tx => tx != null),
                                item,
                                Arg.Is<CancellationToken>(t => t !=  default(CancellationToken))));

                await dispatcherTask
                    .Received(numberOfItemsAlreadyInQueue)
                    .Invoke(
                        Arg.Is<ITransaction>(tx => tx != null),
                        Arg.Any<object>(),
                        Arg.Is<CancellationToken>(t => t != default(CancellationToken)))
                    .ConfigureAwait(false);
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_CommitsGetItemTransaction()
        {
            var getCountTransaction = Substitute.For<ITransaction>();
            var getItemTransaction = Substitute.For<ITransaction>();

            var transactionFactory = Substitute.For<ITransactionFactory>();
            transactionFactory
                .Create()
                .Returns(getCountTransaction, getItemTransaction, getCountTransaction);

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(CreateReliableQueue(Substitute.For<object>()), transactionFactory))
            {
                task = reliableDispatcher.RunAsync(Substitute.For<DispatcherTask<object>>(), CancellationToken.None);
            }

            await DispatcherCompletionAsync(task).ConfigureAwait(false);

            await getItemTransaction
                .Received()
                .CommitAsync()
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task RunAsync_EnqueueAsyncRestartsDispatcher()
        {
            var dispatcherTask = Substitute.For<DispatcherTask<object>>();
            var item = Substitute.For<object>();

            var reliableQueue = Substitute.For<IReliableQueue<object>>();
            reliableQueue
                .GetCountAsync(Arg.Is<ITransaction>(tx => tx != null))
                .Returns(0);
            reliableQueue
                .TryDequeueAsync(Arg.Is<ITransaction>(tx => tx != null), Arg.Any<TimeSpan>(),
                    Arg.Any<CancellationToken>())
                .Returns(ConditionalItem(item));

            Task task;
            using (var reliableDispatcher = CreateReliableDispatcher(reliableQueue))
            {
                task = reliableDispatcher.RunAsync(dispatcherTask, CancellationToken.None);

                await reliableDispatcher.EnqueueAsync(item).ConfigureAwait(false);
                await Task.Delay(250).ConfigureAwait(false);
            }

            await dispatcherTask
                .Received(1)
                .Invoke(Arg.Is<ITransaction>(tx => tx != null), item, Arg.Is<CancellationToken>(t => t != default(CancellationToken)))
                .ConfigureAwait(false);

            await DispatcherCompletionAsync(task).ConfigureAwait(false);
        }

        private static IReliableDispatcher<object> CreateReliableDispatcher(
            IReliableQueue<object> reliableQueue = null,
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

        private static IReliableQueue<object> CreateReliableQueue(params object[] items)
        {
            var reliableQueue = Substitute.For<IReliableQueue<object>>();

            reliableQueue
                .GetCountAsync(Arg.Is<ITransaction>(tx => tx != null))
                .Returns(
                    returnThis: Task.FromResult(1L),
                    returnThese: items.Skip(1).Select(i => 1L).Concat(new[] {0L}).Select(Task.FromResult).ToArray());

            reliableQueue
                .TryDequeueAsync(Arg.Is<ITransaction>(tx => tx != null), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
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
            catch (OperationCanceledException) { }
        }
    }
}
