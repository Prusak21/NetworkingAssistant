using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Calculator
{
    public class IPv6Engine
    {
        public List<SubnetResult> GenerateSubnets(string baseIp, int currentCidr, int targetCidr, int limit = 16)
        {
            var results = new List<SubnetResult>();

            BigInteger ipVal = IPv6ToBigInteger(baseIp);

            int bitsDiff = targetCidr - currentCidr;

            if (bitsDiff < 0) throw new ArgumentException("Docelowa maska musi być większa od obecnej.");

            BigInteger increment = BigInteger.Pow(2, 128 - targetCidr);

            for (int i = 0; i < limit; i++)
            {
                BigInteger currentNetworkVal = ipVal + (increment * i);

                string netAddr = BigIntegerToIPv6(currentNetworkVal);

                results.Add(new SubnetResult
                {
                    NetworkAddress = netAddr,
                    SubnetMask = $"/{targetCidr}", 
                    Cidr = targetCidr,
                    FirstHost = netAddr,
                    HostsCount = 0 
                });
            }

            return results;
        }
        private BigInteger IPv6ToBigInteger(string ip)
        {
            var address = IPAddress.Parse(ip);
            byte[] bytes = address.GetAddressBytes();

            Array.Reverse(bytes);
            byte[] unsignedBytes = new byte[bytes.Length + 1];
            Array.Copy(bytes, unsignedBytes, bytes.Length);

            return new BigInteger(unsignedBytes);
        }

        private string BigIntegerToIPv6(BigInteger val)
        {
            byte[] bytes = val.ToByteArray();

            if (bytes.Length > 16)
            {
                var tmp = new byte[16];
                Array.Copy(bytes, tmp, 16);
                bytes = tmp;
            }
            // dopełnienie zerami
            else if (bytes.Length < 16)
            {
                var tmp = new byte[16];
                Array.Copy(bytes, tmp, bytes.Length);
                bytes = tmp;
            }

            Array.Reverse(bytes); 
            return new IPAddress(bytes).ToString();
        }
    }
}
