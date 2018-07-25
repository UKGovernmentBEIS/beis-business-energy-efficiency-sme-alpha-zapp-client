using Microsoft.Win32;

namespace Zapp.Desktop.Helpers
{
    public interface IRegistryManager
    {
        void CreateEntryToLaunchOnStartup();
        void RemoveEntryToLaunchOnStartup();
    }

    public class RegistryManager : IRegistryManager
    {
        private const string StartupKeyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppKeyName = "BEIS Zapp";

        private static RegistryKey StartupRegistryKey => Registry.CurrentUser.OpenSubKey(StartupKeyName, true);

        public void CreateEntryToLaunchOnStartup()
        {
            var startupKey = StartupRegistryKey;
            if (startupKey != null)
            {
                startupKey.SetValue(AppKeyName, AppInfo.Location);
                startupKey.Close();
            }
        }

        public void RemoveEntryToLaunchOnStartup()
        {
            var startupKey = StartupRegistryKey;
            if (startupKey != null)
            {
                startupKey.DeleteValue(AppKeyName);
                startupKey.Close();
            }
        }
    }
}
