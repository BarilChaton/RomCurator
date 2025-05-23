using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace RomSorter
{
    class RomCuratorComponent : RomCuratorApp
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
                        Console.WriteLine("Select what you wish to do with the roms in this folder.");

                        RunCurationOptionsMenu();

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
                    Console.WriteLine("Select what you wish to do with the roms in this folder.");

                    RunCurationOptionsMenu();

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

        public void RunCurationOptionsMenu()
        {
            string prompt = "Select an option: \n";
            string[] options = { "Sort ROMs", "Delete ROMs", "Back" };
            UI_Selector menu = new UI_Selector(prompt, options);
            int selectedOption = menu.Run();
            switch (selectedOption)
            {
                case 0:
                    SortRoms();
                    break;
                case 1:
                    // DeleteRoms();
                    break;
                case 2:
                    RunFolderSelectMenu();
                    break;
            }
        }

        public void SortRoms()
        {
            if (string.IsNullOrWhiteSpace(RomDirectory) || !Directory.Exists(RomDirectory))
            {
                Console.WriteLine("No valid ROM directory selected");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                RunFolderSelectMenu();
                return;
            }

            string prompt = "Select a sorting method: \n";
            string[] options = { "Sort by Console/System", "Sort by Region", "Sort Alphabetically", "Cancel" };

            UI_Selector selector = new UI_Selector(prompt, options);
            int selected = selector.Run();

            switch (selected)
            {
                case 0:
                    SortBySystem();
                    break;
                case 1:
                    SortByRegion();
                    break;
                case 2:
                    SortAlphabetically();
                    break;
                case 3:
                    RunCurationOptionsMenu();
                    break;
            }
        }

        private void SortAlphabetically()
        {
            throw new NotImplementedException();
        }

        private void SortByRegion()
        {
            throw new NotImplementedException();
        }

        private void SortBySystem()
        {
            string[] files = Directory.GetFiles(RomDirectory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                string? systemFolder = null;

                if (extension == ".zip")
                {
                    systemFolder = InspectZipFile(file);
                    Console.WriteLine($"File: {file} - System: {systemFolder}");
                } 
                else
                {
                    systemFolder = extension switch
                    {
                        ".nes" => "NES",
                        ".sfc" or ".smc" => "SNES",
                        ".gb" => "GameBoy",
                        ".gbc" => "GameBoy Color",
                        ".gba" => "GameBoy Advance",
                        ".n64" or ".z64" or ".v64" => "Nintendo 64",
                        ".md" => "Genesis",
                        ".iso" => "PlayStation 2",
                        ".cue" => "PlayStation 1", // Assumes bin is next to cue
                        _ => null
                    };
                }

                if (systemFolder != null)
                {
                    string destFolder = Path.Combine(RomDirectory, systemFolder);
                    Directory.CreateDirectory(destFolder);
                    string destPath = Path.Combine(destFolder, Path.GetFileName(file));
                    File.Move(file, destPath, overwrite: true);
                    Console.WriteLine($"Moved {Path.GetFileName(file)} to {systemFolder}/");
                }
                else
                {
                    Console.WriteLine($"Unknown system for file: {file}");
                }
            }
        }

        private string? InspectZipFile(string zipPath)
        {
            try
            {
                using ZipArchive archive = ZipFile.OpenRead(zipPath);
                foreach (var entry in archive.Entries)
                {
                    string innerExt = Path.GetExtension(entry.FullName).ToLower();

                    return innerExt switch
                    {
                        ".nes" => "NES",
                        ".sfc" or ".smc" => "SNES",
                        ".gb" => "GameBoy",
                        ".gbc" => "GameBoy Color",
                        ".gba" => "GameBoy Advance",
                        ".n64" or ".z64" or ".v64" => "Nintendo 64",
                        ".md" => "Genesis",
                        ".iso" => "PlayStation 2",
                        ".cue" => "PlayStation 1", // Assumes bin is next to cue
                        _ => null
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading ZIP: {zipPath} - {ex.Message}");
            }

            return null;
        }
    }
}
