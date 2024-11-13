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
        private readonly IFileService _fileService;
        public ObservableCollection<Cell> Cells { get; }
        public int Rows => Game.Rows;
        public int Cols => Game.Cols;
        public ICommand NextGenerationCommand { get; }
        public ICommand RandomiseCommand { get; }
        public ICommand CleanCommand { get; }
        public ICommand ReturnToMainMenuCommand { get; }
        public ICommand ToggleAutoplayCommand { get; }
        public ICommand SpeedUpCommand { get; }
        public ICommand SlowDownCommand { get; }
        public ICommand ShowStatsCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private DispatcherTimer _gameTimer { get; }
        private List<TimeSpan> _timerSpeeds = [TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.75), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(2)];
        private int _timerSpeedIndex = 3;


        private Game Game { get; set; }



        public GameViewModel(int rows, int cols, string rules, INavigationService navigationService, IUserMessageService userMessageService, IFileService fileService)
        {
            _navigationService = navigationService;
            _userMessageService = userMessageService;
            _fileService = fileService;

            Game = new Game(rows, cols, rules);

            Cells = new ObservableCollection<Cell>();
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
            SaveCommand = new RelayCommand(SaveToFile);
            LoadCommand = new RelayCommand(LoadFromFile);
        }

        public void CalculateNewGeneration()
        {
            Game.CalculateNewGeneration();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cells[GetCellIndex(row, col)].IsAlive = Game.Cells[row, col];
                }
            }

            OnPropertyChanged(nameof(Cells));
        }

        private void InitCells()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var cell = new Cell(Game.Cells[row, col], row, col);
                    cell.CellStateChanged += CellStateChangedHandler;
                    Cells.Add(cell);
                }
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
                Game.Cells[cell.Row, cell.Col] = cell.IsAlive;
            }
        }

        private void Randomise()
        {
            var r = new Random();
            foreach (var cell in Cells)
            {
                cell.IsAlive = r.NextDouble() > 0.5;
            }
        }

        private void Clean()
        {
            foreach (var cell in Cells)
            {
                cell.IsAlive = false;
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
            _userMessageService.ShowMessage($"Generation count: {Game.GenerationCount}, death count: {Game.DeathCount}, birth count: {Game.BirthCount}");
        }

        private void SaveToFile()
        {
            string save = Game.CreateSaveState();
            _fileService.SaveToFile(save);

        }

        private void LoadFromFile()
        {
            string? save = _fileService.LoadFromFile("Choose your save file");
            if (save == null)
                return;
            
            var newGame = Game.LoadSaveState(save);
            Game = newGame;

            Cells.Clear();
            InitCells();
            // for (int row = 0; row < Rows; row++)
            // {
            //     for (int col = 0; col < Cols; col++)
            //     {
            //         Cells[GetCellIndex(row, col)].IsAlive = Game.Cells[row, col];
            //     }
            // }
            OnPropertyChanged(nameof(Cells));
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Cols));
        }

        private int GetCellIndex(int row, int col)
        {
            return row * Cols + col;
        }

    }
}
