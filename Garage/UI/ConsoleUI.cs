using Garage.Interfaces;

namespace Garage.UI
{
    internal class ConsoleUI : IConsoleUI, ILogger
    {
        public string LogFilePath => "log.txt";

        public void LogError(string errorMessage)
        {
            File.AppendAllText(LogFilePath, "Error: " + errorMessage + Environment.NewLine);
        }

        public void LogMessage(string message)
        {
            File.AppendAllText(LogFilePath, "Message: " + message + Environment.NewLine);
        }

        public void LogWarning(string warningMessage)
        {
            File.AppendAllText(LogFilePath, "Warning: " + warningMessage + Environment.NewLine);
        }

        public void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void ShowWarning(string warningMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warningMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Draw(string[]? options, List<string> table)
        {
            Console.Clear();
            if(options != null)
            {
                foreach (string s in options)
                {
                    Console.Write(s + "     ");
                }
                Console.WriteLine();
            }
           
            DrawColumns(table, padding: 4);
        }

        public ConsoleKey GetKey() => Console.ReadKey(intercept: true).Key;

        // Each item gets the same column width (longest item + padding)
        // Number of columns is calculated from Console.WindowWidth
        public void DrawColumns(List<string> items, int padding = 2)
        {
            if (items == null || items.Count == 0)
            {
                Console.WriteLine("");
                return;
            }

            int maxLen = items.Max(s => s?.Length ?? 0);
            int colWidth = Math.Max(1, maxLen + padding); // total width per column

            int consoleWidth;
            try
            {
                consoleWidth = Console.WindowWidth;
            }
            catch
            {
                consoleWidth = colWidth; //fallback
            }

            int columns = Math.Max(1, consoleWidth / colWidth);
            columns = Math.Min(columns, items.Count);

            for (int i = 0; i < items.Count; i += columns)
            {
                var row = items.Skip(i).Take(columns);
                foreach (var cell in row)
                {
                    string text = cell ?? "";
                    // If text longer than colWidth, it will extend; optionally truncate:
                    if (text.Length > colWidth - padding)
                        text = text.Substring(0, colWidth - padding - 1) + "…";
                    Console.Write(text.PadRight(colWidth));
                }
                Console.WriteLine();
            }
        }
    }
}
