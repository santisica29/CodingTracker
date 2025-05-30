using Microsoft.Data.Sqlite;
using System.Configuration;
using Spectre.Console;
using static CodingTracker.Enums;

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
        new SelectionPrompt<MenuOption>()
        .Title("MENU")
        .AddChoices(Enum.GetValues<MenuOption>()));

    switch (choice)
    {
        case MenuOption.AddCodingSession:
            //AnsiConsole.MarkupLine();
            break;
        case MenuOption.ViewCodingSession:
            break;
        case MenuOption.DeleteCodingSession:
            break;
        default:
            AnsiConsole.MarkupLine("anoh");
            break;
    }
}

