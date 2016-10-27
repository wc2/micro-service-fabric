using System.Fabric;

namespace MicroServiceFabric.Bootstrap.StatefulServices
{
    internal sealed class GetStatefulContext : IGetStatefulContext
    {
        public GetStatefulContext(StatefulServiceContext context)
        {
            Context = context;
        }

        public StatefulServiceContext Context { get; }
    }
}