using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Calculator
{
    public class SubnetResult
    {
        public string NetworkAddress { get; set; }
        public string BroadcastAddress { get; set; }
        public string SubnetMask { get; set; }
        public int Cidr { get; set; }
        public string FirstHost { get; set; }
        public string LastHost { get; set; }
        public int HostsCount { get; set; }
    }
}
