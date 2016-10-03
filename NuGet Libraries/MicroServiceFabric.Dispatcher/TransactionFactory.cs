using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Dispatcher
{
    public sealed class TransactionFactory : ITransactionFactory
    {
        private readonly IReliableStateManager _reliableStateManager;

        public TransactionFactory(IReliableStateManager reliableStateManager)
        {
            Contract.RequiresNotNull(reliableStateManager, nameof(reliableStateManager));

            _reliableStateManager = reliableStateManager;
        }

        ITransaction ITransactionFactory.Create()
        {
            return _reliableStateManager.CreateTransaction();
        }
    }
}