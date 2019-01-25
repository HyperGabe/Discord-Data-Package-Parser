using System;
using System.IO;
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
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Discord_Data_Package_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    class ChannenJson
    {
        public string Type          { get; set; }
        public string ChannelID     { get; set; }
        public string UserID        { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();           
        }

        private void MenuOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = false,
                AllowNonFileSystemItems = false,
                DefaultFileName = "Select the Package Folder",
                Title = "Select The Folder To Parse"
            };
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Clear();
                if(!Directory.Exists(dialog.FileName + "/messages"))
                {
                    MessageBox.Show("messages folder could not be found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(dialog.FileName);
                    ListSubDir(dialog.FileName + "/messages");
                }
            }
            else
            {
                MessageBox.Show("Could not be loaded", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void MenuOpenZIP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuTheme_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Will be added soon :)", "Not available", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Clear()
        {
            lstDM.Items.Clear();
            lstServers.Items.Clear();
        }

        public string MessageType(string Dir)
        {
            ChannenJson data = new ChannenJson();
            string json = File.ReadAllText(Dir + "/channel.json");
            var ChannelDetails = JsonConvert.DeserializeObject<dynamic>(json);
            data.Type = ChannelDetails.type.ToString();
            switch (data.Type)
            {
                case "0":
                    return "GUILD_TEXT";
                case "1":
                    return "DM";
                case "3":
                    return "GROUP_DM";
                default:
                    return "ERROR";
            }
        }

        private ProgressBar LoadProgressBar(string title, string content, int max)
        {
            ProgressBar progressBar = new ProgressBar();
            progressBar.Title = title; //"Indexing channels...";
            progressBar.lblProgress.Content = content; //"0 / " + directoriescount.ToString();
            progressBar.proProgress.Maximum = max; //directoriescount;
            progressBar.Show();

            return progressBar;
        }

        private void ListSubDir(string dir)
        {
            MessageBox.Show(dir);

            var directories = new List<string>(Directory.GetDirectories(dir));
            int directoriescount = directories.Count();

            var progressBar = LoadProgressBar("Indexing Channels...", "0 / " + directoriescount, directories.Count());
            MessageBox.Show("Test");


            for (int I = 0; I < directories.Count(); I++)
            {
                switch (MessageType(directories[I]))
                {
                    case "DM":
                        lstDM.Items.Add(directories[I].Remove(0, dir.Length + 1));
                        break;
                }
                progressBar.lblProgress.Content = I + 1 + " / " + directoriescount.ToString();
                progressBar.proProgress.Value = I + 1;
                
                System.Threading.Thread.Sleep(100);
            }
            
        }

        
    }
}
