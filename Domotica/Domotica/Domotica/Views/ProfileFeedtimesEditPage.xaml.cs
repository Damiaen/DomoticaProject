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
            scheduleDescription.Text = schedule.Description;
            DatePickerFeedTimes.Date = DateTime.Parse(schedule.FeedDate);
            timePicker.Time = TimeSpan.Parse(schedule.FeedTime);
            schedulePortionSize.Text = schedule.PortionSize.ToString();

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            databaseManager.DeleteSchedule(schedule);
            await Navigation.PopToRootAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(scheduleDescription.Text))
            {
                scheduleDescription.Text = "Voederdatum";
            }
            else if (String.IsNullOrEmpty(schedulePortionSize.Text))
            {
                schedulePortionSize.Text = schedule.PortionSize.ToString();
            }
            else if (DatePickerFeedTimes.Date == null)
            {
                await DisplayAlert("Leeg veld", "Voer een voerdatum in", "OK");
            }
            else if (timePicker.Time == null)
            {
                await DisplayAlert("Leeg veld", "Voer een voertijd in", "OK");
            }
            else
            {
                schedule.Description = scheduleDescription.Text;
                schedule.FeedTime = DatePickerFeedTimes.Date.ToString("dd/MM/yyyy");
                schedule.FeedTime = timePicker.Time.ToString();
                schedule.PortionSize = Convert.ToInt32(schedulePortionSize.Text);

                databaseManager.UpdateSchedule(schedule);
                await Navigation.PopToRootAsync();
            }

        }
    }
}