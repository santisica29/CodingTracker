using CodingTracker.Models;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker.Controller;
internal class CodingController
{
    public void GetAllSessions()
    {
        using var connection = new SqliteConnection(Helpers.GetConnectionString());
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
                    StartTime = DateTime.ParseExact(reader.GetString(1), "yyyy-MM-dd", new CultureInfo("en-US")),
                    EndTime = DateTime.ParseExact(reader.GetString(2), "yyyy-MM-dd", new CultureInfo("en-US")),
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
}
