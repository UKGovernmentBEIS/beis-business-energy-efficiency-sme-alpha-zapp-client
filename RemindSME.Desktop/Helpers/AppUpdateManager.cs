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

        public AppUpdateManager(IUpdateManager updateManager, IRegistryManager registryManager)
        {
            this.updateManager = updateManager;
            this.registryManager = registryManager;

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: version => PerformInstallActions(),
                onAppUpdate: version => PerformInstallActions(),
                onAppUninstall: version => PerformUninstallActions());
        }

        private void PerformInstallActions()
        {
            updateManager.CreateShortcutForThisExe();
            registryManager.CreateEntryToLaunchOnStartup();
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
