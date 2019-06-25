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
	public partial class ProfileAddPage : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();

        public ProfileAddPage ()
		{
			InitializeComponent ();
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            databaseManager.AddProfile(ProfileName.Text, Convert.ToInt32(ProfileRFID.Text), ProfilePortion.Text);
            await Navigation.PopToRootAsync();
        }
    }
}