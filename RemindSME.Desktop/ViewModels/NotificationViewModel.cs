using Caliburn.Micro;

namespace RemindSME.Desktop.ViewModels
{
    public class NotificationViewModel : PropertyChangedBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}