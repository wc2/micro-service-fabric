using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;

namespace MicroServiceFabric.Bootstrap
{
    public sealed class GetSettings : IGetSettings
    {
        private readonly KeyedCollection<string, ConfigurationSection> _configurationSections;

        public GetSettings(ServiceContext context)
        {
            var configurationPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            _configurationSections = configurationPackage.Settings.Sections;
        }

        IDictionary<string, string> IGetSettings.GetSettings(string sectionName)
        {
            if (!_configurationSections.Contains(sectionName))
            {
                throw new ArgumentException(
                    $"Section with specified name '{sectionName}' cannot be found in the Service Fabric application configuration.");
            }

            return _configurationSections[sectionName]
                .Parameters
                .ToDictionary(k => k.Name, v => v.Value);
        }
    }
}