using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ICompanyApiClient companyApiClient;

        public WelcomeViewModel(IAppWindowManager appWindowManager, ICompanyApiClient companyApiClient)
        {
            this.appWindowManager = appWindowManager;
            this.companyApiClient = companyApiClient;
        }

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

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
            // if it is a work network
            // then add to system settings
        }

        public string CompanyName => Settings.Default.CompanyName ?? "Company not found";

        public bool CanOpenHubWindow => Settings.Default.CompanyName != null;

        private async void UpdateCompanyName(string companyId )
        {
            await companyApiClient.UpdateCompanyName(companyId);
            NotifyOfPropertyChange(() => CompanyName);
            NotifyOfPropertyChange(() => CanOpenHubWindow);
        }
    }
}
