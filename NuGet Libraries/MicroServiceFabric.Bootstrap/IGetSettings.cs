using System.Collections.Generic;

namespace MicroServiceFabric.Bootstrap
{
    public interface IGetSettings
    {
        IDictionary<string, string> GetSettings(string sectionName);
    }
}