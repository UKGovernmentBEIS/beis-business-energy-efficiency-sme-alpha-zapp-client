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

            // If the next hibernation time is today, but in the past, then bump it along to the default time tomorrow
            if (Settings.Default.NextHibernationTime.Date == DateTime.Today &&
                Settings.Default.NextHibernationTime <= DateTime.Now)
            {
                Settings.Default.NextHibernationTime = DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
            }
        }

        public static void HandleHibernationOnChange(TimeSpan newDefaultHibernationTime)
        {
            Settings.Default.NextHibernationTime = newDefaultHibernationTime > DateTime.Today.TimeOfDay 
                ? DateTime.Today.Add(newDefaultHibernationTime) 
                : DateTime.Today.AddDays(1).Add(newDefaultHibernationTime);
        }

        public static void Snooze(TimeSpan timespan)
        {
            Settings.Default.NextHibernationTime = Settings.Default.NextHibernationTime.Add(timespan);
        }
    }
}
