using RemindSME.Desktop.Logging;

namespace RemindSME.Desktop.Events
{
    public class TrackingEvent
    {
        public TrackedActions? TrackedAction { get; }
        public LogLevel LogLevel { get; }
        public string Message { get; }

        public TrackingEvent(TrackedActions? trackedAction, LogLevel logLevel, string message)
        {
            TrackedAction = trackedAction;
            LogLevel = logLevel;
            Message = message;
        }
    }
}
