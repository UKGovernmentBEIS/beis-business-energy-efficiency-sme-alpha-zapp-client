using System;

namespace RemindSME.Desktop.Configuration
{
    public interface ISettings
    {
        TimeSpan DefaultHibernationTime { get; set; }
        bool DisplaySettingExplanations { get; set; }
        bool HeatingOptIn { get; set; }
        bool HibernationOptIn { get; set; }
        DateTime NextHibernationTime { get; set; }
        string Pseudonym { get; set; }
        DateTime MostRecentFirstLoginReminderDismissal { get; set; }
        DateTime MostRecentLastToLeaveReminderDismissal { get; set; }
        DateTime LastToLeaveReminderSnoozeUntilTime { get; set; }
        double MostRecentPeakTemperature { get; set; }
    }
}
