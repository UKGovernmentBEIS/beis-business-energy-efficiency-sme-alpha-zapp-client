using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using RemindSME.Desktop.Helpers;
using Squirrel;

namespace RemindSME.Desktop.Services
{
    public class AppUpdateService : IService, IDisposable
    {
        private readonly IActionTracker actionTracker;
        private readonly IAppWindowManager appWindowManager;
        private readonly DispatcherTimer timer;
        private readonly IAppConfigurationManager configurationManager;
        private readonly IRegistryManager registryManager;
        private readonly IUpdateManager updateManager;

        public AppUpdateService(
            IActionTracker actionTracker,
            IUpdateManager updateManager,
            IRegistryManager registryManager,
            IAppConfigurationManager configurationManager,
            IAppWindowManager appWindowManager,
            DispatcherTimer timer)
        {
            this.actionTracker = actionTracker;
            this.updateManager = updateManager;
            this.registryManager = registryManager;
            this.configurationManager = configurationManager;
            this.appWindowManager = appWindowManager;
            this.timer = timer;
        }

        public void Initialize()
        {
            timer.Interval = TimeSpan.FromMinutes(5);
            timer.Tick += UpdateTimer_TickAsync;
            timer.Start();

            SquirrelAwareApp.HandleEvents(
                version => PerformInstallActions(),
                version => PerformInstallActions(),
                onAppUninstall: version => PerformUninstallActions());
        }

        public void Dispose()
        {
            updateManager?.Dispose();
        }

        private async void UpdateTimer_TickAsync(object sender, EventArgs e)
        {
            var updateIsAvailable = await CheckForUpdate();
            if (updateIsAvailable)
            {
                await UpdateAndRestart();
            }
        }

        private async Task<bool> CheckForUpdate()
        {
            actionTracker.Log("Checking for update.");
            var updateInfo = await updateManager.CheckForUpdate();
            return updateInfo.FutureReleaseEntry != updateInfo.CurrentlyInstalledVersion;
        }

        private async Task UpdateAndRestart()
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
    }
}
