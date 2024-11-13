using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GraWZycie.Services
{
    public interface IFileService
    {
        void SaveToFile(string data);

        string? LoadFromFile(string prompt);
    }

    internal class FileService: IFileService
    {
        private const string Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

        public void SaveToFile(string data)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File As",
                Filter = Filter,
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, data);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        public string? LoadFromFile(string prompt)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = prompt,
                Filter = Filter,
                DefaultExt = "txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return File.ReadAllText(openFileDialog.FileName);
            }

            return null;
        }
    }
}
