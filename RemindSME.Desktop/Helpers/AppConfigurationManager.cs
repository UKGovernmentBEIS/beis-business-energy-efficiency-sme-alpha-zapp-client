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

    public class AppAppConfigurationManager : IAppConfigurationManager
    {
        private static string SettingsFilePath => System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
        private static string BackupFilePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\backup.config";

        private readonly IActionTracker actionTracker;

        public AppAppConfigurationManager(IActionTracker actionTracker)
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
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            }
            catch (Exception)
            {
                actionTracker.Log("ERROR: Failed to locate settings file folder during restore.");
            }

            try
            {
                File.Copy(sourceFile, destFile, true);
            }
            catch (Exception)
            {
                actionTracker.Log("ERROR: Failed to copy backup settings file during restore.");
            }

            try
            {
                File.Delete(sourceFile);
            }
            catch (Exception)
            {
                actionTracker.Log("ERROR: Failed to delete backup settings file during restore.");
            }
        }
    }
}
