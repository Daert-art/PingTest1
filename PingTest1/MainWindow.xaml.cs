using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;

namespace PingTest
{
    public partial class MainWindow : Window
    {
        private List<IpAddressInfo> ipAddresses;
        private DispatcherTimer pingTimer;

        public MainWindow()
        {
            InitializeComponent();
            ipAddresses = new List<IpAddressInfo>();
            LoadIpAddresses();
            DisplayIpAddresses();

            pingTimer = new DispatcherTimer();
            pingTimer.Interval = TimeSpan.FromSeconds(10); 
            pingTimer.Tick += PingTimer_Tick;
            pingTimer.Start();
        }

        private void LoadIpAddresses()
        {
            ipAddresses.Clear();
            try
            {
                string[] lines = File.ReadAllLines("ipAddresses.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        string deviceName = parts[0].Trim();
                        string ipAddress = parts[1].Trim();
                        bool isReachable = bool.Parse(parts[2].Trim());
                        ipAddresses.Add(new IpAddressInfo { DeviceName = deviceName, IpAddress = ipAddress, IsReachable = isReachable });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load IP addresses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveIpAddresses()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (var ipAddress in ipAddresses)
                {
                    lines.Add($"{ipAddress.DeviceName}, {ipAddress.IpAddress}, {ipAddress.IsReachable}");
                }
                File.WriteAllLines("ipAddresses.txt", lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save IP addresses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DisplayIpAddresses()
        {
            lbPingResults.Items.Clear();
            foreach (var ipAddress in ipAddresses.DistinctBy(ip => ip.IpAddress))
            {
                lbPingResults.Items.Add(ipAddress);
            }
        }

        private async void PingTimer_Tick(object sender, EventArgs e)
        {
            List<IpAddressInfo> ipAddressesCopy = ipAddresses.ToList();

            await Task.Run(() =>
            {
                foreach (var ipAddress in ipAddressesCopy)
                {
                    ipAddress.IsReachable = PingIpAddressMultipleTimes(ipAddress.IpAddress, 3);
                }
            });

            DisplayIpAddresses();
            SaveIpAddresses();
        }
        private bool PingIpAddressMultipleTimes(string ipAddress, int attempts)
        {
            int successfulPings = 0;

            for (int i = 0; i < attempts; i++)
            {
                if (PingIpAddress(ipAddress))
                {
                    successfulPings++;
                }
            }

            return successfulPings > 0;
        }
        private bool PingIpAddress(string ipAddress)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ipAddress);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void EditListButton_Click(object sender, RoutedEventArgs e)
        {
            EditListWindow editWindow = new EditListWindow(ipAddresses);
            editWindow.ShowDialog();
            DisplayIpAddresses();
            SaveIpAddresses();
        }

        private void SetIntervalButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtPingInterval.Text, out int interval) && interval > 0)
            {
                pingTimer.Interval = TimeSpan.FromSeconds(interval);
                MessageBox.Show($"Ping interval set to {interval} seconds.", "Interval Set", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid interval (in seconds).", "Invalid Interval", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
