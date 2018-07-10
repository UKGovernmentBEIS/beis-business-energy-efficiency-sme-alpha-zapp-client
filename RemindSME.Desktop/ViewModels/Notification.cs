using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.ViewModels
{
    public abstract class Notification : ViewAware, IHandle<ResumeFromSuspendedStateEvent>
    {

        protected Notification(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode != PowerModes.Suspend)
            {
                return;
            }
            CloseNotification();
        }

        protected void CloseNotification()
        {
            var view = (DependencyObject)GetView();
            Window.GetWindow(view)?.Close();
        }

        public void Handle(ResumeFromSuspendedStateEvent message)
        {
            CloseNotification();
        }
    }
}
