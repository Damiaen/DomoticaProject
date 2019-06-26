using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Domotica.Views;
using System.Diagnostics;
using Domotica.Services;
using Domotica.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Domotica
{
    public partial class App : Application
    {
        private static Stopwatch stopWatch = new Stopwatch();
        private AsynchronousClient connection = new AsynchronousClient();
        private static DatabaseManager databaseManager = new DatabaseManager();
        private const int defaultTimespan = 1;

        public App()
        {
            InitializeComponent();


            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            // On start runs when your application launches from a closed state, 

            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
            }

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                // Logic for logging out if the device is inactive for a period of time.

                if (stopWatch.IsRunning && stopWatch.Elapsed.Minutes >= defaultTimespan)
                {
                    // Prepare to perform your data pull here as we have hit the 1 minute mark   

                    Settings storedSettings = databaseManager.GetSettings();

                    if (Convert.ToBoolean(storedSettings.Auto_update))
                    {
                        // check date stuff
                        DateTime dateTime = DateTime.Today;

                        List<Schedule> activeSchedules = databaseManager.GetAllNonExpiredSchedules(dateTime.ToString("dd/MM/yyyy"));

                        foreach (Schedule activeSchedule in activeSchedules)
                        {
                            DateTime date1 = DateTime.Parse(dateTime.ToString("hh:mm:ss"));
                            DateTime date2 = DateTime.Parse(activeSchedule.FeedTime);
                            int result = DateTime.Compare(date1, date2);

                            if (result < 0 || result == 0)
                            {
                                Console.WriteLine("is earlier than current time, send command to feeder");
                                Task.Run(async () => { await connection.SingleAction(activeSchedule.PortionSize.ToString() + ";"); }).Wait();
                                Console.WriteLine("Food dispensed, updating database");
                                activeSchedule.Expired = 1;
                                databaseManager.UpdateSchedule(activeSchedule);
                            } else
                            {
                                Console.WriteLine("is later than current time, do nothing");
                            }     
                        }
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        // If you need to do anything with your UI, you need to wrap it in this.
                    });

                    stopWatch.Restart();
                }

                // Always return true as to keep our device timer running.
                return true;
            });
        }

        protected override void OnSleep()
        {
            // Ensure our stopwatch is reset so the elapsed time is 0.
            stopWatch.Reset();
        }

        protected override void OnResume()
        {
            // App enters the foreground so start our stopwatch again.
            stopWatch.Start();
        }
    }
}
