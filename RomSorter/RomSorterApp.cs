using System;
using System.Collections.Generic;
using System.IO;

namespace RomSorter
{
    class RomSorterApp
    {
        public void Start()
        {
            Console.Title = "RomSorter";
            RunMain();
        }

        public void RunMain()
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

            string[] options = { "Sort ROMs", "Download ROMs", "Delete ROMs", "Exit" };
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
                    //DeleteRoms();
                    break;
                case 3:
                    Exit();
                    break;
            }
        }

        void SortRoms()
        {
            RomSorterClass sorter = new RomSorterClass();
            sorter.RunSorterMenu();
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
