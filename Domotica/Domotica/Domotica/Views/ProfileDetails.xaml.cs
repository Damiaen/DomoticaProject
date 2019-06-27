using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Domotica.Models;
using Domotica.Services;

namespace Domotica.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfileDetails : ContentPage
	{
        Profile profileDetails;

        DatabaseManager databaseManager = new DatabaseManager();

        public ProfileDetails (Profile selectedProfile)
		{
            InitializeComponent ();

            profileDetails = selectedProfile;
            profileDetailsRFID.Text = "RFID Code: " + profileDetails.RFID.ToString();
            profileDetailsAnimalName.Text = "Naam: " + profileDetails.AnimalName;
            profileDetailsAnimalType.Text = "Type: " + profileDetails.AnimalType;
            profileDetailsPortionSize.Text = "Standaard portie: " + profileDetails.DefaultPortionSize.ToString();

            SchedulesTitle.Text = "Voedertijden van " + profileDetails.AnimalName;
            SchedulesList.ItemsSource = databaseManager.GetAllSchedulesById(profileDetails.Id);

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfileEditPagexaml(profileDetails));
        }

        private async void SchedulesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            Schedule selectedSchedule = e.SelectedItem as Schedule;
            if (selectedSchedule == null) return;

            await Navigation.PushAsync(new ProfileFeedtimesEditPage(selectedSchedule));

            // We deselect the item so that the background is not greyed when we come back
            SchedulesList.SelectedItem = null;
        }

        private async void Add_time_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfileFeedtimesAddPage(profileDetails));
        }
    }
}