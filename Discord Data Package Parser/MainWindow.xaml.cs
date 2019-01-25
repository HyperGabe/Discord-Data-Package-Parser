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
    class BwParse
    {
        public string Type { get; set; }
        public string Dir  { get; set; }
    }

    public partial class MainWindow : Window
    {

        ProgressBar progressBar;
        int directoryCount;
        
        public MainWindow()
        {
            InitializeComponent();           
        }

        public void MenuOpenFolder_Click(object sender, RoutedEventArgs e)
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

                    var directories = new List<string>(Directory.GetDirectories(dialog.FileName + "/messages"));
                    directoryCount = directories.Count();

                    progressBar = LoadProgressBar("Indexing Channels...", "0 / " + directoryCount.ToString(), directoryCount);

                    BackgroundWorker ListSubDir = new BackgroundWorker();
                    ListSubDir.WorkerReportsProgress = true;
                    ListSubDir.DoWork += ListSubDir_DoWork;
                    ListSubDir.ProgressChanged += ListSubDir_ProgressChanged;
                    ListSubDir.RunWorkerCompleted += ListSubDir_RunWorkerCompleted;
                    ListSubDir.RunWorkerAsync(dialog.FileName + "/messages");
                    //ListSubDir(dialog.FileName + "/messages");
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
            mainWindow.Close();
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
            progressBar = null;
            directoryCount = 0;
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

        private void ListSubDir_DoWork(object sender, DoWorkEventArgs e)
        {
            string dir = (string)e.Argument;
            MessageBox.Show(dir);

            var directories = new List<string>(Directory.GetDirectories(dir));

            MessageBox.Show("Test");

            BwParse bwParse = new BwParse();

            for (int I = 0; I < directories.Count(); I++)
            {
                switch (MessageType(directories[I]))
                {
                    case "DM":
                        bwParse.Type = "DM";
                        bwParse.Dir = directories[I].Remove(0, dir.Length + 1);
                        (sender as BackgroundWorker).ReportProgress(I, bwParse);
                        break;
                    default:
                        (sender as BackgroundWorker).ReportProgress(I);
                        break;
                }
                
            }
            System.Threading.Thread.Sleep(1000);
        }

        private void ListSubDir_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.proProgress.Value = e.ProgressPercentage;
            progressBar.lblProgress.Content = (e.ProgressPercentage + 1).ToString() + " / " + directoryCount.ToString();

            if(e.UserState != null)
            {
                BwParse bwParse = (BwParse)e.UserState;
                switch (bwParse.Type)
                {
                    case "DM":
                        lstDM.Items.Add(bwParse.Dir);
                        break;
                }
            }
            

        }

        private void ListSubDir_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Close();
        }
    }
}
