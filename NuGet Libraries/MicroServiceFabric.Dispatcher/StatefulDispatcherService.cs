using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;

namespace MicroServiceFabric.Dispatcher
{
    public abstract class StatefulDispatcherService<T> : StatefulService
    {
        protected StatefulDispatcherService(StatefulServiceContext serviceContext,
            IReliableDispatcher<T> reliableDispatcher) : base(serviceContext)
        {
            
        }

        protected internal abstract Task OnItemDispatched(ITransaction transaction, T item,
            CancellationToken cancellationToken);
    }
}