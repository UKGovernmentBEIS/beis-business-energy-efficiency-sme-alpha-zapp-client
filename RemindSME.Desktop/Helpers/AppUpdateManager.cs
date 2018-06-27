using System.Threading.Tasks;
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
                System.Windows.Forms.Application.Restart();
            }
        }
    }
}
