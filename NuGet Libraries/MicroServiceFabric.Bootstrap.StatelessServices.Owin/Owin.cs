using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using MicroServiceFabric.CodeContracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Owin;

namespace MicroServiceFabric.Bootstrap.StatelessServices.Owin
{
    internal sealed class Owin : StatelessService
    {
        private readonly Action<IAppBuilder> _appBuilder;
        private readonly IServiceEventSource _serviceEventSource;

        public Owin(StatelessServiceContext serviceContext, IServiceEventSource serviceEventSource,
            Action<IAppBuilder> appBuilder) : base(serviceContext)
        {
            Requires.IsNotNull(serviceEventSource, nameof(serviceEventSource));
            Requires.IsNotNull(appBuilder, nameof(appBuilder));

            _serviceEventSource = serviceEventSource;
            _appBuilder = appBuilder;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return Context.CodePackageActivationContext
                .GetEndpoints()
                .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https)
                .Select(endpoint => new ServiceInstanceListener(
                    serviceContext => new OwinCommunicationListener(_appBuilder, serviceContext, _serviceEventSource, endpoint.Name), endpoint.Name));
        }
    }
}