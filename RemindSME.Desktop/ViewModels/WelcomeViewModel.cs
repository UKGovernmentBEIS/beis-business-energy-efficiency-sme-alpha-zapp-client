using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Services;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ICompanyApiClient companyApiClient;
        private readonly INetworkService networkService;
        private readonly ISettings settings;

        private bool _isWorkNetwork = true;

        public WelcomeViewModel(
            IAppWindowManager appWindowManager,
            ICompanyApiClient companyApiClient,
            INetworkService networkService,
            ISettings settings)
        {
            this.appWindowManager = appWindowManager;
            this.companyApiClient = companyApiClient;
            this.networkService = networkService;
            this.settings = settings;
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
