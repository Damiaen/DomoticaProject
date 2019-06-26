using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Domotica.Services;
using Domotica.Models;

namespace Domotica.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();
        Settings storedSettings;

        public SettingsPage ()
		{
			InitializeComponent ();
            storedSettings = databaseManager.GetSettings();
            IP_Adress.Text = storedSettings.Ip_config;
            AutoUpdateSensors.IsToggled = Convert.ToBoolean(storedSettings.Auto_update);
            LogUsageRFID.IsToggled = Convert.ToBoolean(storedSettings.RFID_logs);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            storedSettings.Ip_config = IP_Adress.Text;
            storedSettings.Auto_update = Convert.ToInt32(AutoUpdateSensors.IsToggled);
            storedSettings.RFID_logs = Convert.ToInt32(LogUsageRFID.IsToggled);
            databaseManager.UpdateSettings(storedSettings);
        }
    }
}