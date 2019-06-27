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
	public partial class ProfileEditPagexaml : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();

        Profile profile;

        public ProfileEditPagexaml (Profile selectedProfile)
		{
			InitializeComponent ();
            profile = selectedProfile;

            EditProfileName.Text = profile.AnimalName;
            EditAnimalType.Text = profile.AnimalType;
            EditProfileRFID.Text =  profile.RFID.ToString();
            EditProfilePortion.Text = profile.DefaultPortionSize.ToString();

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EditProfileName.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een naam in", "OK");
            }
            else if (String.IsNullOrEmpty(EditAnimalType.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een type dier in", "OK");
            }
            else if (String.IsNullOrEmpty(EditProfilePortion.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een portiegrootte in", "OK");
            }
            else if (EditProfileRFID.Text == null || EditProfileRFID.Text == " ")
            {
                EditProfileRFID.Text = "0";
            }
            else
            {
                profile.AnimalName = EditProfileName.Text;
                profile.AnimalType = EditAnimalType.Text;
                profile.RFID = Convert.ToInt32(EditProfileRFID.Text);
                profile.DefaultPortionSize = Convert.ToInt32(EditProfilePortion.Text);

                databaseManager.UpdateProfile(profile);
                await Navigation.PopToRootAsync();
            }
 
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            databaseManager.DeleteProfile(profile);
            await Navigation.PopToRootAsync();
        }
    }
}