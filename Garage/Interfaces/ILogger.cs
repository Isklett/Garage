namespace Garage.Interfaces
{
    public interface ILogger
    {
        string LogFilePath { get; }
        void LogMessage(string message);
        void LogWarning(string warningMessage);
        void LogError(string errorMessage);
    }
}
