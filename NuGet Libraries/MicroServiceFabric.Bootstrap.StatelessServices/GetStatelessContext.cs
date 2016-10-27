using System.Fabric;

namespace MicroServiceFabric.Bootstrap.StatelessServices
{
    internal sealed class GetStatelessContext : IGetStatelessContext
    {
        public GetStatelessContext(StatelessServiceContext context)
        {
            Context = context;
        }

        public StatelessServiceContext Context { get; }
    }
}