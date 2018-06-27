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

    public class AppUpdateManager : IAppUpdateManager
    {
        private const string UpdateUrl = "https://reminds-me-server.herokuapp.com/Releases";

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
                await Task.Delay(TimeSpan.FromMinutes(1));
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
