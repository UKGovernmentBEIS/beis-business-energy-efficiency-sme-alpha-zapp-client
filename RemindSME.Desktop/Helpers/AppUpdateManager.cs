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
        void SetupInstallerEventHandlers();
        Task<bool> CheckForUpdate();
        Task UpdateAndRestart();
    }

    public class AppUpdateManager : IAppUpdateManager, IDisposable
    {
        private readonly IUpdateManager updateManager;
        private readonly IRegistryManager registryManager;

        public AppUpdateManager(IUpdateManager updateManager, IRegistryManager registryManager)
        {
            this.updateManager = updateManager;
            this.registryManager = registryManager;
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
            registryManager.CreateEntryToLaunchOnStartup();
        }

        private void UninstallActions()
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
