using CodingTracker.Data;
using CodingTracker.Models;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Numerics;

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
                    Date = DateTime.ParseExact(reader.GetString(1), "yyyy-MM-dd", new CultureInfo("en-US")),
                    StartTime = DateTime.ParseExact(reader.GetString(2), "HH:mm", new CultureInfo("en-US")),
                    EndTime = DateTime.ParseExact(reader.GetString(3), "HH:mm", new CultureInfo("en-US")),
                });
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Date[/]");
        table.AddColumn("[yellow]Start Time[/]");
        table.AddColumn("[yellow]End Time[/]");
        table.AddColumn("[yellow]Duration[/]");

        foreach (var session in sessions)
        {
            table.AddRow(
                session.Id.ToString(),
                $"[cyan]{session.Date}[/]",
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
        string startDate = Helpers.ValidateDateinput(@"Enter the start date of your coding session (Format: yyyy-MM-dd HH:mm)");

        string endDate = Helpers.ValidateDateinput(@"Enter the end date of your coding session (Format: yyyy-MM-dd HH:mm)");

        using var connection = new SqliteConnection(DatabaseInitializer.GetConnectionString());
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
            @$"INSERT INTO {DatabaseInitializer.GetDBPath()} (date, startTime, endTime, duration)
               VALUES (@Date, @StartTime, @EndTime, @Duration)";




    }
}
