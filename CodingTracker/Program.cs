using Microsoft.Data.Sqlite;
using CodingTracker;
using CodingTracker.View;

CreateDatabase();
UserInterface userInterface = new();
userInterface.SeeMenu();

void CreateDatabase()
{
    using var connection = new SqliteConnection(Helpers.GetConnectionString());
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



