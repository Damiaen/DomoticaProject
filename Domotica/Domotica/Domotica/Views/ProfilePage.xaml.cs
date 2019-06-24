using Domotica.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Domotica.Models;

namespace Domotica.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();

        public ProfilePage ()
		{
            InitializeComponent();
            Profiles.ItemsSource = databaseManager.GetAllProfiles();
        }

        async void Profiles_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            Profile selectedProfile = e.SelectedItem as Profile;
            if (selectedProfile == null) return;

            await Navigation.PushAsync(new ProfileDetails(selectedProfile));

            // We deselect the item so that the background is not greyed when we come back
            Profiles.SelectedItem = null;
        }

        async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfileAddPage());
        }
    }
}