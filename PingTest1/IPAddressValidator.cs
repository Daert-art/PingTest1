using System.Net;
using System.Text.RegularExpressions;

namespace PingTest
{
    public class IPAddressValidator
    {
        public static bool ValidateIPAddress(string ipAddress)
        {
            string ipPattern = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";
            Regex regex = new Regex(ipPattern);

            if (regex.IsMatch(ipAddress))
            {
                IPAddress parsedIpAddress;
                if (IPAddress.TryParse(ipAddress, out parsedIpAddress))
                {                    
                    return true;
                }
            }
            return false;
        }
        public static bool CheckInstantAddress(string ipAddress, List<IpAddressInfo> values)
        {
            if (ipAddress == null || values == null)
                return false;
            var distinctValues = values.GroupBy(ip => ip.IpAddress).Select(group => group.First());
            foreach (IpAddressInfo ip in distinctValues)
            {
                if (ip.IpAddress == ipAddress)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
