using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using RemindSME.Desktop.Views;
using Squirrel;

namespace RemindSME.Desktop.Helpers
{
    public interface IAppUpdateManager
    {
        void SetupInstallerEventHandlers();
        Task<bool> CheckForUpdate();
        Task UpdateAndRestart();
    }

    public class AppUpdateManager : IAppUpdateManager, IDisposable
    {
        private readonly IUpdateManager updateManager;

        public AppUpdateManager(IUpdateManager updateManager)
        {
            this.updateManager = updateManager;
        }

        public void SetupInstallerEventHandlers()
        {
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: version => InstallActions(),
                onAppUpdate: version => InstallActions(),
                onAppUninstall: version => UninstallActions());
        }

        private void InstallActions()
        {
            updateManager.CreateShortcutForThisExe();
            CreateRegistryEntryToLaunchOnStartup();
        }

        private void UninstallActions()
        {
            updateManager.RemoveShortcutForThisExe();
            RemoveRegistryEntryToLaunchOnStartup();
        }

        private static void CreateRegistryEntryToLaunchOnStartup()
        {
            var startupKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (startupKey != null)
            {
                var appName = AppInfo.Title;
                var executablePath = AppInfo.Location;

                startupKey.SetValue(appName, executablePath);
                startupKey.Close();
            }
        }

        private static void RemoveRegistryEntryToLaunchOnStartup()
        {
            var startupKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (startupKey != null)
            {
                var appName = AppInfo.Title;
                startupKey.DeleteValue(appName);
                startupKey.Close();
            }
        }

        public async Task<bool> CheckForUpdate()
        {
            var updateInfo = await updateManager.CheckForUpdate();
            return updateInfo.FutureReleaseEntry != updateInfo.CurrentlyInstalledVersion;
        }

        public async Task UpdateAndRestart()
        {
            await updateManager.UpdateApp();
            RestartWhenAllWindowsClosed();
        }

        private static async void RestartWhenAllWindowsClosed()
        {
            while (AnyWindowIsOpen())
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            UpdateManager.RestartApp();
            Application.Current.Shutdown();
        }

        private static bool AnyWindowIsOpen()
        {
            return Application.Current.Windows.Cast<Window>().Any(window => window is HubView);
        }

        public void Dispose()
        {
            updateManager?.Dispose();
        }
    }
}
