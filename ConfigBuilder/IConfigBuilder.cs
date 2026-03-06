using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.ConfigBuilder
{
    public interface IConfigBuilder
    {
        void SetHostname(string hostname);
        void SetBanner(string message);
        void DisableDnsLookup();
        void SetEnableSecret(string password);
        void CreateAdminUser(string username, string password);
        void EnablePasswordEncryption();
        void ConfigureSshAccess(); 
        void AddInterface(string name, string ip, string mask, string description);
        void AddDhcpPool(string poolName, string network, string mask, string defaultGw);
        void ConfigureConsoleLogging();
        RouterConfig Build();
    }
}
