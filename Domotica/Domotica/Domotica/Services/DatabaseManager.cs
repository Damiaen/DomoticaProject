using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;
using Domotica.Models;

namespace Domotica.Services
{
    public class DatabaseManager
    {
        readonly SQLiteConnection Connection = DependencyService.Get<IDBInterface>().CreateConnection();
        public DatabaseManager()
        {
        }

        /// <summary>
        /// Gets settings from database
        /// </summary>
        /// <returns>settings</returns>
        public Settings GetSettings()
        {
            return Connection.FindWithQuery<Settings>("SELECT * FROM Settings WHERE Settings_id = 1");
        }

        /// <summary>
        /// Updates settings with edited info
        /// </summary>
        /// <param name="settings">settings</param>
        public void UpdateSettings(Settings settings)
        {
            Connection.Update(settings);
        }

        /// <summary>
        /// Gets all profiles from database
        /// </summary>
        /// <returns>profiles</returns>
        public List<Profile> GetAllProfiles()
        {
            return Connection.Query<Profile>("SELECT * FROM Profile");
        }

        /// <summary>
        /// Updates profile with edited info
        /// </summary>
        /// <param name="profile">profile</param>
        public void UpdateProfile(Profile profile)
        {
            Connection.Update(profile);
        }

        /// <summary>
        /// Delete profile 
        /// </summary>
        /// <param name="profile">selected profile</param>
        public void DeleteProfile(Profile profile)
        {
            Connection.Delete(profile);
        }

        /// <summary>
        /// Adds new profile to the database
        /// </summary>
        /// <param name="ProfileName">Profilename</param>
        /// <param name="ProfileRFID">RFID of animal/profile</param>
        /// <param name="defaultportionsize">Default portion size for feeding</param>
        /// <param name="AnimalType">Animaltype</param>
        public void AddProfile(string ProfileName, int ProfileRFID, int defaultportionsize, string AnimalType)
        {
            Connection.Insert(new Profile {AnimalName = ProfileName, RFID = ProfileRFID, DefaultPortionSize = defaultportionsize, AnimalType = AnimalType });
        }

        /// <summary>
        /// Add new schedule to the database
        /// </summary>
        /// <param name="profileId">Profile id of selected profile</param>
        /// <param name="description">description of schedule</param>
        /// <param name="portionsize">portionsize of food</param>
        /// <param name="feeddate">date animal has to be fed on</param>
        /// <param name="feedtime">time that the animal is to get fed</param>
        public void AddSchedule(int profileId, string description, int portionsize, string feeddate, string feedtime)
        {
            Connection.Insert(new Schedule { ProfileId = profileId, Description = description, PortionSize = portionsize, FeedDate = feeddate, FeedTime = feedtime, Expired = 0 });
        }

        /// <summary>
        /// Get all schedules by profile id
        /// </summary>
        /// <param name="profileId">profileid of selected profile</param>
        /// <returns>schedules</returns>
        public List<Schedule> GetAllSchedulesById(int profileId)
        {
            return Connection.Query<Schedule>("SELECT * FROM Schedule WHERE ProfileId = ?", profileId);
        }

        /// <summary>
        /// Get all non expired schedules
        /// </summary>
        /// <param name="currentDate">time at this moment</param>
        /// <returns>schedules</returns>
        public List<Schedule> GetAllNonExpiredSchedules(string currentDate)
        {
            return Connection.Query<Schedule>("SELECT * FROM Schedule WHERE Expired = 0");
        }


        /// <summary>
        /// Update schedules with edited infos
        /// </summary>
        /// <param name="schedule">schedule</param>
        public void UpdateSchedule(Schedule schedule)
        {
            Connection.Update(schedule);
        }

        /// <summary>
        /// Delete selected schedule
        /// </summary>
        /// <param name="schedule"></param>
        public void DeleteSchedule(Schedule schedule)
        {
            Connection.Delete(schedule);
        }

    }
}