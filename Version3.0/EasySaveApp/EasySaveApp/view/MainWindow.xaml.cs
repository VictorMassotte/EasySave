﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySaveApp.model;
using EasySaveApp.viewmodel;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace EasySaveApp.view
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewmodel;
        public string langue = "en";

        
        public MainWindow()
        {
            InitializeComponent();
            viewmodel = new ViewModel();
            ShowListBox();



            Process Proc_EnCours = Process.GetCurrentProcess(); //Obtains the current process of the application

            Process[] Les_Proc = Process.GetProcesses(); //Collection of currently launched processes

            foreach (Process Processus in Les_Proc)
                if (Proc_EnCours.Id != Processus.Id) ////If the IDs are different but of the same name
                {
                    if (Proc_EnCours.ProcessName == Processus.ProcessName)
                    {
                        MessageBox.Show("The EasySeave software is already running.");
                        this.Close();
                    }
                }
        }

        private void Open_cryptextension(object sender, RoutedEventArgs e)//Function that allows the button to open the file of extensions to be encrypted
        {
            System.Diagnostics.Process.Start("notepad.exe", @"..\..\..\Ressources\CryptExtension.json");
        }

        private void Button_Click_fr(object sender, RoutedEventArgs e)//Function to translate the software into French
        {
            result.Text = "";
            langue = "fr";
            name_backup.Content = "Nom de la sauvegarde: ";
            diff_button.Content = "Sauvegarde diffenrentielle";
            mirror_button.Content = "Sauvegarde Complete";
            source_name.Content = "Chemin du dossier à sauvegarder:";
            target_name.Content = "Chemin de destination:";
            mirror_name.Content = "Chemin de la sauvegarde complete:";
            text_addsave.Text = "AJOUTER UNE SAUVEGARDE";
            textcrypt.Text = "Parametre des extensions de \n" +
                "Cryptographie";
            text_startbackup.Text = "DEMARER LA SAUVEGARDE";
            textbacklist.Text = "LOGICIEL A BANNIR";
            francaistext.Text = "FRANCAIS";
            englishtext.Text = "ENGLISH";
            textextensionprio.Text = "EXTENSION PRIORITAIRE";

        }

        private void Button_Click_en(object sender, RoutedEventArgs e)//Function to translate the software into English
        {
            result.Text = "";
            langue = "en";
            name_backup.Content = "Name of backup: ";
            diff_button.Content = "Differential Backup";
            mirror_button.Content = "Mirror Backup";
            source_name.Content = "Source folder path: ";
            target_name.Content = "Target folder path: ";
            mirror_name.Content = "Mirror folder backup";
            text_addsave.Text = "ADD SAVE";
            textcrypt.Text = "PARAMETER OF CRYPTAGE";
            text_startbackup.Text = "START THE BACKUP";
            textbacklist.Text = "BLACKLIST SOFTWARE";
            francaistext.Text = "FRENCH";
            englishtext.Text = "ENGLISH";
            textextensionprio.Text = "PRIORITY EXTENSION";

        }

        private void buton_addsave_Click(object sender, RoutedEventArgs e)//Function that allows the button to add a backup
        {
            string saveName = "";
            string sourceDir = "";
            string targetDir = "";
            string mirrorDir = "";

            saveName = name_save.Text;
            sourceDir = SoureDir.Text;
            targetDir = TargetDir.Text;
            mirrorDir = MirrorDir.Text;

            if (mirror_button.IsChecked.Value) //If the button of the full backup is selected
            {
                if (name_save.Text.Length.Equals(0) || SoureDir.Text.Length.Equals(0) || TargetDir.Text.Length.Equals(0))
                {
                    if (langue == "fr")
                    {
                        result.Text = " Veuillez remplir tous les champs ! ";
                    }
                    else
                    {
                        result.Text = " Please complete all fields ! ";
                    }
                }
                else
                {
                    int type = 1;

                    viewmodel.AddSaveModel(type, saveName, sourceDir, targetDir, ""); //Function to add the backup

                    if (langue == "fr")//Condition for the display of the success message according to the language chosen by the user.
                    {
                        result.Text = "VOUS AVEZ AJOUTÉ UNE SAUVEGARDE \n";
                    }
                    else
                    {
                        result.Text = "YOU HAVE ADDED A BACKUP";
                    }

                    ShowListBox();//Function to update the list.
                }

            }
            else if (diff_button.IsChecked.Value)//If the button of the full backup is selected
            {
                if (name_save.Text.Length.Equals(0) || SoureDir.Text.Length.Equals(0) || TargetDir.Text.Length.Equals(0) || MirrorDir.Text.Length.Equals(0))
                {
                    if (langue == "fr")
                    {
                        result.Text = " Veuillez remplir tous les champs sauf celui du mirror path ! ";
                    }
                    else
                    {
                        result.Text = " Please complete all fields test! ";
                    }
                }
                else
                {
                    int type = 2;
                    viewmodel.AddSaveModel(type, saveName, sourceDir, targetDir, mirrorDir);//Function to add the backup

                    if (langue == "fr")//Condition for the display of the success message according to the language chosen by the user.
                    {
                        result.Text = "VOUS AVEZ AJOUTÉ UNE SAUVEGARDE \n" +
                            " DIFFÉRENTIELLE";
                    }
                    else
                    {
                        result.Text = "YOU HAVE ADDED A DIFFERENTIAL\n" +
                                    " BACKUP";
                    }

                    ShowListBox();//Function to update the list.
                }
            }
        }

        private void source_directory_Click(object sender, RoutedEventArgs e)//Function to retrieve the path to the source folder
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog(); //Declaration of the method to open the window to choose the folder path.
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SoureDir.Text = dialog.FileName; //Displays the path in the window text.
            }
        }

        private void target_directory_Click(object sender, RoutedEventArgs e)//Function to retrieve the path to the destination folder
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog(); //Declaration of the method to open the window to choose the folder path.
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TargetDir.Text = dialog.FileName; //Displays the path in the window text.
            }
        }

        private void mirror_directory_Click(object sender, RoutedEventArgs e)//Function to retrieve the folder path of the mirror backup.
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog(); //Declaration of the method to open the window to choose the folder path.
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MirrorDir.Text = dialog.FileName; //Displays the path in the window text.
            }
        }

        private void ShowListBox() //Function that displays the names of the backups in the list.
        {

            Save_work.Items.Clear();

            List<string> names = viewmodel.ListBackup();
            foreach(string name in names)//Loop that allows you to manage the names in the list.
            {
                Save_work.Items.Add(name); //Function that allows you to insert the names of the backups in the list.
            }
        }

        private void mirror_button_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_exit(object sender, RoutedEventArgs e)//Function of the button to close the software
        {
            Process.GetCurrentProcess().Kill();
        }

        private void GridMenu_MouseDown(object sender, RoutedEventArgs e)//Function that allows you to move the software window.
        {
            DragMove();//Function that allows movement
        }

        private void button_startsave_Click(object sender, RoutedEventArgs e)//Function that launches the backup
        {
            string saveName = "";

            if (Save_work.SelectedItem != null) //Condition that allows to check if the user has selected a backup.
            {
                foreach(string item in Save_work.SelectedItems)//Loop that allows you to select multiple saves
                {
                    try
                    {
                        saveName = item.ToString();
                        //viewmodel.LoadBackup(saveName, langue);
                        new Thread(() => viewmodel.LoadBackup(saveName, langue)).Start();

                    }
                    catch
                    {
                        
                    }
                }
            }
        }

        private void Open_blacklist(object sender, RoutedEventArgs e)//Function that allows the button to open the file of blacklisted software
        {
            System.Diagnostics.Process.Start("notepad.exe", @"..\..\..\Ressources\BlackList.json");
        }

        private void Open_extensionprio(object sender, RoutedEventArgs e)//Function allowing the button to open the file for priority extensions
        {
            System.Diagnostics.Process.Start("notepad.exe", @"..\..\..\Ressources\Priority.json");
        }

        private void Button_minimize(object sender, RoutedEventArgs e)//Function to reduce the window.
        {
            WindowState = (WindowState)FormWindowState.Minimized;
        }

        private void Button_Play_click(object sender, RoutedEventArgs e)
        {
            viewmodel.PlayButton_click();
        }

        private void Button_Pause_click(object sender, RoutedEventArgs e)
        {
            viewmodel.PauseButton_click();
        }

        private void Button_Stop_click(object sender, RoutedEventArgs e)
        {
            viewmodel.StopButton_click();
        }
    }
}
