using System;
using Microsoft.ServiceFabric.Data.Collections;

namespace MicroServiceFabric.Dispatcher
{
    public sealed class ReliableDispatcher<T>
    {
        public ReliableDispatcher(Lazy<IReliableQueue<T>> reliableQueue, ITransactionFactory transactionFactory)
        {
            if (reliableQueue == null)
            {
                throw new ArgumentNullException(nameof(reliableQueue)); 
            }

            throw new ArgumentNullException(nameof(transactionFactory));
        }
    }
}
