using Microsoft.Data.Sqlite;
using System.Configuration;

string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

CreateDatabase();

void CreateDatabase()
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText =
        @"CREATE TABLE IF NOT EXISTS coding_tracker (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            StartTime TEXT,
            EndTime TEXT,
            Duration INTEGER
            )";

    tableCmd.ExecuteNonQuery();
}

