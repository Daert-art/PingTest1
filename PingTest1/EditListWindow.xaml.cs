using System.Windows;

namespace PingTest
{
    public partial class EditListWindow : Window
    {
        private List<IpAddressInfo> ipAddresses;

        public EditListWindow(List<IpAddressInfo> ipAddresses)
        {
            InitializeComponent();
            this.ipAddresses = ipAddresses;
            DisplayIpAddresses();
        }

        private void DisplayIpAddresses()
        {
            lbIpAddresses.Items.Clear();
            foreach (var ipAddress in ipAddresses)
            {
                lbIpAddresses.Items.Add(new Tuple<string, IpAddressInfo>($"{ipAddress.DeviceName} ({ipAddress.IpAddress})", ipAddress));
            }
        }
        private void AddIPAddressButton_Click(object sender, RoutedEventArgs e)
        {
            AddAddress();
        }

        private void EditIPAddressButton_Click(object sender, RoutedEventArgs e)
        {
            if (lbIpAddresses.SelectedItem is Tuple<string, IpAddressInfo> selectedTuple)
            {
                var selectedAddress = selectedTuple.Item2;
                string newIpAddress = txtNewIPAddress.Text.Trim();
                string newDeviceName = txtDeviceName.Text.Trim();

                if (IPAddressValidator.ValidateIPAddress(newIpAddress) && !ipAddresses.Any(ip => ip.IpAddress == newIpAddress && ip != selectedAddress))
                {
                    selectedAddress.IpAddress = newIpAddress;
                    selectedAddress.DeviceName = newDeviceName;
                    DisplayIpAddresses();
                }
                else
                {
                    MessageBox.Show("Please enter a valid and unique IP address.", "Invalid IP Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select an address to edit.", "Edit Address", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeleteIPAddressButton_Click(object sender, RoutedEventArgs e)
        {
            if (lbIpAddresses.SelectedItem is Tuple<string, IpAddressInfo> selectedTuple)
            {
                var selectedAddress = selectedTuple.Item2;
                ipAddresses.Remove(selectedAddress);
                DisplayIpAddresses();
            }
            else
            {
                MessageBox.Show("Please select an address to delete.", "Delete Address", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void txtNewIPAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddAddress();
            }
        }

        private void AddAddress()
        {
            string ipAddress = txtNewIPAddress.Text.Trim();
            string deviceName = txtDeviceName.Text.Trim();

            if (IPAddressValidator.ValidateIPAddress(ipAddress) && !ipAddresses.Any(ip => ip.IpAddress == ipAddress))
            {
                ipAddresses.Add(new IpAddressInfo { IpAddress = ipAddress, DeviceName = deviceName, IsReachable = false });
                DisplayIpAddresses();
                txtNewIPAddress.Clear();
                txtDeviceName.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid and unique IP address.", "Invalid IP Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNewIPAddress.Clear();
                txtDeviceName.Clear();
            }
        }
    }
}
