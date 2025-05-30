using Microsoft.Data.Sqlite;
using System.Configuration;
using Spectre.Console;

string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

CreateDatabase();
UserInput();

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

void UserInput()
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("MENU")
        .AddChoices(["Track My Code", "See Previous Coding Sessions", "Delete Coding Sessions"]));

    switch (choice)
    {
        case "T":
            AnsiConsole.MarkupLine(choice);
            break;
        default:
            AnsiConsole.MarkupLine("anoh");
            break;
    }
}

