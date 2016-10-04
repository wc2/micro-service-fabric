using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using NSubstitute;
using Xunit;

namespace MicroServiceFabric.Dispatcher.Tests
{
    public sealed class StatefulDispatcherServiceTests
    {
        [Fact]
        public void ctor_ReliableDispatcherRequired()
        {
            Assert.Throws<ArgumentNullException>(
                "reliableDispatcher",
                () =>
                    new TestStatefulDispatcherService<object>(
                        CreateStatefulServiceContext(),
                        Substitute.For<IReliableStateManagerReplica>(),
                        null));
        }

        [Fact]
        public void Dispose_DisposesReliableDispatcher()
        {
            var reliableDispatcher = Substitute.For<IReliableDispatcher<object>>();
            IDisposable service = CreateStatefulDispatcherService(reliableDispatcher);

            service.Dispose();

            reliableDispatcher
                .Received()
                .Dispose();
        }

        [Fact]
        public async Task RunDispatcherAsync_InvokesReliableDispatcherRunAsyncWithOnItemDispatchedAsync()
        {
            var tokenSource = new CancellationTokenSource();
            var reliableDispatcher = Substitute.For<IReliableDispatcher<object>>();
            var service = CreateStatefulDispatcherService(reliableDispatcher);

            await service.RunDispatcherAsync(tokenSource.Token).ConfigureAwait(false);

            await reliableDispatcher
                .Received()
                .RunAsync(service.OnItemDispatchedAsync, tokenSource.Token)
                .ConfigureAwait(false);
        }

        private static TestStatefulDispatcherService<T> CreateStatefulDispatcherService<T>(
            IReliableDispatcher<T> reliableDispatcher)
        {
            return new TestStatefulDispatcherService<T>(
                CreateStatefulServiceContext(),
                Substitute.For<IReliableStateManagerReplica>(),
                reliableDispatcher);
        }

        private static StatefulServiceContext CreateStatefulServiceContext()
        {
            return new StatefulServiceContext(
                new NodeContext("", new NodeId(0, 1), 0, "Type", "localhost"),
                Substitute.For<ICodePackageActivationContext>(),
                "ServiceTypeName",
                new Uri("fabric:/App/ServiceTypeName", UriKind.Absolute),
                null,
                Guid.Empty,
                0L);
        }

        private sealed class TestStatefulDispatcherService<T> : StatefulDispatcherService<T>
        {
            public TestStatefulDispatcherService(StatefulServiceContext serviceContext,
                IReliableStateManagerReplica reliableStateManagerReplica, IReliableDispatcher<T> reliableDispatcher)
                : base(serviceContext, reliableStateManagerReplica, reliableDispatcher)
            {
            }

            public override Task OnItemDispatchedAsync(ITransaction transaction, T item,
                CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}