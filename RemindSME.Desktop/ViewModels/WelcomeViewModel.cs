using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;
using RemindSME.Desktop.Services;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ICompanyApiClient companyApiClient;
        private readonly INetworkService networkService;
        
        private bool isWorkNetwork = true;

        public WelcomeViewModel(
            IAppWindowManager appWindowManager,
            ICompanyApiClient companyApiClient,
            INetworkService networkService)
        {
            this.appWindowManager = appWindowManager;
            this.companyApiClient = companyApiClient;
            this.networkService = networkService;
        }

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

            networkService.AddNetwork(isWorkNetwork);

            Settings.Default.DisplaySettingExplanations = true;
            Settings.Default.Save();

            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }

        public string CompanyIdInput
        {
            get => Settings.Default.CompanyId == null ? "" : Settings.Default.CompanyId.ToString();
            set
            {
                if (value.Length == 6)
                {
                    UpdateCompanyName(value);
                }

                Settings.Default.CompanyId = value;
            }
        }

        public string CompanyName => Settings.Default.CompanyName ?? "Company not found";

        public bool CanOpenHubWindow => Settings.Default.CompanyName != null;

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
