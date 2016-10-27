using System.Fabric;

namespace MicroServiceFabric.Bootstrap.StatefulServices
{
    public interface IGetStatefulContext
    {
        StatefulServiceContext Context { get; }
    }
}