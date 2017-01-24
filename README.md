# Micro Service Fabric

Repository containing packages for working with [Service Fabric](https://azure.microsoft.com/en-us/documentation/services/service-fabric/) applications.

## Contents

* Getting started
* Components
* License
* Copyright

## Getting started

See the [Wiki](https://github.com/wc2/micro-service-fabric/wiki) for information on how to get started.

## Components

| Component | Description |
| --------- | ----------- |
| [MicroServiceFabric.Bootstrap.Actors](https://www.nuget.org/packages/MicroServiceFabric.Bootstrap.Actors/) | Package for bootstrapping Service Fabric [Actors](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-actors-introduction/). |
| [MicroServiceFabric.Bootstrap.StatefulService](https://www.nuget.org/packages/MicroServiceFabric.Bootstrap.StatefulServices/) | Package for bootstrapping Service Fabric [Stateful Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). |
| [MicroServiceFabric.Bootstrap.StatefulService.Data](https://www.nuget.org/packages/MicroServiceFabric.Bootstrap.StatefulServices.Data/) | Package to facilitate bootstrapping Reliable Services that require [Reliable Collections](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-reliable-collections) upon construction.(https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). |
| [MicroServiceFabric.Bootstrap.StatelessService](https://www.nuget.org/packages/MicroServiceFabric.Bootstrap.StatelessServices/) | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). |
| [MicroServiceFabric.Bootstrap.StatelessService.Owin](https://www.nuget.org/packages/MicroServiceFabric.Bootstrap.StatelessServices.Owin/) | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/) that expose OWIN-hosted applications, such as Web APIs. |
| [MicroServiceFabric.Dispatcher](https://www.nuget.org/packages/MicroServiceFabric.Dispatcher/) | Package providing an [event dispatcher](https://en.wikipedia.org/wiki/Event_loop). The dispatcher builds upon the Service Fabric [IReliableQueue](https://msdn.microsoft.com/en-us/library/azure/dn971527.aspx?f=255&MSPPError=-2147217396). | 

## License

**Micro Service Fabric** is released under [Apache License 2.0](https://raw.githubusercontent.com/wc2/micro-service-fabric/master/LICENSE.txt).

## Copyright

Copyright 2017 William Cowell Consulting Limited.
