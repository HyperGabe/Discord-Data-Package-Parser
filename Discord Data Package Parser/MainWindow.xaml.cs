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
using CsvHelper;

namespace Discord_Data_Package_Parser
{
    public class ChannelJson
    {
        public string Type          { get; set; }
        public string ID            { get; set; }
        public string Name          { get; set; }
    }
    public class MessageCSV
    {
        public string ID            { get; set; }
        public string Timestamp     { get; set; }
        public string Contents      { get; set; }
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

        private void LoadProgressBar(string title, int max)
        {
            lblTask.Content = title;
            lblProgress.Content = "0 / " + max.ToString();
            pbIndexing.Value = 0;
            pbIndexing.Maximum = max;
            grdPopup.Visibility = Visibility.Visible;

        }

        private void Clear()
        {
            lstDM.Items.Clear();
            lstServers.Items.Clear();
            lstGDM.Items.Clear();
            directoryCount = 0;
        }

        string dataDIR;
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
                    dataDIR = dialog.FileName;
                    string messages = dataDIR + "/messages";
                    var directories = new List<string>(Directory.GetDirectories(messages));
                    string json = File.ReadAllText(messages + "/index.json");
                    index = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    directoryCount = directories.Count();

                    LoadProgressBar("Indexing Channels...", directoryCount);

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

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.IsSelected)
            {
                ChannelJson json = item.DataContext as ChannelJson;

                LoadMessages(json.ID);             
            }
        }

        //##########----Message Loader----##########
        private void LoadMessages(string ID)
        {
            string messageDIR = dataDIR + "/messages/" + ID;
            if(File.Exists(messageDIR + "/messages.csv"))
            {
                using (var reader = new StreamReader(messageDIR + "/messages.csv"))
                using (var csv = new CsvReader(reader))
                {
                    List<MessageCSV> records = csv.GetRecords<MessageCSV>().ToList();
                    lstMessages.ItemsSource = records;
                }
            }
            else
            {
                MessageBox.Show("No message CSV found", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
            }           
        }
        //##########################################



        //##########----BACKGROUND WORKER----##########
        private void ListSubDir_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grdPopup.Visibility = Visibility.Hidden;

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
            pbIndexing.Value = e.ProgressPercentage;
            lblProgress.Content = (e.ProgressPercentage + 1).ToString() + " / " + directoryCount.ToString();

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
        //#############################################



    }
}
