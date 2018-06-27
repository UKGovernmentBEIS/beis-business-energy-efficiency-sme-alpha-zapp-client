using System;

namespace RemindSME.Desktop.Helpers
{
    public static class HibernationSettings
    {
        public static TimeSpan HibernationPromptPeriod = TimeSpan.FromMinutes(15);
        public static TimeSpan SnoozeTime = TimeSpan.FromHours(1);
    }
}
