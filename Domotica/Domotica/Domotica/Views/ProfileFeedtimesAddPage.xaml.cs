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
        int portionSize;
        string description;

        public ProfileFeedtimesAddPage (Profile selectedProfile)
		{
			InitializeComponent ();
            profile = selectedProfile;
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (DatePickerFeedTimes.Date == null || timePicker.Time == null)
            {
                await DisplayAlert("Leeg veld", "Vul alle voedertijden in", "OK");
            }
            else
            {
                if (String.IsNullOrEmpty(schedulePortionSize.Text))
                {
                    portionSize = profile.DefaultPortionSize;
                }
                if (String.IsNullOrEmpty(scheduleDescription.Text))
                {
                    description = "Voederdatum";
                }
                portionSize = Convert.ToInt32(schedulePortionSize.Text);
                databaseManager.AddSchedule(profile.Id, description, portionSize, DatePickerFeedTimes.Date.ToString("dd/MM/yyyy"), timePicker.Time.ToString());
                await Navigation.PopToRootAsync();
            }
            
        }
    }
}