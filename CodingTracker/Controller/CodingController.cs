using CodingTracker.Data;
using CodingTracker.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker.Controller;
internal class CodingController : BaseController, IBaseController
{
    public List<CodingSession>? GetSessions()
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
                $"[cyan]{session.StartTime}[/]",
                $"[cyan]{session.EndTime}[/]",
                $"[cyan]{session.Duration.ToString()}[/]"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();
    }

    public void AddSession()
    {
        string startTime = Helpers.ValidateDateinput(@"Enter the start time of your coding session (HH:mm)");

        string endTime = Helpers.ValidateDateinput(@"Enter the end time of your coding session (HH:mm)");

        var session = new CodingSession(DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")), DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")));

        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        var sql =
            @$"INSERT INTO {DatabaseInitializer.GetDBPath()} (startTime, endTime, duration)
               VALUES (@StartTime, @EndTime, @Duration)";

        var affectedRows = connection.Execute(sql, new {StartTime = startTime, EndTime = endTime, Duration = session.CalculateDuration().ToString()});
    }

    public void DeleteSession()
    {
        Console.Clear();
        var list = GetSessions();



        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        var sql = $@"DELETE from {DatabaseInitializer.GetDBPath()} WHERE Id = @Id";
    }
}
