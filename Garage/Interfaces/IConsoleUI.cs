namespace Garage.Interfaces
{
    public interface IConsoleUI
    {
        void ShowMessage(string message);
        void ShowWarning(string warningMessage);
        void ShowError(string errorMessage);
        ConsoleKey GetKey();
        void Draw(string[]? options, List<string> data);
    }
}
