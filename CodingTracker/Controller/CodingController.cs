using CodingTracker.Data;
using CodingTracker.Models;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace CodingTracker.Controller;
internal class CodingController
{
    public void GetAllSessions()
    {
        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT * FROM coding_tracker";

        List<CodingSession> sessions = new();

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.HasRows)
        {
            AnsiConsole.MarkupLine("[red]No data found.[/]");
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
            return;
        }

        while (reader.Read())
        {
            sessions.Add(
                new CodingSession
                {
                    Id = reader.GetInt32(0),
                    StartTime = DateTime.ParseExact(reader.GetString(1), "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
                    EndTime = DateTime.ParseExact(reader.GetString(2), "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
                    Duration = TimeSpan.ParseExact(reader.GetString(3)),
                });
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Start Time[/]");
        table.AddColumn("[yellow]End Time[/]");
        table.AddColumn("[yellow]Duration[/]");

        foreach (var session in sessions)
        {
            table.AddRow(
                session.Id.ToString(),
                $"[cyan]{session.StartTime}[/]",
                $"[cyan]{session.EndTime}[/]",
                $"[cyan]{session.Duration}[/]"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();
    }

    internal void Insert()
    {
        string startTime = Helpers.ValidateDateinput(@"Enter the start time of your coding session (HH:mm)");

        string endTime = Helpers.ValidateDateinput(@"Enter the end time of your coding session (HH:mm)");

        var session = new CodingSession(DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")), DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")));

        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
            @$"INSERT INTO {DatabaseInitializer.GetDBPath()} (startTime, endTime, duration)
               VALUES (@StartTime, @EndTime, @Duration)";

        tableCmd.Parameters.Add("@StartTime", SqliteType.Text).Value = startTime;
        tableCmd.Parameters.Add("@EndTime", SqliteType.Text).Value = endTime;
        tableCmd.Parameters.Add("@Duration", SqliteType.Text).Value = session.CalculateDuration();

        tableCmd.ExecuteNonQuery();
    }
}
