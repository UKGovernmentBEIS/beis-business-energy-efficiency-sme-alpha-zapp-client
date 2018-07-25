using System;
using System.Collections.Specialized;

namespace Zapp.Desktop.Configuration
{
    public interface ISettings
    {
        DateTime NextHibernationTime { get; set; }
        bool HeatingOptIn { get; set; }
        bool HibernationOptIn { get; set; }
        TimeSpan DefaultHibernationTime { get; set; }
        bool DisplaySettingExplanations { get; set; }
        string Pseudonym { get; set; }
        DateTime MostRecentFirstLoginReminderDismissal { get; set; }
        DateTime MostRecentLastToLeaveReminderDismissal { get; set; }
        DateTime LastToLeaveReminderSnoozeUntilTime { get; set; }
        string CompanyId { get; set; }
        string CompanyName { get; set; }
        StringCollection WorkNetworks { get; set; }
        StringCollection OtherNetworks { get; set; }
        double MostRecentPeakTemperature { get; set; }
        string Location { get; set; }

        void Save();
    }
}
