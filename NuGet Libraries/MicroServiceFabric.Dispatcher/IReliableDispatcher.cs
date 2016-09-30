using System.Threading;
using System.Threading.Tasks;

namespace MicroServiceFabric.Dispatcher
{
    public interface IReliableDispatcher<T>
    {
        Task EnqueueAsync(T item);
        Task RunAsync(DispatcherTask<T> dispatcherTask, CancellationToken cancellationToken);
    }
}