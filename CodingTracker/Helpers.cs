using CodingTracker.Models;
using CodingTracker.View;
using Spectre.Console;
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

    internal static string ValidateDateinput(string message)
    {
        AnsiConsole.MarkupLine("");
        var dateInput = AnsiConsole.Prompt(
            new TextPrompt<string>(message));


        //check if its the same mainmenu
        if (dateInput == "0") userInterface.MainMenu();
        if (dateInput == "t") return DateTime.Now.ToString("yyyy-MM-dd");

        while (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            AnsiConsole.MarkupLine("Invalid input");
            dateInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Please insert the date: (Format: yyyy - MM - dd).Type t to enter today's date or 0 to return to main menu"));
        }

        return dateInput;
    }
}
