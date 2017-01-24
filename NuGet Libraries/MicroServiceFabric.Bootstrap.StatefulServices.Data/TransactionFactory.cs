using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public sealed class TransactionFactory : ITransactionFactory
    {
        private readonly IReliableStateManager _reliableStateManager;

        public TransactionFactory(IReliableStateManager reliableStateManager)
        {
            Requires.IsNotNull(reliableStateManager, nameof(reliableStateManager));

            _reliableStateManager = reliableStateManager;
        }

        ITransaction ITransactionFactory.Create()
        {
            return _reliableStateManager.CreateTransaction();
        }
    }
}