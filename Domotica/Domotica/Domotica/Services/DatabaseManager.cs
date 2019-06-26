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

        public Settings GetSettings()
        {
            return Connection.FindWithQuery<Settings>("SELECT * FROM Settings WHERE Settings_id = 1");
        }

        public void UpdateSettings(Settings settings)
        {
            Connection.Update(settings);
        }

        public List<Profile> GetAllProfiles()
        {
            return Connection.Query<Profile>("SELECT * FROM Profile");
        }

        public void UpdateProfile(Profile profile)
        {
            Connection.Update(profile);
        }

        public void AddProfile(string ProfileName, int ProfileRFID, int defaultportionsize, string AnimalType)
        {
            Connection.Insert(new Profile {AnimalName = ProfileName, RFID = ProfileRFID, DefaultPortionSize = defaultportionsize, AnimalType = AnimalType });
        }

        public void AddSchedule(int profileId, string description, int portionsize, string feeddate, string feedtime)
        {
            Connection.Insert(new Schedule { ProfileId = profileId, Description = description, PortionSize = portionsize, FeedDate = feeddate, FeedTime = feedtime, Expired = 0 });
        }

        public List<Schedule> GetAllSchedulesById(int profileId)
        {
            return Connection.Query<Schedule>("SELECT * FROM Schedule WHERE ProfileId = ?", profileId);
        }

        public List<Schedule> GetAllNonExpiredSchedules(string currentDate)
        {
            return Connection.Query<Schedule>("SELECT * FROM Schedule WHERE Expired = 0");
        }

        public void UpdateSchedule(Schedule schedule)
        {
            Connection.Update(schedule);
        }

        public void DeleteSchedule(Schedule schedule)
        {
            Connection.Delete(schedule);
        }

        //public List<Movie> GetAllMovies()
        //{
        //    return Connection.Query<Movie>("SELECT * FROM Movie");
        //}

        //public List<Serie> GetAllSeries()
        //{
        //    return Connection.Query<Serie>("SELECT * FROM Serie");
        //}

        //public List<Movie> GetAllMoviesByUser(string username)
        //{
        //    return Connection.Query<Movie>("SELECT Movie.* FROM Movie, MovieUser, User WHERE Username = ? AND MovieUser.UserID = User.UserID AND Movie.MovieID = MovieUser.MovieID", username);
        //}

        //public List<Serie> GetAllSeriesByUser(string username)
        //{
        //    return Connection.Query<Serie>("SELECT Serie.* FROM Serie, SerieUser, User WHERE Username = ? AND SerieUser.UserID = User.UserID AND Serie.SerieID = SerieUser.SerieID", username);
        //}

        //public void AddMovie(string title, string description, int year)
        //{
        //    Connection.Insert(new Movie { Title = title, Description = description, Year = year });
        //}

        //public void AddSerie(string title, string description, int yearStarted, int yearEnded, int numberOfEpisodes, int numberOfSeasons)
        //{
        //    Connection.Insert(new Serie { Title = title, Description = description, YearStarted = yearStarted, YearEnded = yearEnded, numberOfEpisodes = numberOfEpisodes, NumberOfSeasons = numberOfSeasons });
        //}

        //public void AddUser(string username, string password)
        //{
        //    Connection.Insert(new User { Username = username, Password = password });
        //}

        //public bool DoesUserExist(string username, string password)
        //{
        //    return Connection.Query<User>("SELECT * FROM User WHERE Username = ? AND Password = ?", username, password).Count > 0;
        //}

        //public void UserLogin(string username)
        //{
        //    Connection.Execute("UPDATE User SET LoggedIn = 1 WHERE Username = ?", username);
        //}

        //public void UserLogout()
        //{
        //    Connection.Execute("UPDATE User SET LoggedIn = 0");
        //}
    }
}