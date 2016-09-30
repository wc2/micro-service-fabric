# Micro Service Fabric

Repository containing packages for working with [Service Fabric](https://azure.microsoft.com/en-us/documentation/services/service-fabric/) applications.

## Components

| Component | Description | NuGet |
| --------- | ----------- | ----- |
| MicroServiceFabric.Actors | Package for bootstrapping Service Fabric [Actors](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-actors-introduction/). | |
| MicroServiceFabric.MessagePump | Package providing a reliable message pump. The message pump builds upon the Service Fabric [IReliableQueue](https://msdn.microsoft.com/en-us/library/azure/dn971527.aspx?f=255&MSPPError=-2147217396).  |  |
| MicroServiceFabric.Logging | Package for providing common logging functionality. | |
| MicroServiceFabric.Services.StatefulService | Package for bootstrapping Service Fabric [Stateful Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). | |
| MicroServiceFabric.Services.StatelessService | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). | |
| MicroServiceFabric.Services.StatelessService.Owin | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/) that expose OWIN-hosted applications, such as Web APIs. | |

## License

NGauge is released under Apache License 2.0

## Copyright

Copyright 2016 William Cowell Consulting Limited.
