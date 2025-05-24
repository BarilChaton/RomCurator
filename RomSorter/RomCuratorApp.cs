using System;
using System.Collections.Generic;
using System.IO;

namespace RomSorter
{
    class RomCuratorApp
    {
        public void Start()
        {
            Console.Title = "RomCurator";
            RunMain();
        }

        public void RunMain()
        {
            string prompt = @"
Baril Chaton's
______                _____                 _             
| ___ \              /  __ \               | |            
| |_/ /___  _ __ ___ | /  \/_   _ _ __ __ _| |_ ___  _ __ 
|    // _ \| '_ ` _ \| |   | | | | '__/ _` | __/ _ \| '__|
| |\ \ (_) | | | | | | \__/\ |_| | | | (_| | || (_) | |   
\_| \_\___/|_| |_| |_|\____/\__,_|_|  \__,_|\__\___/|_|   
                                                          
                                                          
" + "\n Version 0.0.1" +
"\n Use the arrow keys to select an option.";

            string[] options = { "Curate ROMs", "Download ROMs", "Exit" };
            UI_Selector mainMenu = new UI_Selector(prompt, options);
            int selectedOption = mainMenu.Run();

            switch (selectedOption)
            {
                case 0:
                    SortRoms();
                    break;
                case 1:
                    //DownloadRoms();
                    break;
                case 2:
                    Exit();
                    break;
            }
        }

        void SortRoms()
        {
            RomCuratorComponent sorter = new RomCuratorComponent();
            sorter.RunFolderSelectMenu();
        }

        void Exit()
        {
            Console.Clear();
            Console.WriteLine("\n Press any key to exit.");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
    }
}
