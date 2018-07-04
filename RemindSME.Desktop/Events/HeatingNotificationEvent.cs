namespace RemindSME.Desktop.Events
{
    public class HeatingNotificationEvent
    {
        public string Title { get; }
        public string Message { get; }

        public HeatingNotificationEvent(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
