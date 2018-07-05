using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;

// ReSharper disable once CheckNamespace
namespace RemindSME.Desktop.Properties
{
    internal partial class Settings : ISettings
    {
        private IEventAggregator _eventAggregator;

        internal Settings()
        {
            PropertyChanged += Settings_PropertyChanged;
        }

        private IEventAggregator EventAggregator => _eventAggregator ?? (_eventAggregator = IoC.Get<IEventAggregator>());

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Save();
            EventAggregator.PublishOnUIThread(new SettingChangedEvent(e.PropertyName));
        }
    }
}
