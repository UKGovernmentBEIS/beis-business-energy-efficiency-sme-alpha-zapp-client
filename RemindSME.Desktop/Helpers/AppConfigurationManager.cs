using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Caliburn.Micro;

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

        private readonly ILog log;

        public AppConfigurationManager(ILog log)
        {
            this.log = log;
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
                log.Error(e);
            }
        }
    }
}
