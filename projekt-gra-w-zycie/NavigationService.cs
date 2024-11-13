using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraWZycie
{
    public interface INavigationService
    {
        void ReturnToMainMenu();
    }

    internal class NavigationService : INavigationService
    {
        public void ReturnToMainMenu()
        {
            Application.Current.Windows.OfType<GameWindow>().SingleOrDefault(w => w.IsActive)?.ReturnToMainMenu();
        }
    }
}
