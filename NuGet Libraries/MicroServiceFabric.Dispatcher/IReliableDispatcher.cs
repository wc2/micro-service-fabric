using System.Threading.Tasks;

namespace MicroServiceFabric.Dispatcher
{
    public interface IReliableDispatcher<in T>
    {
        Task EnqueueAsync(T item);
    }
}