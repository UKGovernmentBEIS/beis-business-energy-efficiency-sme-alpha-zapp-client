using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;

namespace RemindSME.Desktop.ViewModels
{
    public abstract class Notification : ViewAware
    {
        protected Notification()
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode != PowerModes.Suspend)
            {
                return;
            }
            var view = (DependencyObject)GetView();
            Window.GetWindow(view)?.Close();
        }
    }
}
