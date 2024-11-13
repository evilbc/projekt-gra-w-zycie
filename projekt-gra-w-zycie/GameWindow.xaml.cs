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
        private const int Rows = 50;

        private const int Cols = 50;

        private DispatcherTimer _gameTimer { get; }
        private readonly GameViewModel _game;
        private bool _isReturnToMainMenu { get; set; }


        public GameWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            _game = new GameViewModel(Rows, Cols, Properties.Settings.Default.Ruleset);
            DataContext = _game;

            CreateGrid();
            CreateKeyBindings();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += (sender, args) => _game.CalculateNewGeneration();
        }

        private void CreateKeyBindings()
        {
            AddBinding(new RelayCommand(ReturnToMainMenu), Key.Escape);
            AddBinding(_game.NextGenerationCommand, Key.A);
            AddBinding(_game.RandomiseCommand, Key.R);
            AddBinding(_game.CleanCommand, Key.C);
            AddBinding(new RelayCommand(ToggleAutoplay), Key.P);
            AddBinding(new RelayCommand(ShowStats), Key.I);
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

        private void ReturnToMainMenu()
        {
            _isReturnToMainMenu = true;
            _mainWindow.Show();
            Close();
        }

        private void ToggleAutoplay()
        {
            if (_gameTimer.IsEnabled)
                _gameTimer.Stop();
            else
                _gameTimer.Start();
        }

        private void ShowStats()
        {
            MessageBox.Show($"Liczba pokoleń: {_game.GenerationCount}, liczba umarłych: {_game.DeathCount}, liczba urodzonych: {_game.BirthCount}");
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
