using System;
using FluentAssertions;
using Microsoft.ServiceFabric.Data;
using NSubstitute;
using Xunit;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data.Tests
{
    public sealed class TransactionFactoryTests
    {
        [Fact]
        public void ctor_StateManagerRequired()
        {
            Assert.Throws<ArgumentNullException>(
                "reliableStateManager",
                () => new TransactionFactory(null));
        }

        [Fact]
        public void Create_ReturnsTransactionFromReliableStateManager()
        {
            var expectedTransaction = Substitute.For<ITransaction>();
            var reliableStorage = Substitute.For<IReliableStateManagerReplica>();
            reliableStorage
                .CreateTransaction()
                .Returns(expectedTransaction);

            ITransactionFactory factory = new TransactionFactory(reliableStorage);

            var transaction = factory.Create();

            transaction
                .Should()
                .Be(expectedTransaction);
        }
    }
}