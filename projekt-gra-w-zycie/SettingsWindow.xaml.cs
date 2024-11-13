using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraWZycie
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            RulesTextBox.Text = Properties.Settings.Default.Ruleset;
            RowsTextBox.Text = Properties.Settings.Default.Rows.ToString();
            ColumnsTextBox.Text = Properties.Settings.Default.Cols.ToString();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string ruleset = RulesTextBox.Text;
            if (!ValidateRules(ruleset))
            {
                MessageBox.Show("Invalid ruleset");
                return;
            }

            if (int.TryParse(RowsTextBox.Text, out int r))
            {
                Properties.Settings.Default.Rows = r;
            }
            if (int.TryParse(ColumnsTextBox.Text, out int c))
            {
                Properties.Settings.Default.Cols = c;
            }
            Properties.Settings.Default.Ruleset = ruleset;
            Properties.Settings.Default.Save();

            Close();
        }

        private bool ValidateRules(string rules)
        {
            return Regex.IsMatch(rules, @"^B\d+\/S\d+$");
        }
    }
}
