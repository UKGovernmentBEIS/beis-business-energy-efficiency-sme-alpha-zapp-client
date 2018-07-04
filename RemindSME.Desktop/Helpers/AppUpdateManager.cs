using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RemindSME.Desktop.Views;
using Squirrel;

namespace RemindSME.Desktop.Helpers
{
    public interface IAppUpdateManager
    {
        Task<bool> CheckForUpdate();
        Task UpdateAndRestart();
    }

    // DEBUG
    public class DummyAppUpdateManager : IAppUpdateManager
    {
        public Task<bool> CheckForUpdate()
        {
            return Task.FromResult(false);
        }

        public Task UpdateAndRestart()
        {
            return Task.CompletedTask;
        }
    }

    // RELEASE
    public class AppUpdateManager : IAppUpdateManager, IDisposable
    {
        private readonly IUpdateManager updateManager;
        private readonly IRegistryManager registryManager;
        private readonly IAppConfigurationManager configurationManager;
        private readonly IAppWindowManager appWindowManager;

        public AppUpdateManager(
            IUpdateManager updateManager, 
            IRegistryManager registryManager, 
            IAppConfigurationManager configurationManager,
            IAppWindowManager appWindowManager)
        {
            this.updateManager = updateManager;
            this.registryManager = registryManager;
            this.configurationManager = configurationManager;
            this.appWindowManager = appWindowManager;

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: version => PerformInstallActions(),
                onAppUpdate: version => PerformInstallActions(),
                onAppUninstall: version => PerformUninstallActions());
        }

        private void PerformInstallActions()
        {
            updateManager.CreateShortcutForThisExe();
            registryManager.CreateEntryToLaunchOnStartup();
            configurationManager.RestoreSettings();
        }

        private void PerformUninstallActions()
        {
            updateManager.RemoveShortcutForThisExe();
            registryManager.RemoveEntryToLaunchOnStartup();
        }

        public async Task<bool> CheckForUpdate()
        {
            var updateInfo = await updateManager.CheckForUpdate();
            return updateInfo.FutureReleaseEntry != updateInfo.CurrentlyInstalledVersion;
        }

        public async Task UpdateAndRestart()
        {
            configurationManager.BackupSettings();
            await updateManager.UpdateApp();
            RestartWhenAllWindowsClosed();
        }

        private async void RestartWhenAllWindowsClosed()
        {
            while (appWindowManager.AnyAppWindowIsOpen())
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            UpdateManager.RestartApp();
        }
        public void Dispose()
        {
            updateManager?.Dispose();
        }
    }
}
