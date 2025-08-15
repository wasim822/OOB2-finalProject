using Microsoft.Data.Sqlite;
using YourApp.Models;

namespace YourApp.Services
{
    public class ReservationService
    {
        private readonly string _dbPath;

        public ReservationService(string dbPath)
        {
            _dbPath = dbPath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Reservations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT,
                    LastName TEXT,
                    Email TEXT,
                    Time TEXT
                );";
            tableCmd.ExecuteNonQuery();
        }

        public void AddReservation(Reservation reservation)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText =
                @"INSERT INTO Reservations (FirstName, LastName, Email, Time)
                  VALUES ($first, $last, $email, $time);";
            insertCmd.Parameters.AddWithValue("$first", reservation.FirstName);
            insertCmd.Parameters.AddWithValue("$last", reservation.LastName);
            insertCmd.Parameters.AddWithValue("$email", reservation.Email);
            insertCmd.Parameters.AddWithValue("$time", reservation.Time.ToString("o"));
            insertCmd.ExecuteNonQuery();
        }

        public List<Reservation> GetReservations()
        {
            var reservations = new List<Reservation>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"SELECT Id, FirstName, LastName, Email, Time FROM Reservations;";
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                reservations.Add(new Reservation
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    Time = DateTime.Parse(reader.GetString(4))
                });
            }
            return reservations;
        }
    }
}