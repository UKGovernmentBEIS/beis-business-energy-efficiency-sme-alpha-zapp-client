using RemindSME.Desktop.Logging;

namespace RemindSME.Desktop.Events
{
    public class TrackingEvent
    {
        public LogLevel LogLevel { get; }
        public string Message { get; }

        public TrackingEvent(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}
