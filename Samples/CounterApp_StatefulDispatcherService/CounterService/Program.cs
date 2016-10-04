using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using MicroServiceFabric.Dispatcher;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CounterService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                ServiceRuntime.RegisterServiceAsync("CounterServiceType", CreateCounterService).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(CounterService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static CounterService CreateCounterService(StatefulServiceContext context)
        {
            IReliableStateManagerReplica stateManager = new ReliableStateManager(context);

            var reliableDispatcher = new ReliableDispatcher<int>(
                new Lazy<IReliableQueue<int>>(
                    () => stateManager.GetOrAddAsync<IReliableQueue<int>>("CounterIncrements").Result),
                new TransactionFactory(stateManager));

            return new CounterService(context, stateManager, reliableDispatcher);
        }
    }
}
