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
using System.Windows.Media.Media3D;
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
        public ICommand NextGenerationCommand => new RelayCommand(CalculateNewGeneration);
        public ICommand RandomiseCommand => new RelayCommand(Randomise);
        public ICommand CleanCommand => new RelayCommand(Clean);
        public ICommand ReturnToMainMenuCommand => new RelayCommand(_navigationService.ReturnToMainMenu);
        public ICommand ToggleAutoplayCommand => new RelayCommand(ToggleAutoplay);
        public ICommand SpeedUpCommand => new RelayCommand(() => ChangeTempo(true));
        public ICommand SlowDownCommand => new RelayCommand(() => ChangeTempo(false));
        public ICommand ShowStatsCommand => new RelayCommand(ShowStats);
        public ICommand SaveCommand => new RelayCommand(SaveToFile);
        public ICommand LoadCommand => new RelayCommand(LoadFromFile);
        public ICommand ZoomInCommand => new RelayCommand(ZoomIn);
        public ICommand ZoomOutCommand => new RelayCommand(ZoomOut);
        public ICommand ChangeShapeCommand => new RelayCommand(() => IsCircle = !IsCircle);

        private double _availableWidth;
        public double AvailableWidth
        {
            get => _availableWidth;
            set
            {
                _availableWidth = value;
                OnPropertyChanged(nameof(CellWidth));
            }
        }
        private double _availableHeight;
        public double AvailableHeight
        {
            get => _availableHeight;
            set
            {
                _availableHeight = value;
                OnPropertyChanged(nameof(CellHeight));
            }
        }
        public double CellWidth => AvailableWidth / Cols * Zoom;
        public double CellHeight => AvailableHeight / Rows * Zoom;

        private DispatcherTimer _gameTimer { get; }
        private List<TimeSpan> _timerSpeeds = [TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.75), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(2)];
        private int _timerSpeedIndex = 3;

        private double _zoom = 1.0;
        public double Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                OnPropertyChanged(nameof(Zoom));
                OnPropertyChanged(nameof(CellHeight));
                OnPropertyChanged(nameof(CellWidth));
                OnPropertyChanged(nameof(IsScrollVisible));
                OnPropertyChanged(nameof(CornerRadius));
            }
        }
        public bool IsScrollVisible => Zoom > 1.0;

        public CornerRadius CornerRadius => new CornerRadius(IsCircle ? Math.Max(CellWidth, CellHeight) : 0);

        private bool _isCircle = false;

        public bool IsCircle
        {
            get => _isCircle;
            set
            {
                _isCircle = value;
                OnPropertyChanged(nameof(IsCircle));
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        private Game _game;

        private Game Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged(nameof(Cols));
                OnPropertyChanged(nameof(Rows));
            }
        }



        public GameViewModel(int rows, int cols, string rules, INavigationService navigationService, IUserMessageService userMessageService, IFileService fileService)
        {
            _navigationService = navigationService;
            _userMessageService = userMessageService;
            _fileService = fileService;

            _game = new Game(rows, cols, rules);

            Cells = new ObservableCollection<Cell>();
            InitCells();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = _timerSpeeds[_timerSpeedIndex];
            _gameTimer.Tick += (sender, args) => CalculateNewGeneration();
        }

        public void CalculateNewGeneration()
        {
            Game.CalculateNewGeneration();
            int i = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cells[i++].IsAlive = Game.Cells[row, col];
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
            Cells.AsParallel().ForAll(cell => cell.IsAlive = Random.Shared.NextDouble() > 0.5);
        }

        private void Clean()
        {
            Cells.AsParallel().ForAll(cell => cell.IsAlive = false);
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
            OnPropertyChanged(nameof(Cells));
        }

        private int GetCellIndex(int row, int col)
        {
            return row * Cols + col;
        }

        private void ZoomIn()
        {
            Zoom += 0.1;
        }

        private void ZoomOut()
        {
            Zoom = Math.Max(1, Zoom - 0.1);
        }


    }
}
