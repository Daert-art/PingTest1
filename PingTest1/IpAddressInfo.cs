namespace PingTest
{
    public class IpAddressInfo
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public bool IsReachable { get; set; }

        public override string ToString()
        {
            return $"{IpAddress} {(IsReachable ? "Reachable" : "Not Reachable")}";
        }
    }
}
