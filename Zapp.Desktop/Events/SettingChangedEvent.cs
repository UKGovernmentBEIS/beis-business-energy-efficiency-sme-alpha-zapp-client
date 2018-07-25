namespace Zapp.Desktop.Events
{
    public class SettingChangedEvent
    {
        public SettingChangedEvent(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; }
    }
}
