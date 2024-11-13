using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraWZycie.Services
{
    public interface IFileService
    {
        void SaveToFile(string data);

        string LoadFromFile();
    }

    internal class FileService: IFileService
    {
        public void SaveToFile(string data)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File As",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
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

        public string LoadFromFile()
        {
            throw new NotImplementedException();
        }
    }
}
