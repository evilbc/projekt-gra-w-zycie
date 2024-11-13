using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraWZycie
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

        private void ShowGameWindow(object sender, RoutedEventArgs e)
        {
            new GameWindow(this, Properties.Settings.Default.Rows, Properties.Settings.Default.Cols).Show();
            Hide();
        }

        private void ShowSettingsWindow(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}