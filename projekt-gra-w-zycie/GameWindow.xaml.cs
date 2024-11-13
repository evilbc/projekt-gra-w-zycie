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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using GraWZycie.Services;
using GraWZycie.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace GraWZycie
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly MainWindow _mainWindow;

        private readonly GameViewModel _game;
        private bool IsReturnToMainMenu { get; set; }


        public GameWindow(MainWindow mainWindow, int rows, int cols)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            _game = new GameViewModel(rows, cols, Properties.Settings.Default.Ruleset, new NavigationService(),
                new UserMessageService(), new FileService());
            DataContext = _game;
        }

        public void ReturnToMainMenu()
        {
            IsReturnToMainMenu = true;
            _mainWindow.Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!IsReturnToMainMenu)
                _mainWindow.Close();

            base.OnClosed(e);
        }

        private void PreviewZoomMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0) _game.ZoomInCommand.Execute(null);
                else _game.ZoomOutCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _game.AvailableWidth = e.NewSize.Width;
            _game.AvailableHeight = e.NewSize.Height;
        }

    }
}
