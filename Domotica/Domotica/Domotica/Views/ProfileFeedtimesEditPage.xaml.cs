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
	public partial class ProfileFeedtimesEditPage : ContentPage
	{
        DatabaseManager databaseManager = new DatabaseManager();

        Schedule schedule;

		public ProfileFeedtimesEditPage (Schedule selectedSchedule)
		{
			InitializeComponent ();
            schedule = selectedSchedule;
            ScheduleInfoEdit.Text = schedule.ScheduleInfo;

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            databaseManager.DeleteSchedule(schedule);
            await Navigation.PopToRootAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            schedule.ScheduleInfo = ScheduleInfoEdit.Text;
            databaseManager.UpdateSchedule(schedule);
            await Navigation.PopToRootAsync();
        }
    }
}