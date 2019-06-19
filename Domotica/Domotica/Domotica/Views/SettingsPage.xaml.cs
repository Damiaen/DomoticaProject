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
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            storedSettings.Ip_config = IP_Adress.Text;
            databaseManager.UpdateSettings(storedSettings);
        }
    }
}