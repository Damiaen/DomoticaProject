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
            EditProfileRFID.Text = profile.DefaultPortionSize.ToString();
            EditProfilePortion.Text = profile.RFID.ToString();

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            profile.AnimalName = EditProfileName.Text;
            profile.AnimalType = EditAnimalType.Text;
            profile.DefaultPortionSize = Convert.ToInt32(EditProfileRFID.Text);
            profile.RFID = Convert.ToInt32(EditProfilePortion.Text);

            databaseManager.UpdateProfile(profile);
            await Navigation.PopToRootAsync();
        }
    }
}