using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;

namespace RemindSME.Desktop.ViewModels
{
    public abstract class Notification : ViewAware
    {
        protected Notification(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            CloseNotification();
        }

        protected void CloseNotification()
        {
            var view = (DependencyObject)GetView();
            if (view != null)
            {
                Window.GetWindow(view)?.Close();
            }
        }
    }
}
