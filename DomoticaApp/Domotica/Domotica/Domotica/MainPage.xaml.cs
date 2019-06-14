using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Domotica.Providers;

namespace Domotica
{
    public partial class MainPage : ContentPage
    {
        AsynchronousClient connection = new AsynchronousClient();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void UpdateSensor_Clicked(object sender, EventArgs e)
        {
            DisplaySensorValue.Text = await connection.SingleAction("a");
        }

        private async void UpdateSensors_Clicked(object sender, EventArgs e)
        {
            await connection.GetSensorValues();
        }
    }
}
