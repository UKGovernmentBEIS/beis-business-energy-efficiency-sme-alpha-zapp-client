﻿using Caliburn.Micro;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Services;

namespace Zapp.Desktop.ViewModels
{
    public class NewNetworkNotificationViewModel : Notification
    {
        private readonly ILog log;
        private readonly INetworkService networkService;
        private readonly ISettings settings;

        public NewNetworkNotificationViewModel(
            ILog log,
            INetworkService networkService,
            ISettings settings,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.log = log;
            this.networkService = networkService;
            this.settings = settings;
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
    }
}
