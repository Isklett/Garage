using Garage.Interfaces;
using Garage.ValueTypes;
using System.ComponentModel;
using System.Runtime.InteropServices.Java;
using static Garage.ValueTypes.Enumerators.VehicleEnums;

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

        public string GetStringInput(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            return input;
        }
        public int GetIntInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                else
                {
                    ShowError("Invalid input. Please enter a valid integer.");
                }
            }
        }
        public float GetFloatInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (float.TryParse(input, out float result))
                {
                    return result;
                }
                else
                {
                    ShowError("Invalid input. Please enter a valid float.");
                }
            }
        }

        public double GetDoubleInput(string prompt)
        {
            while(true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if(double.TryParse(input,out double result))
                {
                    return result;
                }
                else
                {
                    ShowError("Invalid input. Please enter a valid double.");
                }
            }
        }

        [Description("User can choose number directly (only with max 10 choices) or traverse choices with keys and press enter to choose.")]
        public int? GetChoiceInput(string prompt, IReadOnlyList<string> choices, string errorMessage, int maxChoices = int.MaxValue, int maxRows = 5)
        {
            string[] names = choices.Take(maxChoices).ToArray();
            if (names.Length == 0)
            {
                Console.WriteLine(errorMessage);
                return null;
            }

            int selected = 0;
            ConsoleKey keyInfo;
            int currentRow = Console.CursorTop;
            int largestName = names.Max(x => x.Length) + 6;
            int maxColumns = 6;


            while (true)
            {
                Console.CursorTop = currentRow;
                Console.CursorVisible = false;
                ClearScreen(currentRow);
                Console.WriteLine(prompt);

                int indent = (selected / maxRows) % maxColumns;

                for (int i = indent * maxColumns * maxRows; i < Math.Min(names.Length, indent * maxColumns * maxRows + (maxColumns * maxRows)); i++)
                {
                    int column = Math.Min(i / maxRows, maxColumns);
                    int row = i % maxRows;

                    Console.CursorLeft = largestName * column;
                    Console.CursorTop = currentRow + row + 1;

                    if (names.Length < 10)
                    {
                        if (i == selected)
                        {
                            var prev = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("> ");
                            Console.Write($"{i}: {names[i]}");
                            Console.ForegroundColor = prev;
                        }
                        else
                        {
                            Console.Write("  ");
                            Console.Write($"{i}: {names[i]}");
                        }
                    }
                    else
                    {
                        if (i == selected)
                        {
                            var prev = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("> ");
                            Console.Write($"{names[i]}");
                            Console.ForegroundColor = prev;
                        }
                        else
                        {
                            Console.Write("  ");
                            Console.Write($"{names[i]}");
                        }
                    }
                }

                keyInfo = Console.ReadKey(intercept: true).Key;

                // Navigation between choices. Modulus to enable going between last and first choice.
                if (keyInfo == ConsoleKey.UpArrow)
                    selected = (selected - 1 + names.Length) % names.Length;
                else if (keyInfo == ConsoleKey.DownArrow)
                    selected = (selected + 1) % names.Length;
                else if (keyInfo == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    Console.CursorTop = maxRows + currentRow + 1;
                    return selected;
                }
                else
                {
                    // Quick numeric selection (0-9)
                    if (keyInfo >= ConsoleKey.D0 && keyInfo <= ConsoleKey.D9)
                    {
                        int num = keyInfo - ConsoleKey.D0; //Translates key to numeric value.
                        if (num >= 0 && num < names.Length)
                        {
                            Console.WriteLine();
                            Console.CursorVisible = true;
                            Console.CursorTop = maxRows + currentRow + 1;
                            return num;
                        }
                    }
                }
            }
        }

        [Description("lastChoice is the option to finish editing, for example 'Accept' or 'Search'")]
        public SearchParameters.VehicleSearch GetMultipleLines(string[] inPrompts, string lastChoice)
        {
            string[] prompts = new string[inPrompts.Length + 1];
            for (int i = 0; i < inPrompts.Length; i++)
            {
                prompts[i] = inPrompts[i];
            }
            prompts[inPrompts.Length - 1] = lastChoice;
            ConsoleKey keyInfo;
            int startRow = Console.CursorTop;
            int selected = 0;
            string[] answers = new string[prompts.Length];
            foreach (string prompt in prompts)
            {
                Console.WriteLine(prompt);
            }
            while (true)
            {
                for (int i = 0; i < prompts.Length; i++)
                {
                    if (i == selected)
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("> ");
                        Console.WriteLine($"{prompts[i]}");
                        Console.ForegroundColor = prev;
                    }
                    else
                    {
                        Console.Write("  ");
                        Console.WriteLine($"{prompts[i]}");
                    }
                }
                keyInfo = Console.ReadKey(intercept: true).Key;

                // Navigation between choices. Modulus to enable going between last and first choice.
                if (keyInfo == ConsoleKey.UpArrow)
                    selected = (selected - 1 + prompts.Length) % prompts.Length;
                else if (keyInfo == ConsoleKey.DownArrow)
                    selected = (selected + 1) % prompts.Length;
                else if (keyInfo == ConsoleKey.Enter)
                {
                    if(selected == prompts.Length - 1)
                    {
                        return new SearchParameters.VehicleSearch(answers[0], answers[1], answers[2], answers[3], int.Parse(answers[4]), (FuelType)int.Parse(answers[5]));
                    }
                    else
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        answers[selected] = Console.ReadLine() ?? "";
                        Console.ForegroundColor = prev;
                    }
                }


            }
        }

        public void ClearScreen(int fromRow = 1)
        {
            try
            {
                int startRow = fromRow;
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                for (int row = startRow; row < height; row++)
                {
                    Console.SetCursorPosition(0, row);
                    Console.Write(new string(' ', width));
                }

                Console.SetCursorPosition(0, startRow);
            }
            catch
            {
                // Fallback when console dimensions aren't available (redirected output, etc.)
                Console.Clear();
            }
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
