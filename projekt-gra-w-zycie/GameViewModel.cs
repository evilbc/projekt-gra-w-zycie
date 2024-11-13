using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace GraWZycie
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ObservableCollection<Cell>> Cells { get; }
        public int Rows { get; }
        public int Cols { get; }
        public ICommand NextGenerationCommand { get; }
        public ICommand RandomiseCommand { get; }
        public ICommand CleanCommand { get; }
        public int GenerationCount => _game.GenerationCount;
        public int DeathCount => _game.DeathCount;
        public int BirthCount => _game.BirthCount;


        private readonly Game _game;



        public GameViewModel(int rows, int cols, string rules)
        {
            Random r = new();
            Cells = new ObservableCollection<ObservableCollection<Cell>>();
            for (int row = 0; row < rows; row++)
            {
                var cellRow = new ObservableCollection<Cell>();
                for (int col = 0; col < cols; col++)
                {
                    var cell = new Cell(false, row, col);
                    cell.CellStateChanged += CellStateChangedHandler;
                    cellRow.Add(cell);
                }
                Cells.Add(cellRow);
            }
            Rows = rows;
            Cols = cols;
            _game = new Game(rows, cols, rules);
            NextGenerationCommand = new RelayCommand(CalculateNewGeneration);
            RandomiseCommand = new RelayCommand(Randomise);
            CleanCommand = new RelayCommand(Clean);
        }

        public void CalculateNewGeneration()
        {
            _game.CalculateNewGeneration();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cells[row][col].IsAlive = _game.Cells[row, col];
                }
            }

            OnPropertyChanged(nameof(Cells));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void CellStateChangedHandler(object sender, EventArgs e)
        {
            if (sender is Cell cell)
            {
                _game.Cells[cell.Row, cell.Col] = cell.IsAlive;
            }
        }

        private void Randomise()
        {
            Random r = new();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cells[row][col].IsAlive = r.NextDouble() > 0.5;
                }
            }

        }

        private void Clean()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cells[row][col].IsAlive = false;
                }
            }
        }

    }
}
