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

namespace GraWZycie
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private const int Rows = 50;
        private const int Cols = 50;
        // private DispatcherTimer gameTimer;
        private readonly GameViewModel Game;


        public GameWindow()
        {
            InitializeComponent();
            Game = new GameViewModel(Rows, Cols, Properties.Settings.Default.Ruleset);
            DataContext = Game;

            CreateGrid();

            // gameTimer = new DispatcherTimer();
            // gameTimer.Interval = TimeSpan.FromSeconds(1);
            // gameTimer.Tick += UpdateBoard;

            // ColorAnimation colorAnimation = new();
            // colorAnimation.From = Colors.White;
            // colorAnimation.To = Colors.Black;
            // colorAnimation.BeginTime = TimeSpan.FromSeconds(3);
            // Cells[80, 80].BeginAnimation(TopProperty, colorAnimation);
            //colorAnimation.
        }

        private void CreateGrid()
        {

            for (int row = 0; row < Game.Rows; row++)
            {
                Board.RowDefinitions.Add(new RowDefinition());

                for (int col = 0; col < Game.Cols; col++)
                {
                    if (row == 0)
                        Board.ColumnDefinitions.Add(new ColumnDefinition());


                    Button button = new Button();
                    var cell = Game.Cells[row][col];
                    button.DataContext = cell;
                    button.Template = (ControlTemplate) FindResource("ButtonTemplate");
                    button.Command = cell.ClickCommand;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);

                    Board.Children.Add(button);
                }
            }
        }
    }
}
