using System;
using System.Configuration;
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

    public class DummyAppUpdateManager : IAppUpdateManager
    {
        public Task<bool> CheckForUpdate()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAndRestart()
        {
            throw new NotImplementedException();
        }
    }

    public class AppUpdateManager : IAppUpdateManager, IDisposable
    {
        private readonly IUpdateManager updateManager;

        public AppUpdateManager(IUpdateManager updateManager)
        {
            this.updateManager = updateManager;

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: v => updateManager.CreateShortcutForThisExe(),
                onAppUpdate: v => updateManager.CreateShortcutForThisExe(),
                onAppUninstall: v => updateManager.RemoveShortcutForThisExe());
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
