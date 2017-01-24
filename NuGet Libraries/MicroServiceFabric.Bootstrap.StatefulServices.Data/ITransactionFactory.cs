using Microsoft.ServiceFabric.Data;

namespace MicroServiceFabric.Bootstrap.StatefulServices.Data
{
    public interface ITransactionFactory
    {
        ITransaction Create();
    }
}