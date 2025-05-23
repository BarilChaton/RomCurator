using System;
using System.IO;
using System.Collections.Generic;

namespace RomSorter
{
    class RomSorterClass : RomSorterApp
    {
        public string? RomDirectory { get; private set; }

        public void RunSorterMenu()
        {
            string prompt = @"
# ______                _____            _            
# | ___ \              /  ___|          | |           
# | |_/ /___  _ __ ___ \ `--.  ___  _ __| |_ ___ _ __ 
# |    // _ \| '_ ` _ \ `--. \/ _ \| '__| __/ _ \ '__|
# | |\ \ (_) | | | | | /\__/ / (_) | |  | ||  __/ |   
# \_| \_\___/|_| |_| |_\____/ \___/|_|   \__\___|_|   
                                                    
                                                    
" + "\n Version 0.0.1" +
"\n Use the arrow keys to select an option.";

            string[] options = { "Select folder path", "Back" };
            UI_Selector menu = new UI_Selector(prompt, options);
            int selectedOption = menu.Run();

            switch (selectedOption)
            {
                case 0:
                    PromptForFolder();
                    break;
                case 1:
                    RunMain();
                    break;
            }


        }

        public void PromptForFolder()
        {
            string currentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            while (true)
            {
                Console.Clear();
                string[] directories;

                try
                {
                    directories = Directory.GetDirectories(currentPath);
                }
                catch
                {
                    Console.WriteLine("Unable to access this folder.");
                    Console.WriteLine("Press any key to go back...");
                    Console.ReadKey(true);
                    return;
                }

                List<string> options = new List<string> { "[Type path manually]", "[Select this folder]", "../", "Cancel" };
                foreach (string dir in directories)
                {
                    options.Add(Path.GetFileName(dir));
                }

                UI_Selector selector = new UI_Selector("Browsing: " + currentPath, options.ToArray());
                int selectedIndex = selector.Run();

                if (selectedIndex == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Enter the full path manually:");
                    string? manualPath = Console.ReadLine();
                }
                else if (selectedIndex == 1)
                {
                    RomDirectory = currentPath;
                    Console.WriteLine($"\nSelected: {RomDirectory}");
                    Console.WriteLine("Press any key to begin sorting...");
                    Console.ReadKey(true);
                    //SortByRegion(); // or call another submenu
                    return;
                }
                else if (selectedIndex == 2)
                {
                    string? parent = Directory.GetParent(currentPath)?.FullName;
                    if (parent != null)
                        currentPath = parent;
                }
                else
                {
                    string chosenDir = directories[selectedIndex - 2]; // offset for [Select]/[Up]
                    currentPath = chosenDir;
                }
            }
        }
    }
}
