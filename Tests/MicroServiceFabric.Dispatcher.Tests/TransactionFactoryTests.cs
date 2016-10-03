using System;
using Xunit;

namespace MicroServiceFabric.Dispatcher.Tests
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
    }
}