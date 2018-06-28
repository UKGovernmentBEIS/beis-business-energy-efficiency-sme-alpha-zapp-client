using Microsoft.Win32;

namespace RemindSME.Desktop.Helpers
{
    public interface IRegistryManager
    {
        void CreateEntryToLaunchOnStartup();
        void RemoveEntryToLaunchOnStartup();
    }

    public class RegistryManager : IRegistryManager
    {
        private const string StartupKeyName = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public void CreateEntryToLaunchOnStartup()
        {
            var startupKey = Registry.CurrentUser.OpenSubKey(StartupKeyName, true);
            if (startupKey != null)
            {
                startupKey.SetValue(AppInfo.Title, AppInfo.Location);
                startupKey.Close();
            }
        }

        public void RemoveEntryToLaunchOnStartup()
        {
            var startupKey = Registry.CurrentUser.OpenSubKey(StartupKeyName, true);
            if (startupKey != null)
            {
                startupKey.DeleteValue(AppInfo.Title);
                startupKey.Close();
            }
        }
    }
}
