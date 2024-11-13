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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;

namespace GraWZycie
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly MainWindow _mainWindow;

        private readonly GameViewModel _game;
        private bool _isReturnToMainMenu { get; set; }


        public GameWindow(MainWindow mainWindow, int rows, int cols)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            _game = new GameViewModel(rows, cols, Properties.Settings.Default.Ruleset, new NavigationService(), new UserMessageService());
            DataContext = _game;

            CreateGrid();
            CreateKeyBindings();

        }

        private void CreateKeyBindings()
        {
            AddBinding(_game.ReturnToMainMenuCommand, Key.Escape);
            AddBinding(_game.NextGenerationCommand, Key.A);
            AddBinding(_game.RandomiseCommand, Key.R);
            AddBinding(_game.CleanCommand, Key.C);
            AddBinding(_game.ToggleAutoplayCommand, Key.P);
            AddBinding(_game.ShowStatsCommand, Key.I);
            AddBinding(_game.SpeedUpCommand, Key.Up);
            AddBinding(_game.SlowDownCommand, Key.Down);
        }

        private void CreateGrid()
        {

            for (int row = 0; row < _game.Rows; row++)
            {
                Board.RowDefinitions.Add(new RowDefinition());

                for (int col = 0; col < _game.Cols; col++)
                {
                    if (row == 0)
                        Board.ColumnDefinitions.Add(new ColumnDefinition());


                    Button button = new Button();
                    var cell = _game.Cells[row][col];
                    button.DataContext = cell;
                    button.Template = (ControlTemplate)FindResource("ButtonTemplate");
                    button.Command = cell.ClickCommand;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);

                    Board.Children.Add(button);
                }
            }
        }

        public void ReturnToMainMenu()
        {
            _isReturnToMainMenu = true;
            _mainWindow.Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!_isReturnToMainMenu)
                _mainWindow.Close();

            base.OnClosed(e);
        }

        private void AddBinding(ICommand command, Key key)
        {
            InputBindings.Add(new KeyBinding() { Command = command, Key = key });
        }
    }
}
