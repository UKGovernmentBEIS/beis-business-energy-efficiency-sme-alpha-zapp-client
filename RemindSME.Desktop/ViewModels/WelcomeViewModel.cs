using System;
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

        public WelcomeViewModel(IAppWindowManager appWindowManager)
        {
            this.appWindowManager = appWindowManager;
        }

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

            Settings.Default.DisplaySettingExplanations = true;
            Settings.Default.Save();

            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }

        public string CompanyCode
        {
            get => Settings.Default.CompanyCode == 0 ? "" : Settings.Default.CompanyCode.ToString();
            set => Settings.Default.CompanyCode = Int32.Parse(value);
            // if it is a work network
            // if X digits long, do a GET request to verify
            // if verified, show company name, otherwise show error and contact us message 
            // then add to system settings
        }
    }
}
