using System;
using System.IO;
using System.Collections.Generic;

namespace RomSorter
{
    class RomSorterClass : RomSorterApp
    {
        public string? RomDirectory { get; private set; }

        public void RunFolderSelectMenu()
        {
            string prompt = "Use the arrow keys to select an option. \n";

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
            string? currentPath = null;

            while (true)
            {
                Console.Clear();
                List<string> options = new List<string> { "[Type path manually]" };
                string title;

                if (currentPath == null)
                {
                    title = "This PC - Select a drive: ";
                    string[] drives = Directory.GetLogicalDrives();

                    foreach (var drive in drives)
                    {
                        options.Add(drive);
                    }
                } 
                else
                {
                    title = "Browsing: " + currentPath;

                    try
                    {
                        string[] directories = Directory.GetDirectories(currentPath);
                        options.Add("[Select this folder]");
                        options.Add("[.. (Go Up)]");
                        options.Add("[Cancel]");

                        foreach (string dir in directories)
                        {
                            options.Add(Path.GetFileName(dir));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Unable to access this folder.");
                        Console.WriteLine("Press any key to go back...");
                        Console.ReadKey(true);
                        return;
                    }
                }


                UI_Selector selector = new UI_Selector(title, options.ToArray());
                int selectedIndex = selector.Run();

                if (selectedIndex == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Enter the full path manually:");
                    string? manualPath = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(manualPath) && Directory.Exists(manualPath))
                    {
                        RomDirectory = manualPath;
                        Console.WriteLine($"\n Selected: {RomDirectory}");
                        Console.WriteLine("Press any key to begin sorting...");
                        Console.ReadKey(true);

                        // Run Sort options menu here

                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid path. Press any key to try again...");
                        Console.ReadKey(true);
                    }
                }
                else if (currentPath == null)
                {
                    currentPath = options[selectedIndex];
                }
                else if (selectedIndex == 1)
                {
                    RomDirectory = currentPath;
                    Console.WriteLine($"\nSelected: {RomDirectory}");
                    Console.WriteLine("Press any key to begin sorting...");
                    Console.ReadKey(true);

                    // Run Sort options menu here

                    return;
                }
                else if (selectedIndex == 2)
                {
                    string? parent = Directory.GetParent(currentPath)?.FullName;
                    currentPath = parent ?? null;
                }
                else if (selectedIndex == 3)
                {
                    RunFolderSelectMenu();
                    return;
                }
                else
                {
                    int commandCount = 3;

                    if (selectedIndex >= commandCount)
                    {
                        string selectedSubDirName = options[selectedIndex];
                        string selectedFullPath = Path.Combine(currentPath, selectedSubDirName);
                        currentPath = selectedFullPath;
                    }
                }
            }
        }
    }
}
