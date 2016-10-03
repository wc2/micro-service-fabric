using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Reliable Dispatcher")]
[assembly: AssemblyDescription("Provides an [event dispatcher](https://en.wikipedia.org/wiki/Event_loop) (ReliableDispatcher) and an abstract service implementation of an event dispatcher (StatefulDispatcherService). The dispatcher builds upon the Service Fabric [IReliableQueue](https://msdn.microsoft.com/en-us/library/azure/dn971527.aspx?f=255&MSPPError=-2147217396).")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c330b920-c5e9-4d9a-8e22-0a082bb85410")]
[assembly: InternalsVisibleTo("MicroServiceFabric.Dispatcher.Tests")]