using Zapp.Desktop.Logging;

namespace Zapp.Desktop.Events
{
    public class TrackingEvent
    {
        public TrackingEvent(TrackedActions? trackedAction, LogLevel logLevel, string message)
        {
            TrackedAction = trackedAction;
            LogLevel = logLevel;
            Message = message;
        }

        public TrackedActions? TrackedAction { get; }
        public LogLevel LogLevel { get; }
        public string Message { get; }
    }
}
