using System;

namespace RomSorter
{
    class UI_Selector
    {
        private int selectedIndex;
        private string[] options;
        private string? prompt;
        private int windowSize;

        public UI_Selector(string prompt, string[] options)
        {
            this.prompt = prompt;
            this.options = options;
            selectedIndex = 0;
            this.windowSize = Console.WindowHeight - 6;
        }

        public void DisplayOptions(int scrollOffset)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(this.prompt);
            for (int i = scrollOffset; i < scrollOffset + windowSize && i < options.Length; i++)
            {
                string currentOption = this.options[i];
                string prefix;

                if (i == selectedIndex)
                {
                    prefix = "-> ";
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    prefix = "   ";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{prefix} << {currentOption} >>");
            }

            Console.ResetColor();

            if (options.Length > windowSize)
            {
                Console.WriteLine($"\n Showing {scrollOffset + 1}–{Math.Min(scrollOffset + windowSize, options.Length)} of {options.Length} options");
            }
        }

        public int Run()
        {
            ConsoleKey keyPressed;
            int scrollOffset = 0;

            do
            {
                Console.Clear();
                DisplayOptions(scrollOffset);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                    {
                        selectedIndex = options.Length - 1;
                        scrollOffset = Math.Max(0, options.Length - windowSize);
                    }
                    else if (selectedIndex < scrollOffset)
                    {
                        scrollOffset = selectedIndex;
                    }
                }
                else if(keyPressed == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= options.Length)
                    {
                        selectedIndex = 0;
                        scrollOffset = 0;
                    }
                    else if (selectedIndex >= scrollOffset + windowSize)
                    {
                        scrollOffset = selectedIndex - windowSize + 1;
                    }
                }
            } while (keyPressed != ConsoleKey.Enter);

            return selectedIndex;
        }
    }
}
