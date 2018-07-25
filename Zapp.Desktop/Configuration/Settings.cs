using Caliburn.Micro;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Events;

// ReSharper disable once CheckNamespace
// Namespace must match the partial class in Settings.Designer.cs (auto-generated file).
namespace Zapp.Desktop.Properties
{
    internal partial class Settings : ISettings
    {
        private IEventAggregator _eventAggregator;

        internal Settings()
        {
            Reset();
            Save();
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
