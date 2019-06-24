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

        }
    }
}