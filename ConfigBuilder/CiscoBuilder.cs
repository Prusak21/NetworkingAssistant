using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.ConfigBuilder
{
    public class CiscoBuilder : IConfigBuilder
    {
        private readonly RouterConfig _config = new RouterConfig();

        public CiscoBuilder()
        {
            _config.AppendLine("version 15.1");
            _config.AppendLine("service timestamps debug datetime msec");
            _config.AppendLine("service timestamps log datetime msec");
        }
        public void SetHostname(string hostname) 
        {
            _config.AppendLine($"hostname {hostname}");
        }

        public void SetBanner(string message) 
        {
            _config.AppendLine($"banner motd #{message}#");
        }

        public void DisableDnsLookup() 
        {
            _config.AppendLine("no ip domain-lookup");
        }
        public void SetEnableSecret(string password) 
        {
            _config.AppendLine($"enable secret {password}");
        }

        public void EnablePasswordEncryption() 
        {
            _config.AppendLine("service password-encryption");
        }

        public void CreateAdminUser(string username, string password) 
        {
            _config.AppendLine($"username {username} privilege 15 secret {password}");
        }

        public void ConfigureSshAccess()
        {
            _config.AppendLine("ip domain-name lan.local");
            _config.AppendLine("crypto key generate rsa modulus 1024");
            _config.AppendLine("line vty 0 4");
            _config.AppendLine(" transport input ssh");
            _config.AppendLine(" login local");
            _config.AppendLine(" exit");
        }

        public void ConfigureConsoleLogging() 
        {
            _config.AppendLine("line console 0");
            _config.AppendLine(" logging synchronous");
            _config.AppendLine(" exit");
        }

        public void AddInterface(string name, string ip, string mask, string description)
        {
            _config.AppendLine($"interface {name}");
            if (!string.IsNullOrEmpty(description))
                _config.AppendLine($" description {description}");
            _config.AppendLine($" ip address {ip} {mask}");
            _config.AppendLine(" no shutdown");
            _config.AppendLine(" exit");
        }

        public void AddDhcpPool(string poolName, string network, string mask, string defaultGw)
        {
            _config.AppendLine($"ip dhcp pool {poolName}");
            _config.AppendLine($" network {network} {mask}");
            _config.AppendLine($" default-router {defaultGw}");
            _config.AppendLine(" dns-server 8.8.8.8");
            _config.AppendLine(" exit");
        }

        public RouterConfig Build()
        {
            _config.AppendLine("!");
            _config.AppendLine("end");
            _config.AppendLine("write memory"); 
            return _config;
        }
    }
}
