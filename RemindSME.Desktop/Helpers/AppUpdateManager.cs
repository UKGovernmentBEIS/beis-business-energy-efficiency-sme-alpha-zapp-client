using System;
using System.Threading.Tasks;
using Squirrel;

namespace RemindSME.Desktop.Helpers
{
    public interface IAppUpdateManager
    {
        void HandleUpdateEvents();
        Task<bool> CheckForUpdate();
        Task UpdateAndRestart();
    }

    // DEBUG
    public class DummyAppUpdateManager : IAppUpdateManager
    {
        public void HandleUpdateEvents() { }

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
        private readonly IActionTracker actionTracker;
        private readonly IUpdateManager updateManager;
        private readonly IRegistryManager registryManager;
        private readonly IAppConfigurationManager configurationManager;
        private readonly IAppWindowManager appWindowManager;

        public AppUpdateManager(
            IActionTracker actionTracker,
            IUpdateManager updateManager, 
            IRegistryManager registryManager, 
            IAppConfigurationManager configurationManager,
            IAppWindowManager appWindowManager)
        {
            this.actionTracker = actionTracker;
            this.updateManager = updateManager;
            this.registryManager = registryManager;
            this.configurationManager = configurationManager;
            this.appWindowManager = appWindowManager;
        }

        public void HandleUpdateEvents()
        {
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: version => PerformInstallActions(),
                onAppUpdate: version => PerformInstallActions(),
                onAppUninstall: version => PerformUninstallActions());
        }

        private void PerformInstallActions()
        {
            actionTracker.Log("Performing post-update actions.");
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
            actionTracker.Log("Checking for update.");
            var updateInfo = await updateManager.CheckForUpdate();
            return updateInfo.FutureReleaseEntry != updateInfo.CurrentlyInstalledVersion;
        }

        public async Task UpdateAndRestart()
        {
            actionTracker.Log("Updating app in background.");
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

            actionTracker.Log("Restarting after update.");
            UpdateManager.RestartApp();
        }

        public void Dispose()
        {
            updateManager?.Dispose();
        }
    }
}
