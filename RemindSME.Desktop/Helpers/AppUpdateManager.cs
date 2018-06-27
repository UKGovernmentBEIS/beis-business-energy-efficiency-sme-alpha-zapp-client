using System.Threading.Tasks;
using System.Windows;
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
                RestartApp();
            }
        }

        private static void RestartApp()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
