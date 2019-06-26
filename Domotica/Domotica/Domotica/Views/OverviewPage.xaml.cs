using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Domotica.Services;

namespace Domotica.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OverviewPage : ContentPage
    {
        AsynchronousClient connection = new AsynchronousClient();

        public OverviewPage()
        {
            InitializeComponent();
            // UpdateSensor();

        }

        private async void UpdateSensor()
        {
            string[] sensorArray = await connection.GetSensorValues() as string[];
            string array = sensorArray[1];
            DisplaySensorValue.Text = sensorArray[1];
        }

        async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
        private async void ThrowFood_Pressed(object sender, EventArgs e)
        {
            await connection.SingleAction("i");
        }

        private async void ThrowFood_Released(object sender, EventArgs e)
        {
            await connection.SingleAction("i");
            UpdateSensor();
        }

        private void Interval_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            DisplayInterval.Text = Interval.Value.ToString();
        }

        private async void SendInterval_Clicked(object sender, EventArgs e)
        {
            await connection.SingleAction(Interval.Value.ToString() + ";");
            UpdateSensor();
        }

    }
}