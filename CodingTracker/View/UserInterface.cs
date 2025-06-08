using CodingTracker.Controller;
using Spectre.Console;
using static CodingTracker.Enums;

namespace CodingTracker.View;
internal class UserInterface
{
    private readonly CodingController _codingController = new();

    internal void MainMenu()
    {
        bool flag = true;
        while (flag)
        {
            Console.Clear();

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<MenuOption>()
            .Title("MENU")
            .AddChoices(Enum.GetValues<MenuOption>()));

            switch (choice)
            {
                case MenuOption.AddCodingSession:
                    _codingController.AddSession();
                    break;
                case MenuOption.ViewCodingSession:
                    _codingController.ViewSessions();
                    break;
                case MenuOption.DeleteCodingSession:
                    _codingController.DeleteSession();
                    break;
                case MenuOption.Exit:
                    AnsiConsole.MarkupLine("Goodbye");
                    flag = false;
                    break;
                default:
                    AnsiConsole.MarkupLine("Invalid input");
                    break;
            }
        }
        
    }
}
