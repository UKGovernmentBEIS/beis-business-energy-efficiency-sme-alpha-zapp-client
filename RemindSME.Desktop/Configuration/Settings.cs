using RemindSME.Desktop.Configuration;

// ReSharper disable once CheckNamespace
namespace RemindSME.Desktop.Properties
{
    internal partial class Settings : ISettings
    {
        internal Settings()
        {
            PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Save();
        }
    }
}
