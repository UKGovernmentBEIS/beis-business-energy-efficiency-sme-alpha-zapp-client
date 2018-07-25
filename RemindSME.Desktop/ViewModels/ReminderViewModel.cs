using System.Collections.Generic;
using Caliburn.Micro;
using RemindSME.Desktop.Models;
using Action = System.Action;

namespace RemindSME.Desktop.ViewModels
{
    public class ReminderViewModel : Notification
    {
        public ReminderViewModel(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public string Icon { get; set; } = NotificationIcon.Lightbulb;
        public string Title { get; set; }
        public string Message { get; set; }

        public IEnumerable<Button> Buttons { get; set; }

        public class Button
        {
            private readonly Action onClick;

            public Button(string label, Action onClick = null)
            {
                this.onClick = onClick;
                Label = label;
            }

            public string Label { get; }

            // Must invoke the Action via method call to allow model binding.
            public void OnClick()
            {
                onClick?.Invoke();
            }
        }
    }
}
