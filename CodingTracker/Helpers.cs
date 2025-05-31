using CodingTracker.View;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker;
internal static class Helpers
{
    private static readonly UserInterface userInterface = new();

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
