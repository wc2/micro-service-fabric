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
| MicroServiceFabric.Bootstrap.Actors | Package for bootstrapping Service Fabric [Actors](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-actors-introduction/). |
| MicroServiceFabric.Bootstrap.StatefulService | Package for bootstrapping Service Fabric [Stateful Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). |
| MicroServiceFabric.Bootstrap.StatelessService | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/). |
| MicroServiceFabric.Bootstrap.StatelessService.Owin | Package for bootstrapping Service Fabric [Stateless Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/) that expose OWIN-hosted applications, such as Web APIs. |
| [MicroServiceFabric.Dispatcher](https://www.nuget.org/packages/MicroServiceFabric.Dispatcher/) | Package providing an [event dispatcher](https://en.wikipedia.org/wiki/Event_loop). The dispatcher builds upon the Service Fabric [IReliableQueue](https://msdn.microsoft.com/en-us/library/azure/dn971527.aspx?f=255&MSPPError=-2147217396). | 

## License

**Micro Service Fabric** is released under [Apache License 2.0](https://raw.githubusercontent.com/wc2/micro-service-fabric/master/LICENSE.txt).

## Copyright

Copyright 2016 William Cowell Consulting Limited.
