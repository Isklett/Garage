using Garage.ValueTypes;

namespace Garage.Interfaces
{
    public interface IConsoleUI
    {
        void ShowMessage(string message);
        void ShowWarning(string warningMessage);
        void ShowError(string errorMessage);
        string GetStringInput(string prompt);
        int GetIntInput(string prompt);
        float GetFloatInput(string prompt);
        double GetDoubleInput(string prompt);
        int? GetChoiceInput(string prompt, IReadOnlyList<string> choices, string errorMessage, int maxChoices = int.MaxValue, int maxRows = 5);
        SearchParameters.VehicleSearch GetMultipleLines(string[] inPrompts, string lastChoice);
        ConsoleKey GetKey();
        void Draw(string[]? options, List<string> data);
        void ClearScreen(int row);
    }
}
