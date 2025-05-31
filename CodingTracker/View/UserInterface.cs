using CodingTracker.Controller;
using Spectre.Console;
using static CodingTracker.Enums;

namespace CodingTracker.View;
internal class UserInterface
{
    private readonly CodingController _codingController = new();

    internal void MainMenu()
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
                _codingController.GetAllSessions();
                break;
            case MenuOption.DeleteCodingSession:
                break;
            default:
                AnsiConsole.MarkupLine("anoh");
                break;
        }
    }
}
