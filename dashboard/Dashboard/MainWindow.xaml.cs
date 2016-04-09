using System;
using System.CodeDom;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Looksfamiliar.D2C2D.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
        //    GetDeviceList();

        //    if (DeviceList.Items.Count == 0)
        //    {
        //        StartButton.IsEnabled = false;
        //        StopButton.IsEnabled = false;
        //        PingButton.IsEnabled = false;
        //    }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ProvisionButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void GetDeviceList()
        //{
        //    var devices = _provisionM.GetAll();

        //    foreach (var device in devices.list)
        //    {
        //        DeviceList.Items.Add(device.serialnumber);
        //    }

        //    DeviceList.SelectedIndex = 0;
        //}

        //private static async Task<d2c2d.MessageModels.Location> GetLocationAsync()
        //{
        //    var client = new HttpClient();
        //    var json = await client.GetStringAsync("http://ip-api.com/json");
        //    var location = JsonConvert.DeserializeObject<d2c2d.MessageModels.Location>(json);
        //    return location;
        //}

        private void DeviceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (DeviceList.Items.Count == 0) return;
            //var deviceId = (string)DeviceList.SelectedItems[0];
            //PingFeed.Clear();
            //TelemetryFeed.Clear();
            //AlarmFeed.Clear();
            //_currDevice = _provisionM.GetById(deviceId);
        }
    }
}
