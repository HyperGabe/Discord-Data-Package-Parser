using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Discord_Data_Package_Parser
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}