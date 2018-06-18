using Caliburn.Micro;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : PropertyChangedBase
    {
        public bool HeatingOptIn
        {
            get => Settings.Default.HeatingOptIn;
            set
            {
                if (value == Settings.Default.HeatingOptIn)
                {
                    return;
                }
                Settings.Default.HeatingOptIn = value;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => HeatingOptIn);
            }
        }

        public bool LightingOptIn
        {
            get => Settings.Default.LightingOptIn;
            set
            {
                if (value == Settings.Default.LightingOptIn)
                {
                    return;
                }
                Settings.Default.LightingOptIn = value;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => LightingOptIn);
            }
        }
    }
}