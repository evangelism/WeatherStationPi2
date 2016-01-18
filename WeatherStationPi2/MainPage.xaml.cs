using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MakerFriendly.Azure.EventHub;
using MakerFriendly.I2C;

namespace WeatherStationPi2
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        BMP180 sensor = new BMP180();
        EventHubHelper EHub;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await sensor.Init();
            EHub = new EventHubHelper();
            EHub.serviceNamespace = "weathercenter-ns";
            EHub.hubName = "weatherhub";
            EHub.deviceName = "shwarspi";
            EHub.sharedAccessPolicyName = "all";
            EHub.sharedAccessKey = "cFsp8GEvk/iRnjehSt/JBHjIyAV0lGVBWGJqAc9/IMw=";

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += async (sender, o) =>
            {
                var temp = sensor.Temperature;
                Debug.WriteLine("Temperature = " + temp);
                TheTextBlock.Text = $"Temperature : {temp}°C";
                await EHub.SendMessage(
                    $"{{\"temperature\":\"{temp.ToString().Replace(',', '.')}\", \"source\":\"RPi2\", \"timewhen\":\"{DateTime.Now.ToString("o")}\"}}");
            };
            timer.Start();
        }


    }
}
