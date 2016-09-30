using System;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Data.Collections;

namespace MicroServiceFabric.Dispatcher
{
    public sealed class ReliableDispatcher<T> : IReliableDispatcher<T>
    {
        public ReliableDispatcher(Lazy<IReliableQueue<T>> reliableQueue, ITransactionFactory transactionFactory)
        {
            Contract.RequiresNotNull(reliableQueue, nameof(reliableQueue));
            Contract.RequiresNotNull(transactionFactory, nameof(transactionFactory));
        }

        Task IReliableDispatcher<T>.EnqueueAsync(T item)
        {
            throw new ArgumentNullException(nameof(item));
        }
    }
}
