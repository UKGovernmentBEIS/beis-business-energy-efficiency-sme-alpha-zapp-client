using System;
using System.Net.NetworkInformation;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop.Services
{
    public interface INetworkService : IService
    {
        bool IsWorkNetwork { get; }
        void AddCurrentNetwork(bool isWorkNetwork);
    }

    public class NetworkService : INetworkService
    {
        private static string currentNetwork;
        private static bool isShowingNotification;

        private readonly IEventAggregator eventAggregator;
        private readonly ILog log;
        private readonly INetworkAddressFinder networkAddressFinder;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;

        public NetworkService(
            IEventAggregator eventAggregator,
            ILog log,
            INetworkAddressFinder networkAddressFinder,
            INotificationManager notificationManager,
            ISettings settings)
        {
            this.eventAggregator = eventAggregator;
            this.log = log;
            this.networkAddressFinder = networkAddressFinder;
            this.notificationManager = notificationManager;
            this.settings = settings;
        }

        public bool IsWorkNetwork => settings.WorkNetworks.Contains(currentNetwork);

        public void Initialize()
        {
            currentNetwork = networkAddressFinder.GetCurrentNetworkAddress();
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        public void AddCurrentNetwork(bool isWorkNetwork)
        {
            if (isWorkNetwork)
            {
                settings.WorkNetworks.Add(currentNetwork);
            }
            else
            {
                settings.OtherNetworks.Add(currentNetwork);
            }
            settings.Save();
            PublishNetworkAddressChangeEvent();
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            currentNetwork = networkAddressFinder.GetCurrentNetworkAddress();

            if (string.IsNullOrEmpty(currentNetwork) || currentNetwork.StartsWith("127."))
            {
                return;
            }

            PublishNetworkAddressChangeEvent();

            if (settings.WorkNetworks.Count == 0 && settings.OtherNetworks.Count == 0)
            {
                // During initial registration.
                return;
            }

            var networkIsNew = IsNewNetwork(currentNetwork);
            if (networkIsNew && !isShowingNotification)
            {
                ShowNewNetworkNotification();
            }
        }

        private bool IsNewNetwork(string network)
        {
            return !(settings.WorkNetworks.Contains(network) || settings.OtherNetworks.Contains(network));
        }

        private void ShowNewNetworkNotification()
        {
            isShowingNotification = true;
            var model = IoC.Get<NewNetworkNotificationViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2), onClose: () => isShowingNotification = false);
            log.Info("Showed new network notification.");
        }

        private void PublishNetworkAddressChangeEvent()
        {
            eventAggregator.PublishOnUIThread(new NetworkAddressChangedEvent(IsWorkNetwork));
        }
    }
}
