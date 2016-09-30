using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroServiceFabric.Dispatcher
{
    public interface IReliableDispatcher<T> : IDisposable
    {
        Task EnqueueAsync(T item);
        Task RunAsync(DispatcherTask<T> dispatcherTask, CancellationToken cancellationToken);
    }
}