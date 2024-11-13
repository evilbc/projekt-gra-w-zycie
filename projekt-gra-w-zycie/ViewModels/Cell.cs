using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace GraWZycie.ViewModels
{
    public class Cell : INotifyPropertyChanged
    {
        private bool _isAlive;
        public bool IsAlive
        {
            get => _isAlive;
            set
            {
                _isAlive = value;
                OnPropertyChanged(nameof(IsAlive));
                CellStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Row { get; }
        public int Col { get; }

        public ICommand ClickCommand { get; }
        public event EventHandler CellStateChanged;


        public Cell(bool isAlive, int row, int col)
        {
            IsAlive = isAlive;
            Row = row;
            Col = col;
            ClickCommand = new RelayCommand(ChangeState);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void ChangeState()
        {
            IsAlive = !IsAlive;
        }
    }

}
