using Microsoft.ServiceFabric.Services.Runtime;

namespace MicroServiceFabric.Bootstrap.StatefulServices
{
    public interface IStatefulServiceEventSource
    {
        void Message(string message);
        void Message(string message, params object[] args);
        void ServiceHostInitializationFailed(string exception);
        void ServiceMessage(StatefulService service, string message, params object[] args);
        void ServiceRequestStart(string requestTypeName);
        void ServiceRequestStop(string requestTypeName, string exception = "");
        void ServiceTypeRegistered(int hostProcessId, string serviceType);
    }
}