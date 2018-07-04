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
            get => Settings.Default.CompanyId == 0 ? "" : Settings.Default.CompanyId.ToString();
            set
            {
                if (value.Length == 6)
                {
                    companyApiClient.UpdateCompanyName(int.Parse(value));
                    NotifyOfPropertyChange(() => CompanyName);
                    NotifyOfPropertyChange(() => CanOpenHubWindow);

                }
                Settings.Default.CompanyId = value.Length == 0 ? 0 : int.Parse(value);
            }
            // if it is a work network
            // then add to system settings
        }

        public string CompanyName => Settings.Default.CompanyName ?? "Company not found";

        public bool CanOpenHubWindow => Settings.Default.CompanyName != null;
    }
}
