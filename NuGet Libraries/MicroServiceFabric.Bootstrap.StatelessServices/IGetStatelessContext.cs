using System.Fabric;

namespace MicroServiceFabric.Bootstrap.StatelessServices
{
    public interface IGetStatelessContext
    {
        StatelessServiceContext Context { get; }
    }
}