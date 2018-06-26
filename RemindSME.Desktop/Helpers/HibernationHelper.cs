using System;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop
{
    public class HibernationHelper
    {
        public static void HandleHibernationOnTick()
        {
            // If yesterday or before, set the  next hibernation to today at the default time
            if (Settings.Default.NextHibernationTime.Date <= DateTime.Today.AddDays(-1))
            {
                Settings.Default.NextHibernationTime = DateTime.Today.Add(Settings.Default.DefaultHibernationTime);
            }

            // If the next hibernation time is today, but in the past or within the next 15 minutes, then bump it along to the default time tomorrow
            if (Settings.Default.NextHibernationTime.Date == DateTime.Today)
            {
                if (Settings.Default.NextHibernationTime.Subtract(DateTime.Now) <= TimeSpan.FromMinutes(15))
                {
                    Settings.Default.NextHibernationTime = DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
                }
                Settings.Default.Save();
            }
                
        }

        public static void HandleHibernationOnChange(TimeSpan newDefaultHibernationTime)
        {
            // If the new default time is set to later today and at least 15 minutes in the future, then set it for today. Otherwise set it for tomorrow.
            Settings.Default.NextHibernationTime = newDefaultHibernationTime.Subtract(TimeSpan.FromMinutes(15)) > DateTime.Today.TimeOfDay 
                ? DateTime.Today.Add(newDefaultHibernationTime) 
                : DateTime.Today.AddDays(1).Add(newDefaultHibernationTime);
            Settings.Default.Save();
        }

        public static void Snooze(TimeSpan timespan)
        {
            Settings.Default.NextHibernationTime = Settings.Default.NextHibernationTime.Add(timespan);
       

        }
    }
}
