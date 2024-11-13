using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using GraWZycie.Models;
using GraWZycie.Services;

namespace GraWZycie.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IUserMessageService _userMessageService;
        public ObservableCollection<ObservableCollection<Cell>> Cells { get; }
        public int Rows { get; }
        public int Cols { get; }
        public ICommand NextGenerationCommand { get; }
        public ICommand RandomiseCommand { get; }
        public ICommand CleanCommand { get; }
        public ICommand ReturnToMainMenuCommand { get; }
        public ICommand ToggleAutoplayCommand { get; }
        public ICommand SpeedUpCommand { get; }
        public ICommand SlowDownCommand { get; }
        public ICommand ShowStatsCommand { get; }

        private DispatcherTimer _gameTimer { get; }
        private List<TimeSpan> _timerSpeeds = [TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.75), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(2)];
        private int _timerSpeedIndex = 3;


        private readonly Game _game;



        public GameViewModel(int rows, int cols, string rules, INavigationService navigationService, IUserMessageService userMessageService)
        {
            _navigationService = navigationService;
            _userMessageService = userMessageService;
            Rows = rows;
            Cols = cols;
            _game = new Game(rows, cols, rules);

            Cells = new ObservableCollection<ObservableCollection<Cell>>();
            InitCells();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = _timerSpeeds[_timerSpeedIndex];
            _gameTimer.Tick += (sender, args) => CalculateNewGeneration();

            NextGenerationCommand = new RelayCommand(CalculateNewGeneration);
            RandomiseCommand = new RelayCommand(Randomise);
            CleanCommand = new RelayCommand(Clean);
            ReturnToMainMenuCommand = new RelayCommand(_navigationService.ReturnToMainMenu);
            ToggleAutoplayCommand = new RelayCommand(ToggleAutoplay);
            SpeedUpCommand = new RelayCommand(() => ChangeTempo(true));
            SlowDownCommand = new RelayCommand(() => ChangeTempo(false));
            ShowStatsCommand = new RelayCommand(ShowStats);
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

        private void InitCells()
        {
            for (int row = 0; row < Rows; row++)
            {
                var cellRow = new ObservableCollection<Cell>();
                for (int col = 0; col < Cols; col++)
                {
                    var cell = new Cell(false, row, col);
                    cell.CellStateChanged += CellStateChangedHandler;
                    cellRow.Add(cell);
                }
                Cells.Add(cellRow);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void CellStateChangedHandler(object? sender, EventArgs e)
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

        private void ToggleAutoplay()
        {
            if (_gameTimer.IsEnabled)
                _gameTimer.Stop();
            else
                _gameTimer.Start();
        }

        private void ChangeTempo(bool speedUp)
        {
            if (speedUp && _timerSpeedIndex > 0)
                _timerSpeedIndex--;
            else if (!speedUp && _timerSpeedIndex + 1 < _timerSpeeds.Count)
                _timerSpeedIndex++;
            _gameTimer.Interval = _timerSpeeds[_timerSpeedIndex];
        }

        private void ShowStats()
        {
            _userMessageService.ShowMessage($"Liczba pokoleń: {_game.GenerationCount}, liczba umarłych: {_game.DeathCount}, liczba urodzonych: {_game.BirthCount}");
        }



    }
}
