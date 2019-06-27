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

        public ProfileAddPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(ProfileName.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een naam in", "OK");
            } else if (String.IsNullOrEmpty(AnimalType.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een type dier in", "OK");
            } else if (String.IsNullOrEmpty(ProfilePortion.Text))
            {
                await DisplayAlert("Leeg veld", "Voer een portiegrootte in", "OK");
            } else if (ProfileRFID.Text == null || ProfileRFID.Text == " ")
            {
                ProfileRFID.Text = "0";
            } else
            {
                databaseManager.AddProfile(ProfileName.Text, Convert.ToInt32(ProfileRFID.Text), Convert.ToInt32(ProfilePortion.Text), AnimalType.Text);
                await Navigation.PopToRootAsync();
            }

        }
    }
}