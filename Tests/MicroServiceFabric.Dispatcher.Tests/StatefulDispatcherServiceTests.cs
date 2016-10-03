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

        private static TestStatefulDispatcherService<T> CreateStatefulDispatcherService<T>(IReliableDispatcher<T> reliableDispatcher)
        {
            return new TestStatefulDispatcherService<T>(
                CreateStatefulServiceContext(),
                reliableDispatcher);
        }

        private static StatefulServiceContext CreateStatefulServiceContext()
        {
            return new StatefulServiceContext(
                new NodeContext("", new NodeId(0, 1), 0, "Type","localhost"),
                Substitute.For<ICodePackageActivationContext>(),
                "ServiceTypeName",
                new Uri("fabric:/App/ServiceTypeName", UriKind.Absolute),
                null,
                Guid.Empty,
                0L);
        }

        private sealed class TestStatefulDispatcherService<T> : StatefulDispatcherService<T>
        {
            public TestStatefulDispatcherService(StatefulServiceContext serviceContext, IReliableDispatcher<T> reliableDispatcher) : base(serviceContext, reliableDispatcher)
            {
            }

            protected override Task OnItemDispatched(ITransaction transaction, T item, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
