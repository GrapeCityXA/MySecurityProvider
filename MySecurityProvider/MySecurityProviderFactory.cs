using GrapeCity.Enterprise.Identity.ExternalIdentityProvider.Configuration;
using GrapeCity.Enterprise.Identity.SecurityProvider;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySecurityProvider
{
    public class MySecurityProviderFactory: ISecurityProviderFactory
    {
        public string ProviderName
        {
            get
            {
                return Constants.ProviderName;
            }
        }

        public string Description
        {
            get
            {
                return Constants.Description;
            }
        }

        public IEnumerable<ConfigurationItem> SupportedSettings
        {
            get
            {
                List<ConfigurationItem> settings = new List<ConfigurationItem>();
                settings.Add(new ConfigurationItem("DatabaseConnection", "Database Connection", "Connection string goes here."));
                return settings;
            }
        }
            
        public Task<ISecurityProvider> CreateAsync(IEnumerable<ConfigurationItem> settings, ILogger logger)
        {
            return Task.FromResult<ISecurityProvider>(new MySecurityProvider(settings));
        }
    }
}
