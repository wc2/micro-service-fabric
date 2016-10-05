using System;
using System.Collections.Generic;
using System.Fabric;
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
            Contract.RequiresNotNull(serviceEventSource, nameof(serviceEventSource));
            Contract.RequiresNotNull(appBuilder, nameof(appBuilder));

            _serviceEventSource = serviceEventSource;
            _appBuilder = appBuilder;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(
                    serviceContext =>
                        new OwinCommunicationListener(_appBuilder, serviceContext, _serviceEventSource, "ServiceEndpoint"))
            };
        }
    }
}