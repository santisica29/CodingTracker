using Microsoft.Data.Sqlite;

namespace CodingTracker.Data;
internal static class DatabaseInitializer
{
    internal static void CreateDatabase()
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
}
