using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Views;
using RemindSME.Desktop.Services;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ICompanyApiClient companyApiClient;
        private readonly INetworkService networkService;
        private readonly ISettings settings;

        private bool isWorkNetwork = true;

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

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

            networkService.AddNetwork(isWorkNetwork);

            settings.DisplaySettingExplanations = true;

            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }

        public string CompanyIdInput
        {
            get => settings.CompanyId == null ? "" : settings.CompanyId.ToString();
            set
            {
                if (value.Length == 6)
                {
                    UpdateCompanyName(value);
                }

                settings.CompanyId = value;
            }
        }

        public string CompanyName => settings.CompanyName ?? "Company not found";

        public bool CanOpenHubWindow => settings.CompanyName != null;

        private async void UpdateCompanyName(string companyId )
        {
            await companyApiClient.UpdateCompanyName(companyId);
            NotifyOfPropertyChange(() => CompanyName);
            NotifyOfPropertyChange(() => CanOpenHubWindow);
        }

        public bool YesButton
        {
            get => isWorkNetwork;
            set
            {
                if (value.Equals(isWorkNetwork)) return;
                isWorkNetwork = value;
                NotifyOfPropertyChange(() => YesButton);
            }
        }
    }
}
