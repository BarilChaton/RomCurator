using System.IO.Compression;

namespace RomSorter
{
    class RomCuratorComponent : RomCuratorApp
    {
        public string? RomDirectory { get; private set; }
        string[] skipExtensions = { ".iso", ".bin", ".cue", ".chd", ".img" };

        #region Folder Selection Methods
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

        #endregion

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
            Console.Clear();

            if (string.IsNullOrWhiteSpace(RomDirectory) || !Directory.Exists(RomDirectory))
            {
                Console.WriteLine("No valid ROM directory selected");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                RunFolderSelectMenu();
                return;
            }

            string prompt = "Select a sorting method: \n";
            string[] options = { "Sort by Console/System", "Sort by Region", "Cancel" };

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
                    RunCurationOptionsMenu();
                    break;
            }
        }

        #region Sorting Methods
        private void SortBySystem()
        {

            string[] files = Directory.GetFiles(RomDirectory, "*.*", SearchOption.TopDirectoryOnly);
            int totalFiles = files.Length;
            int processedFiles = 0;

            foreach (string file in files)
            {
                processedFiles++;
                DrawProgressBar(processedFiles, totalFiles);

                string extension = Path.GetExtension(file).ToLower();
                string? systemFolder = null;

                if (skipExtensions.Contains(extension))
                {
                    continue;
                }

                if (extension == ".zip")
                {
                    systemFolder = InspectZipFile(file);
                }
                else
                {
                    systemFolder = extension switch
                    {
                        ".adf" => "amiga",
                        ".nib" => "c64",
                        ".nes" => "nes",
                        ".sfc" or ".smc" => "snes",
                        ".gb" => "gb",
                        ".gbc" => "gbc",
                        ".gba" => "gba",
                        ".nds" => "nds",
                        ".n64" or ".z64" or ".v64" => "n64",
                        ".md" => "megadrive",
                        ".32x" => "sega32x",
                        ".pce" => "pcengine",
                        ".xex" => "atari800",
                        ".a26" => "atari2600",
                        ".a78" => "atari7800",
                        ".lnx" => "atarilynx",
                        ".col" => "coleco",
                        ".int" => "intellivision",
                        ".sg" => "sg-1000",
                        ".vec" => "vectrex",
                        ".cpr" => "amstradgx4000",
                        ".mgw" => "gameandwatch",
                        ".gg" => "gamegear",
                        ".sms" => "mastersystem",
                        ".ws" => "wonderswan",
                        ".ngc" => "ngpc",
                        _ => "Unknown"
                    };
                }

                if (systemFolder == "Unknown" || systemFolder == "UnknownSystem")
                {
                    continue;
                }

                string destDir = Path.Combine(RomDirectory, systemFolder);
                Directory.CreateDirectory(destDir);
                string destPath = Path.Combine(destDir, Path.GetFileName(file));

                File.Move(file, destPath, overwrite: true);
            }

            Console.WriteLine("\nSorting complete!");
            Console.CursorVisible = true;

            Console.ReadKey(true);
            RunMain();
        }

        private void SortByRegion()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods
        private string? InspectZipFile(string zipPath)
        {
            try
            {
                using ZipArchive archive = ZipFile.OpenRead(zipPath);
                foreach (var entry in archive.Entries)
                {
                    string innerExt = Path.GetExtension(entry.FullName).ToLower();

                    if (innerExt is ".iso" or ".bin" or ".cue" or ".chd")
                    {
                        Console.WriteLine($"ZIP contains unsupported disc image format: {entry.FullName}");
                        return null;
                    }

                    return innerExt switch
                    {
                        ".adf" => "amiga",
                        ".nib" => "c64",
                        ".nes" => "nes",
                        ".sfc" or ".smc" => "snes",
                        ".gb" => "gb",
                        ".gbc" => "gbc",
                        ".gba" => "gba",
                        ".nds" => "nds",
                        ".n64" or ".z64" or ".v64" => "n64",
                        ".md" => "megadrive",
                        ".32x" => "sega32x",
                        ".pce" => "pcengine",
                        ".xex" => "atari800",
                        ".a26" => "atari2600",
                        ".a78" => "atari7800",
                        ".lnx" => "atarilynx",
                        ".col" => "coleco",
                        ".int" => "intellivision",
                        ".sg" => "sg-1000",
                        ".vec" => "vectrex",
                        ".cpr" => "amstradgx4000",
                        ".mgw" => "gameandwatch",
                        ".gg" => "gamegear",
                        ".sms" => "mastersystem",
                        ".ws" => "wonderswan",
                        ".ngc" => "ngpc",
                        _ => "Unknown"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading ZIP: {zipPath} - {ex.Message}");
            }

            return null;
        }

        private void DrawProgressBar(int progress, int total, int barLength = 50)
        {
            double percentage = (double)progress / total;
            int filledLength = (int)(barLength * percentage);
            string bar = new string('#', filledLength).PadRight(barLength);
            Console.CursorVisible = false;
            Console.Write($"\rSorting ROMs... [{bar}] {progress}/{total}");
        }

        #endregion
    }
}
