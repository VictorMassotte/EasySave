using System;
using System.Collections.Generic;
using System.Text;
using EasySaveApp.controller;
using EasySaveApp.model;

namespace EasySaveApp.view
{
    class View
    {
        //Display on the console a welcome message
        public void ShowStart()
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine("***            Welcome to EasySave               ***");
            Console.WriteLine("****************************************************");
        }
        //Display on the console the menu
        public void ShowMenu()
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine("***                    Menu                      ***");
            Console.WriteLine("****************************************************");
            Console.WriteLine("***  0. Exit                                     ***");
            Console.WriteLine("***  1. Open a backup job                        ***");
            Console.WriteLine("***  2. Create a backup job                      ***");
            Console.WriteLine("****************************************************");
            Console.Write("Please enter the menu number : ");
        }
        //Display on the console the menu of backup jobs
        public void ShowSubMenu()
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine("***                 Backup Jobs                  ***");
            Console.WriteLine("****************************************************");
            Console.WriteLine("***  0. Exit                                     ***");
            Console.WriteLine("***  1. Complete Save                            ***");
            Console.WriteLine("***  2. Differential Save                        ***");
            Console.WriteLine("****************************************************");
            Console.Write("Please enter the menu number : ");
        }
        //Display on the console when you enter the name of the save
        public void ShowName()
        {
            Console.WriteLine("Please enter the name of your backup : ");
        }
        //Display on the console when you have to enter the path of the folder that you want to back up
        public void ShowSourceDir()
        {
            Console.WriteLine("Please enter the path of the folder you want to back up. [ DRAG AND DROP YOUR FOLDER ] :");
        }
        //Display on the console when you have to enter the path of the folder that you want to back up
        public void ShowTargetDir()
        {
            Console.WriteLine("Please enter the destination path for the backup. [ DRAG AND DROP YOUR FOLDER ] :");
        }

        public void ShowMirrorDir()
        {
            Console.WriteLine("Please enter the path of the folder mirror backup. [ DRAG AND DROP YOUR FOLDER ] :");
        }
        //Display on the console an error message
        public void ErrorMenu(string result)
        {
            Console.WriteLine(result);
        }
        //Display on the console when you have to enter the name of backup
        public void ShowFile()
        {
            Console.Write("Please enter the name of your backup : ");
        }
        //Display on the console when you have to enter the name of backups
        public void ShowNameFile()
        {
            Console.Clear();
            Console.WriteLine("Here are the names of your backups : ");
        }
    }
}
