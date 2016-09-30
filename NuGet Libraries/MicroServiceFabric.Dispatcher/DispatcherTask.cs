using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Dispatcher
{
    public delegate Task DispatcherTask<in T>(ITransaction transaction, T item);
}