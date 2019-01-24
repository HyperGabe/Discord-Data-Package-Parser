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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Discord_Data_Package_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        private void Clear()
        {
            lstDM.Items.Clear();
            lstServers.Items.Clear();
        }
    }
}
