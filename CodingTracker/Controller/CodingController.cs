using CodingTracker.Data;
using CodingTracker.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using static CodingTracker.Enums;
using System.Globalization;

namespace CodingTracker.Controller;
internal class CodingController : BaseController
{
    public void StartSession()
    {
        DisplayMessage("Session started", "blue");
        var startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        var timer = Helpers.RunStopWatch();

        var choice = AnsiConsole.Confirm("Do you want to save your session?");

        if (!choice) return;

        var endTime = timer;

        AddSession(startTime, endTime);
    }

    public void ViewReportOfCodingSession()
    {
        DisplayMessage("Get your coding tracker report!");

        // ask user to choose report based on days, weeks, months, years
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<ReportOption>()
            .Title("Select your report type")
            .AddChoices(Enum.GetValues<ReportOption>())
            );

        string? unit = null;
        if (choice != ReportOption.Total)
        {
            unit = AnsiConsole.Prompt(
                new TextPrompt<string>($"Select the number of {choice} for your report."));
        }

        // send choice as arg on method that gets a sql query
        var listOfReport = GetReport(choice, unit);
        // maybe refactor getSessions method to do it there
        // recieve list
        ViewSessions(listOfReport);
        // do another query to figure out the total and average duration
        // display it

    }

    public List<CodingSession> GetReport(ReportOption choice, string unit)
    {
        var query = $"SELECT * FROM coding_tracker ";
        _ = choice switch
        {
            ReportOption.Days => query += $"WHERE EndTime > date('now', '-{unit} days')",
            ReportOption.Months => query += $"WHERE EndTime > date('now','start of month', '-{unit} months')",
            ReportOption.Years => query += $"WHERE EndTime > date('now','start of year', '-{unit} years')",
            ReportOption.Total => query
        };

        return GetSessions(query);
    }

    public static List<CodingSession>? GetSessions(string? sql = null)
    {
        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());

        if (sql == null) sql = $"SELECT * FROM coding_tracker";

        var listFromDB = connection.Query(sql).ToList();

        if (listFromDB.Count == 0) return null;

        var listOfCodingSessions = Helpers.ParseAnonObjToCodingSession(listFromDB);

        return listOfCodingSessions;
    }
    public void ViewSessions(List<CodingSession>? list = null)
    {
        if (list == null) list = GetSessions();

        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No data found.[/]");
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
            return;
        }

        var table = Helpers.CreateTable(["ID", "Start Time", "End Time", "Duration"], "yellow");

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

    public void AddSession(string? startTime = null, string? endTime = null)
    {
        if (startTime == null || endTime == null)
        {
            startTime = Helpers.GetDateInput("Enter the start time of your coding session (yyyy-MM-dd HH:mm).\nPress 't' to enter actual time.");

            endTime = Helpers.GetDateInput("Enter the end time of your coding session (yyyy-MM-dd HH:mm)\nPress 't' to enter actual time.");
        }

        while (Helpers.IsEndTimeLowerThanStartTime(startTime, endTime))
        {
            endTime = Helpers.GetDateInput("Invalid input. End time must be larger than the start time.");
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
