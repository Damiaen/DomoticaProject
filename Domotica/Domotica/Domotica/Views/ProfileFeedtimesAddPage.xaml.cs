using Domotica.Models;
using Domotica.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Domotica.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfileFeedtimesAddPage : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();

        Profile profile;

        public ProfileFeedtimesAddPage (Profile selectedProfile)
		{
			InitializeComponent ();
            profile = selectedProfile;
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {

            databaseManager.AddSchedule(profile.Id, scheduleDescription.Text, Convert.ToInt32(schedulePortionSize.Text), DatePickerFeedTimes.Date.ToString("dd/MM/yyyy"), timePicker.Time.ToString());
            await Navigation.PopToRootAsync();
        }
    }
}