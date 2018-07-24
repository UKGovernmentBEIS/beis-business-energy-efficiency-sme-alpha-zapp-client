using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Logging;
using RemindSME.Desktop.Services;
using RemindSME.Desktop.Views;
using static RemindSME.Desktop.Logging.TrackedActions;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ICompanyApiClient companyApiClient;
        private readonly INetworkService networkService;
        private readonly ISettings settings;
        private readonly IActionLog log;

        private bool _isWorkNetwork = true;

        public WelcomeViewModel(
            IAppWindowManager appWindowManager,
            ICompanyApiClient companyApiClient,
            INetworkService networkService,
            ISettings settings,
            IActionLog log)
        {
            this.appWindowManager = appWindowManager;
            this.companyApiClient = companyApiClient;
            this.networkService = networkService;
            this.settings = settings;
            this.log = log;
        }

        public string CompanyIdInput
        {
            get => settings.CompanyId ?? "";
            set
            {
                if (value.Equals(settings.CompanyId))
                {
                    return;
                }

                settings.CompanyId = value;
                UpdateCompanyName(value);
                NotifyOfPropertyChange(() => CompanyIdInput);
            }
        }

        public string CompanyName => settings.CompanyName ?? "Company not found";

        public bool NextIsVisible => !string.IsNullOrEmpty(settings.CompanyName);

        public bool IsWorkNetwork
        {
            get => _isWorkNetwork;
            set
            {
                if (value.Equals(_isWorkNetwork))
                {
                    return;
                }
                _isWorkNetwork = value;
                NotifyOfPropertyChange(() => IsWorkNetwork);
            }
        }

        public void Next()
        {
            networkService.AddCurrentNetwork(IsWorkNetwork);
            settings.DisplaySettingExplanations = true;
            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();

            var isOptingInHeating = settings.HeatingOptIn;
            var trackedActionHeating = isOptingInHeating ? OptInToHeating : OptOutOfHeating;
            log.Info(trackedActionHeating, $"User opted {(isOptingInHeating ? "in to" : "out of")} heating notifications.");

            var isOptingInHibernation = settings.HibernationOptIn;
            var trackedActionHibernation = isOptingInHibernation ? OptInToHibernate : OptOutOfHibernate;
            log.Info(trackedActionHibernation, $"User opted {(isOptingInHibernation ? "in to" : "out of")} scheduled hibernation.");

            (GetView() as Window)?.Close();
        }

        private async void UpdateCompanyName(string companyId)
        {
            await companyApiClient.UpdateCompanyName(companyId);
            NotifyOfPropertyChange(() => CompanyName);
            NotifyOfPropertyChange(() => NextIsVisible);
        }
    }
}
