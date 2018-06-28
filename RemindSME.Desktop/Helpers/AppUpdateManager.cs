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

    public class AppUpdateManager : IAppUpdateManager
    {
        private static readonly string UpdateUrl = ConfigurationManager.AppSettings["UpdateUrl"];

        public async Task<bool> CheckForUpdate()
        {
            using (var updateManager = new UpdateManager(UpdateUrl))
            {
                var updateInfo = await updateManager.CheckForUpdate();
                return updateInfo.FutureReleaseEntry != updateInfo.CurrentlyInstalledVersion;
            }
        }

        public async Task UpdateAndRestart()
        {
            using (var updateManager = new UpdateManager(UpdateUrl))
            {
                await updateManager.UpdateApp();
                RestartWhenAllWindowsClosed();
            }
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
    }
}
