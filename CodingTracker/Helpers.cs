using CodingTracker.Models;
using CodingTracker.View;
using Spectre.Console;
using System.Diagnostics;
using System.Globalization;

namespace CodingTracker;
internal static class Helpers
{
    private static readonly UserInterface userInterface = new();

    internal static List<CodingSession> ParseAnonObjToCodingSession(List<dynamic> listFromDB)
    {
        var list = new List<CodingSession>();

        foreach (var item in listFromDB)
        {
            var newCodingSession = new CodingSession()
            {
                Id = (int)item.Id,
                StartTime = DateTime.ParseExact(item.StartTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
                EndTime = DateTime.ParseExact(item.EndTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US")),
                Duration = TimeSpan.Parse(item.Duration),
            };


            list.Add(newCodingSession);
        }

        return list;
    }

    internal static string GetDateInput(string message)
    {
        var dateInput = AnsiConsole.Prompt(
            new TextPrompt<string>(message));

        if (dateInput.ToLower() == "t") return DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        while (!IsFormattedCorrectly(dateInput, "yyyy-MM-dd HH:mm") || String.IsNullOrEmpty(dateInput))
        {
            dateInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Invalid input, try again."));
        }

        return dateInput;
    }

    internal static bool IsFormattedCorrectly(string date, string format)
    {
        if (!DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"), DateTimeStyles.None, out _)){
            return false;
        }

        return true;
    }

    internal static bool IsEndTimeLowerThanStartTime(string startTime, string endTime)
    {
        var sT = DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"));

        var eT = DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"));

        return eT < sT;
    }

    internal static TimeSpan RunStopWatch()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        AnsiConsole.MarkupLine(@"-------- TIME YOUR SESSION ---------
        Press 'q' to quit.
        Press 'p' to pause
        Press 'r' to reset");

        bool isRunning = true;
        bool isPaused = false;

        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.P:
                        Console.SetCursorPosition(0, Console.CursorTop);
                        AnsiConsole.MarkupLine($"Time: {stopwatch.Elapsed.ToString()}");
                        if (isPaused)
                        {
                            stopwatch.Start();
                            isPaused = false;
                        }
                        else
                        {
                            stopwatch.Stop();
                            isPaused = true;
                        }
                        break;

                    case ConsoleKey.Q:
                        isRunning = false;
                        stopwatch.Stop();
                        break;

                    case ConsoleKey.R:
                        stopwatch.Restart();
                        break;
                }
            }

            Thread.Sleep(50);
        }

        return stopwatch.Elapsed;
    }
}
