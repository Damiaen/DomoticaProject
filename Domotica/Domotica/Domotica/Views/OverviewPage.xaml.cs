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

        public OverviewPage ()
		{
			InitializeComponent ();
		}

        private async void UpdateSensor_Clicked(object sender, EventArgs e)
        {
            DisplaySensorValue.Text = await connection.SingleAction("a");
        }

        private async void UpdateSensors_Clicked(object sender, EventArgs e)
        {
            await connection.GetSensorValues();
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
        }
    }
}