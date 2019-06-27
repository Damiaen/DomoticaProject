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
            UpdateSensor();

        }

        private async void UpdateSensor()
        {
            string text = await connection.SingleAction("b");
            int temp = 100 - (Int32.Parse(text) * 100) / 25;
            DisplaySensorValue.Text = temp.ToString() + "%";
        }

        private async void UpdateSensorsManually_Clicked(object sender, EventArgs e)
        {
            string text = await connection.SingleAction("b");
            int temp = 100 - (Int32.Parse(text) * 100) / 25;
            DisplaySensorValue.Text = temp.ToString() + "%";
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

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string text = await connection.SingleAction("b");
            int temp = 100 - (Int32.Parse(text) * 100) / 24;
            DisplaySensorValue.Text = temp.ToString() + "%";
        }
    }
}