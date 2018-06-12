using System.Windows;

namespace RemindsSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel
    {
        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
