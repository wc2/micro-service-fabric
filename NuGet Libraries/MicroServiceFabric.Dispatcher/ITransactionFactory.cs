using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Dispatcher
{
    public interface ITransactionFactory
    {
        ITransaction Create();
    }
}