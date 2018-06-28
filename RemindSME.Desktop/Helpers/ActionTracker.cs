using System;
using Caliburn.Micro;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.Helpers
{
    public interface IActionTracker
    {
        void Log(string message);
    }

    public class ActionTracker : IActionTracker
    {
        private readonly IEventAggregator eventAggregator;

        public ActionTracker(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Log(string message)
        {
            eventAggregator.PublishOnBackgroundThread(new TrackActionEvent(message));
        }
    }
}
