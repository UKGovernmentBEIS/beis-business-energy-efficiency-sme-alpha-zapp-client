using Caliburn.Micro;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.Helpers.Listeners
{
    public class HeatingNotificationListener : SocketListener
    {
        public HeatingNotificationListener(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public override void Call(params object[] args)
        {
            var title = args.GetArgument<string>(0);
            var message = args.GetArgument<string>(1);
            EventAggregator.PublishOnUIThread(new HeatingNotificationEvent(title, message));
        }
    }
}
