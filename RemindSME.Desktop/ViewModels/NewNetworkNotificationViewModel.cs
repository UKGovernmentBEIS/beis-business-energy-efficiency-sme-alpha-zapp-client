using Caliburn.Micro;
using RemindSME.Desktop.Services;
using RemindSME.Desktop.Configuration;

namespace RemindSME.Desktop.ViewModels
{
    public class NewNetworkNotificationViewModel : Notification
    {
        private readonly INetworkService networkService;
        private readonly ILog log;
        private readonly ISettings settings;

        public NewNetworkNotificationViewModel(
            ILog log, 
            INetworkService networkService,
            ISettings settings)
        {
            this.log = log;
            this.networkService = networkService;
            this.settings = settings;
        }

        public void WorkNetwork()
        {
            log.Info("User clicked 'Work network' on new network notification.");
            networkService.AddCurrentNetwork(true);
        }

        public void OtherNetwork()
        {
            log.Info("User clicked 'Other network' on new network notification.");
            networkService.AddCurrentNetwork(false);
        }

        public string NewNetworkMessage
        {
            get
            {
                var firstLetterOfCompanyName = settings.CompanyName[0];
                var article = "aeiouAEIOU".IndexOf(firstLetterOfCompanyName) >= 0 ? "an" : "a";
                return $"We've detected that you're connected to a new network, is this {article} {settings.CompanyName} network?";
            }
        }
    }
}
