using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace RemindSME.Desktop.Helpers
{
    public interface IAppConfigurationManager
    {
        void BackupSettings();
        void RestoreSettings();
    }

    public class AppConfigurationManager : IAppConfigurationManager
    {
        private static readonly string SettingsFilePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
        private static readonly string BackupFilePath = Path.Combine(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName,
            "backup.config");

        private readonly IActionTracker actionTracker;

        public AppConfigurationManager(IActionTracker actionTracker)
        {
            this.actionTracker = actionTracker;
        }

        public void BackupSettings()
        {
            File.Copy(SettingsFilePath, BackupFilePath, true);
        }

        public void RestoreSettings()
        {
            // Restore settings from a backup file.
            var sourceFile = BackupFilePath;
            var destFile = SettingsFilePath;

            if (!File.Exists(sourceFile))
            {
                return;
            }

            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                File.Copy(sourceFile, destFile, true);
                File.Delete(sourceFile);
            }
            catch (Exception e)
            {
                actionTracker.Log($"Error occurred while restoring settings: {e.Message}");
            }
        }
    }
}
