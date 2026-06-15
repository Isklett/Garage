using Garage.Interfaces;
using Garage.ValueTypes;
using System.ComponentModel;

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
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (double.TryParse(input, out double result))
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

            int basePadding = 6;
            int maxColumnsLimit = 6;

            int rows = Math.Min(maxRows, choices.Count);

            int consoleWidth, consoleHeight, bufferHeight;
            try { consoleWidth = Console.WindowWidth; } catch { consoleWidth = names.Max(x => x.Length) + basePadding; }
            try { consoleHeight = Console.WindowHeight; } catch { consoleHeight = Math.Max(maxRows + 2, 10); }
            try { bufferHeight = Console.BufferHeight; } catch { bufferHeight = consoleHeight; }

            int startRow = Console.CursorTop;
            int neededRows = rows + 2;
            int availableBelow = bufferHeight - startRow;
            int extraLines = Math.Max(0, neededRows - availableBelow);

            for (int i = 0; i < extraLines; i++)
                Console.WriteLine();

            int topRow = startRow - extraLines;

            int prevWidth = consoleWidth;
            int prevHeight = consoleHeight;
            int prevBufferHeight = bufferHeight;
            int prevRows = rows;

            bool firstDraw = true;

            while (true)
            {
                Console.CursorVisible = false;

                try { consoleWidth = Console.WindowWidth; } catch { }
                try { consoleHeight = Console.WindowHeight; } catch { }
                try { bufferHeight = Console.BufferHeight; } catch { }

                // Checks if the console window has been resized and recalculates the necessary values.
                bool resized = consoleWidth != prevWidth || consoleHeight != prevHeight || bufferHeight != prevBufferHeight;
                if (resized)
                {
                    // Clear the old footprint at the old position/size before re-anchoring.
                    ClearScreen(topRow, prevRows + 1, prevWidth);

                    // Re-anchor: keep prompt area visible, clamp topRow to current buffer.
                    int maxTopRow = Math.Max(0, bufferHeight - (rows + 1));
                    topRow = Math.Min(topRow, maxTopRow);
                    topRow = Math.Max(0, topRow);

                    prevWidth = consoleWidth;
                    prevHeight = consoleHeight;
                    prevBufferHeight = bufferHeight;
                    firstDraw = true; // force full redraw including prompt without "previous size" assumptions.
                }

                if (!firstDraw)
                    ClearScreen(topRow, rows + 1, consoleWidth);
                else
                    ClearScreen(topRow, rows + 1, Math.Max(prevWidth, consoleWidth));
                firstDraw = false;

                int largestName = names.Max(x => x.Length) + basePadding;
                int colWidth = Math.Max(1, largestName);
                int maxColumns = Math.Max(1, consoleWidth / colWidth);
                maxColumns = Math.Min(maxColumns, maxColumnsLimit);

                int itemsPerPage = maxRows * maxColumns;
                int page = selected / Math.Max(1, itemsPerPage);
                int pageStart = page * itemsPerPage;
                int pageEnd = Math.Min(names.Length, pageStart + itemsPerPage);

                Console.SetCursorPosition(0, topRow);
                Console.Write(prompt.PadRight(consoleWidth));

                for (int i = pageStart; i < pageEnd; i++)
                {
                    int indexInPage = i - pageStart;
                    int column = indexInPage / maxRows;
                    int row = indexInPage % maxRows;

                    int cursorLeft = column * colWidth;
                    int cursorTop = topRow + 1 + row;

                    string label = names.Length < 10 ? $"{i}: {names[i]}" : names[i];
                    string cellText = (i == selected ? "> " : "  ") + label;
                    if (cellText.Length > colWidth - 1)
                        cellText = cellText.Substring(0, Math.Max(0, colWidth - 2)) + "…";

                    if (cursorTop < 0 || cursorTop >= bufferHeight || cursorLeft >= consoleWidth) continue;

                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    if (i == selected)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(cellText.PadRight(colWidth));
                        Console.ForegroundColor = color;
                    }
                    else
                        Console.Write(cellText.PadRight(colWidth));
                }

                prevRows = rows;

                keyInfo = Console.ReadKey(intercept: true).Key;

                if (keyInfo == ConsoleKey.UpArrow) selected = (selected - 1 + names.Length) % names.Length;
                else if (keyInfo == ConsoleKey.DownArrow) selected = (selected + 1) % names.Length;
                else if (keyInfo == ConsoleKey.LeftArrow) selected = (selected - maxRows + names.Length) % names.Length;
                else if (keyInfo == ConsoleKey.RightArrow) selected = (selected + maxRows) % names.Length;
                else if (keyInfo == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, Math.Min(topRow + 1 + maxRows, bufferHeight - 1));
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    return selected;
                }
                else if (keyInfo >= ConsoleKey.D0 && keyInfo <= ConsoleKey.D9)
                {
                    int num = keyInfo - ConsoleKey.D0;
                    if (num >= 0 && num < names.Length)
                    {
                        Console.SetCursorPosition(0, Math.Min(topRow + 1 + maxRows, bufferHeight - 1));
                        Console.WriteLine();
                        Console.CursorVisible = true;
                        return num;
                    }
                }
            }
        }

        [Description("lastChoice is the option to finish editing, for example 'Accept' or 'Search'")]
        public SearchParameters.VehicleSearch GetMultipleLines(string[] inPrompts, string lastChoice)
        {
            string[] prompts = new string[inPrompts.Length + 1];
            for (int i = 0; i < inPrompts.Length; i++)
                prompts[i] = inPrompts[i];
            prompts[inPrompts.Length] = lastChoice;

            ConsoleKey keyInfo;
            int selected = 0;
            string[] answers = new string[inPrompts.Length];
            for (int i = 0; i < answers.Length; i++)
                answers[i] = "";

            int consoleWidth;
            try { consoleWidth = Console.WindowWidth; } catch { consoleWidth = 120; }

            int topRow = Console.CursorTop;
            int bufferHeight;
            try { bufferHeight = Console.BufferHeight; } catch { bufferHeight = topRow + prompts.Length + 1; }

            // Reserve space for all lines
            int needed = prompts.Length;
            int availableBelow = bufferHeight - topRow;
            int extraLines = Math.Max(0, needed - availableBelow);
            for (int i = 0; i < extraLines; i++)
                Console.WriteLine();
            topRow -= extraLines;

            bool editing = false;

            while (true)
            {
                Console.CursorVisible = editing;

                for (int i = 0; i < prompts.Length; i++)
                {
                    int row = topRow + i;
                    if (row < 0 || row >= bufferHeight) continue;

                    try { Console.SetCursorPosition(0, row); } catch { continue; }

                    string blank = new string(' ', Math.Max(1, consoleWidth));
                    Console.Write(blank);
                    Console.SetCursorPosition(0, row);

                    bool isLast = i == prompts.Length - 1;
                    string label = isLast ? prompts[i] : $"{prompts[i]}: {answers[i]}";

                    if (i == selected)
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = editing ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
                        Console.Write("> " + label);
                        Console.ForegroundColor = prev;
                    }
                    else
                    {
                        Console.Write("  " + label);
                    }
                }

                if (editing)
                {
                    // Position cursor right after the current answer for typing
                    string label = $"{prompts[selected]}: ";
                    int col = 2 + label.Length + answers[selected].Length;
                    try { Console.SetCursorPosition(Math.Min(col, consoleWidth - 1), topRow + selected); } catch { }
                }

                ConsoleKeyInfo cki = Console.ReadKey(intercept: true);
                keyInfo = cki.Key;

                if (!editing)
                {
                    if (keyInfo == ConsoleKey.UpArrow)
                        selected = (selected - 1 + prompts.Length) % prompts.Length;
                    else if (keyInfo == ConsoleKey.DownArrow)
                        selected = (selected + 1) % prompts.Length;
                    else if (keyInfo == ConsoleKey.Enter)
                    {
                        if (selected == prompts.Length - 1)
                        {
                            Console.CursorVisible = true;

                            Console.SetCursorPosition(0, Math.Min(topRow + 1 + prompts.Length, bufferHeight - 1));
                            Console.WriteLine();

                            return new SearchParameters.VehicleSearch(
                                answers[0], answers[1], answers[2], answers[3],
                                answers[4], answers[5]);
                        }
                        else
                        {
                            editing = true;
                            Console.CursorVisible = true;
                        }
                    }
                }
                else
                {
                    if (keyInfo == ConsoleKey.Enter)
                    {
                        editing = false;
                        Console.CursorVisible = false;
                    }
                    else if (keyInfo == ConsoleKey.Backspace)
                    {
                        if (answers[selected].Length > 0)
                            answers[selected] = answers[selected].Substring(0, answers[selected].Length - 1);
                    }
                    else
                    {
                        char c = cki.KeyChar;
                        if (!char.IsControl(c))
                            answers[selected] += c;
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
        void ClearScreen(int startRow, int rowCount, int width)
        {
            string blank = new string(' ', Math.Max(1, width));
            for (int i = 0; i < rowCount; i++)
            {
                int row = startRow + i;
                if (row < 0 || row >= Console.BufferHeight) continue;
                try
                {
                    Console.SetCursorPosition(0, row);
                    Console.Write(blank);
                }
                catch { }
            }
        }

        public void Draw(string[]? options, List<string> table)
        {
            ClearScreen(0);
            if (options != null)
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