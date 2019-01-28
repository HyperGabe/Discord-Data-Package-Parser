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
    public class ChannelJson
    {
        public string Type          { get; set; }
        public string ID            { get; set; }
        public string Name          { get; set; }
    }



    public partial class MainWindow : Window
    {

        public string GetName(string ID)
        {
            if (index.ContainsKey(ID) & index[ID] != null)
            {
                return index[ID];
            }
            else
            {
                return "null";
            }
        }

        public ChannelJson MessageType(string Dir)
        {
            ChannelJson data = new ChannelJson();
            string json = File.ReadAllText(Dir + "/channel.json");
            var ChannelDetails = JsonConvert.DeserializeObject<dynamic>(json);
            data.ID = ChannelDetails.id;
            data.Type = ChannelDetails.type;
            if (data.Type == "1")
            {
                data.Name = GetName(data.ID).Substring(20);
            }
            else
            {
                data.Name = GetName(data.ID);
            }
            return data;
        }

        private void Clear()
        {
            lstDM.Items.Clear();
            lstServers.Items.Clear();
            lstGDM.Items.Clear();
            progressBar = null;
            directoryCount = 0;
        }

        ProgressBar progressBar;
        int directoryCount;
        Dictionary<string, string> index;
        List<ChannelJson> DMList = new List<ChannelJson>();
        List<ChannelJson> GDMList = new List<ChannelJson>();
        List<ChannelJson> ServersList = new List<ChannelJson>();

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
                    string messages = dialog.FileName + "/messages";
                    var directories = new List<string>(Directory.GetDirectories(messages));
                    string json = File.ReadAllText(messages + "/index.json");
                    index = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    directoryCount = directories.Count();

                    progressBar = LoadProgressBar("Indexing Channels...", "0 / " + directoryCount.ToString(), directoryCount);

                    BackgroundWorker ListSubDir = new BackgroundWorker();
                    ListSubDir.WorkerReportsProgress = true;
                    ListSubDir.DoWork += ListSubDir_DoWork;
                    ListSubDir.ProgressChanged += ListSubDir_ProgressChanged;
                    ListSubDir.RunWorkerCompleted += ListSubDir_RunWorkerCompleted;
                    ListSubDir.RunWorkerAsync(dialog.FileName + "/messages");
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

        

        

        private ProgressBar LoadProgressBar(string title, string content, int max)
        {
            ProgressBar progressBar = new ProgressBar();

            progressBar.Title = title;
            progressBar.lblProgress.Content = content;
            progressBar.proProgress.Maximum = max;

            progressBar.Show();

            return progressBar;
        }
        
        //##########----BACKGROUND WORKER----##########
        private void ListSubDir_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Close();

            lstDM.ItemsSource = DMList;
            CollectionView DMview = (CollectionView)CollectionViewSource.GetDefaultView(lstDM.ItemsSource);
            DMview.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            lstGDM.ItemsSource = GDMList;
            CollectionView GDMview = (CollectionView)CollectionViewSource.GetDefaultView(lstGDM.ItemsSource);
            GDMview.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            lstServers.ItemsSource = ServersList;
            CollectionView Serverview = (CollectionView)CollectionViewSource.GetDefaultView(lstServers.ItemsSource);
            Serverview.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }
        
        private void ListSubDir_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.proProgress.Value = e.ProgressPercentage;
            progressBar.lblProgress.Content = (e.ProgressPercentage + 1).ToString() + " / " + directoryCount.ToString();

            if (e.UserState != null)
            {
                ChannelJson json = (ChannelJson)e.UserState;
                switch (json.Type)
                {
                    case "3":
                        GDMList.Add(json);
                        break;
                    case "1":
                        DMList.Add(json);
                        break;
                    case "0":
                        ServersList.Add(json);
                        break;
                }
            }
        }

        private void ListSubDir_DoWork(object sender, DoWorkEventArgs e)
        {
            string dir = (string)e.Argument;
            var directories = new List<string>(Directory.GetDirectories(dir));

            for (int I = 0; I < directories.Count(); I++)
            {
                ChannelJson json = MessageType(directories[I]);

                (sender as BackgroundWorker).ReportProgress(I, json);

            }
            System.Threading.Thread.Sleep(500);
        }
        //##########################################################################################
        
    }
}
