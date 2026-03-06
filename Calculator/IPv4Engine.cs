using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Calculator
{
    public class IPv4Engine
    {
        public List<SubnetResult> CalculateVLSM(string baseIp, int baseCidr, List<int> hostsRequirements)
        {
            var results = new List<SubnetResult>();

            uint currentIpInt = IpToUint(baseIp);

            //wymagania malejaco do vlsm, najwieksze podsieci alokowane pierwsze
            var sortedRequirements = hostsRequirements.OrderByDescending(h => h).ToList();

            foreach (var reqHosts in sortedRequirements)
            {
                // obliczenie potrzebnej maski
                int hostBits = (int)Math.Ceiling(Math.Log2(reqHosts + 2));
                int newCidr = 32 - hostBits;

                if (newCidr > 30) newCidr = 30;

                // obliczenie podsieci
                uint maskInt = ~(uint.MaxValue >> newCidr);
                uint networkInt = currentIpInt & maskInt; 
                uint broadcastInt = networkInt | ~maskInt;

                results.Add(new SubnetResult
                {
                    NetworkAddress = UintToIp(networkInt),
                    BroadcastAddress = UintToIp(broadcastInt),
                    Cidr = newCidr,
                    SubnetMask = UintToIp(maskInt),
                    FirstHost = UintToIp(networkInt + 1),
                    LastHost = UintToIp(broadcastInt - 1),
                    HostsCount = reqHosts
                });

                // rozpoczecie nastepnej sieci od broadcastu poprzedniej +1
                currentIpInt = broadcastInt + 1;
            }

            return results;
        }

        //Metody pomocnicze
        private uint IpToUint(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            byte[] bytes = ip.GetAddressBytes();

            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private string UintToIp(uint ipInt)
        {
            byte[] bytes = BitConverter.GetBytes(ipInt);
            Array.Reverse(bytes);
            return new IPAddress(bytes).ToString();
        }
    }
}
