using Microsoft.Data.Sqlite;
using System.Configuration;

string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText = "";

    tableCmd.ExecuteNonQuery();
}

