using System;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceFabric.CodeContracts;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;

namespace MicroServiceFabric.Bootstrap.StatelessServices.Owin
{
    internal class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IServiceEventSource _eventSource;
        private readonly Action<IAppBuilder> _startup;
        private readonly ServiceContext _serviceContext;
        private readonly string _endpointName;
        private readonly string _appRoot;

        private IDisposable _webApp;
        private string _publishAddress;
        private string _listeningAddress;

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, IServiceEventSource eventSource, string endpointName)
            : this(startup, serviceContext, eventSource, endpointName, null)
        {
        }

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, IServiceEventSource eventSource, string endpointName, string appRoot)
        {
            Requires.IsNotNull(startup, nameof(startup));
            Requires.IsNotNull(serviceContext, nameof(serviceContext));
            Requires.IsNotNull(endpointName, nameof(endpointName));
            Requires.IsNotNull(eventSource, nameof(eventSource));

            _startup = startup;
            _serviceContext = serviceContext;
            _endpointName = endpointName;
            _eventSource = eventSource;
            _appRoot = appRoot;
        }

        public bool ListenOnSecondary { get; set; }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            _eventSource.ServiceMessage(_serviceContext, "Calling OpenAsync on endpoint {0}", _endpointName);

            var serviceEndpoint = _serviceContext.CodePackageActivationContext.GetEndpoint(_endpointName);

            _eventSource.ServiceMessage(_serviceContext, "Found endpoint with protocol '{0}' port '{1}'", serviceEndpoint.Protocol, serviceEndpoint.Port);

            var statefulServiceContext = _serviceContext as StatefulServiceContext;
            if (statefulServiceContext != null)
            {
                _listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}://+:{1}/{2}{3}/{4}/{5}",
                    serviceEndpoint.Protocol,
                    serviceEndpoint.Port,
                    string.IsNullOrWhiteSpace(_appRoot)
                        ? string.Empty
                        : _appRoot.TrimEnd('/') + '/',
                    statefulServiceContext.PartitionId,
                    statefulServiceContext.ReplicaId,
                    Guid.NewGuid());
            }
            else if (_serviceContext is StatelessServiceContext)
            {
                _listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}://+:{1}/{2}",
                    serviceEndpoint.Protocol,
                    serviceEndpoint.Port,
                    string.IsNullOrWhiteSpace(_appRoot)
                        ? string.Empty
                        : _appRoot.TrimEnd('/') + '/');
            }
            else
            {
                throw new InvalidOperationException();
            }

            _publishAddress = _listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            try
            {
                _eventSource.ServiceMessage(_serviceContext, "Starting web server on " + _listeningAddress);
                _webApp = WebApp.Start(_listeningAddress, appBuilder => _startup.Invoke(appBuilder));
                _eventSource.ServiceMessage(_serviceContext, "Listening on " + _publishAddress);

                return Task.FromResult(_publishAddress);
            }
            catch (Exception ex)
            {
                _eventSource.ServiceMessage(_serviceContext, "Web server for endpoint {0} failed to open. ", _endpointName, ex.ToString());

                StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _eventSource.ServiceMessage(_serviceContext, "Closing web server for endpoint {0}", _endpointName);

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            _eventSource.ServiceMessage(_serviceContext, "Aborting web server for endpoint {0}", _endpointName);

            StopWebServer();
        }

        private void StopWebServer()
        {
            if (_webApp != null)
            {
                try
                {
                    _webApp.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
