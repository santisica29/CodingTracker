using CodingTracker.Data;
using CodingTracker.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker.Controller;
internal class CodingController : BaseController, IBaseController
{
    public static List<CodingSession>? GetSessions()
    {
        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());

        var sql = $"SELECT * FROM coding_tracker";

        var listFromDB = connection.Query(sql).ToList();

        if (listFromDB.Count == 0) return null;

        var listOfCodingSessions = Helpers.ParseAnonObjToCodingSession(listFromDB);

        return listOfCodingSessions;
    }
    public void ViewSessions()
    {
        var list = GetSessions();

        if (list == null)
        {
            AnsiConsole.MarkupLine("[red]No data found.[/]");
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Start Time[/]");
        table.AddColumn("[yellow]End Time[/]");
        table.AddColumn("[yellow]Duration[/]");

        foreach (var session in list)
        {
            table.AddRow(
                $"{session.Id}",
                $@"[cyan]{session.StartTime.ToString("dd-MMM-yy HH:mm")}[/]",
                $@"[cyan]{session.EndTime.ToString("dd-MMM-yy HH:mm")}[/]",
                $"[cyan]{session.Duration.ToString()}[/]"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();
    }

    public void AddSession()
    {
        string startTime = Helpers.GetDateInput("Enter the start time of your coding session (yyyy-MM-dd HH:mm)");

        string endTime = Helpers.GetDateInput("Enter the end time of your coding session (yyyy-MM-dd HH:mm)");

        while (Helpers.IsEndTimeLowerThanStartTime(startTime, endTime))
        {
            endTime = Helpers.GetDateInput("Invalid input. End time must be after start time.");
        }

        var session = new CodingSession(
            DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
            DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"))
        );

        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        var sql =
            @$"INSERT INTO {DatabaseInitializer.GetDBName()} (startTime, endTime, duration)
               VALUES (@StartTime, @EndTime, @Duration)";

        var affectedRows = connection.Execute(sql, new { StartTime = startTime, EndTime = endTime, Duration = session.CalculateDuration().ToString()});

        if (affectedRows > 0) DisplayMessage("Addition completed.", "green");
        else DisplayMessage("No changes made");

        AnsiConsole.MarkupLine("Press any key to continue.");
        Console.ReadKey();
    }

    public void DeleteSession()
    {
        Console.Clear();
        var list = GetSessions();

        if (list == null)
        {
            DisplayMessage("No sessions recorded.", "red");
            Console.ReadKey();
            return;
        }

        var sessionToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<CodingSession>()
            .Title("Select a [red]session[/] to delete:")
            .UseConverter(s => $"{s.Id} - {s.StartTime} - {s.EndTime} - {s.Duration}")
            .AddChoices(list));

        if (ConfirmDeletion(sessionToDelete.ToString()))
        {
            using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
            var sql = $@"DELETE from {DatabaseInitializer.GetDBName()} WHERE Id = @Id";

            var affectedRows = connection.Execute(sql, new { Id = sessionToDelete.Id });

            if (affectedRows > 0) DisplayMessage("Deletion completed.", "green");
            else DisplayMessage("No changes made");
        }
        else
        {
            DisplayMessage("Deletion canceled", "yellow");
        }

        AnsiConsole.MarkupLine("Press any key to continue.");
        Console.ReadKey();
    }

    public void UpdateSession()
    {
        Console.Clear();
        var list = GetSessions();

        if (list == null)
        {
            DisplayMessage("No sessions recorded.", "red");
            Console.ReadKey();
            return;
        }

        var sessionToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<CodingSession>()
            .Title("Select a [red]session[/] to delete:")
            .UseConverter(s => $"{s.Id} - {s.StartTime} - {s.EndTime} - {s.Duration}")
            .AddChoices(list));

        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        var sql = @$"UPDATE {DatabaseInitializer.GetDBName()} 
                    SET StartTime = @NewStartTime, 
                    EndTime = @NewEndTime,
                    Duration = @Duration
                    WHERE Id = @Id";

        var newStartTime = Helpers.GetDateInput("Enter the start time of your coding session (yyyy-MM-dd HH:mm)");
        var newEndTime = Helpers.GetDateInput("Enter the end time of your coding session (yyyy-MM-dd HH:mm)");
        while (Helpers.IsEndTimeLowerThanStartTime(newStartTime, newEndTime))
        {
            newEndTime = Helpers.GetDateInput("Invalid input. End time must be after start time.");
        }

        var newSession = new CodingSession(
            DateTime.ParseExact(newStartTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
            DateTime.ParseExact(newEndTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"))
        );

        var affectedRows = connection.Execute(sql, new { Id = sessionToUpdate.Id, NewStartTime = newStartTime, NewEndTime = newEndTime, Duration = newSession.CalculateDuration().ToString() });

        if (affectedRows > 0) DisplayMessage("Update successful.", "green");
        else DisplayMessage("No changes made");

        AnsiConsole.MarkupLine("Press any key to continue.");
        Console.ReadKey();
    }
}
