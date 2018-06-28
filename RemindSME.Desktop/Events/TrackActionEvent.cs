namespace RemindSME.Desktop.Events
{
    public class TrackActionEvent
    {
        public string Message { get; }

        public TrackActionEvent(string message)
        {
            Message = message;
        }
    }
}
