using Caliburn.Micro;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.Helpers.Listeners
{
    public class NetworkCountChangeListener : SocketListener
    {
        public NetworkCountChangeListener(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public override void Call(params object[] args)
        {
            var count = args.GetIntegerArgument(0);
            EventAggregator.PublishOnUIThread(new NetworkCountChangeEvent(count ?? 0));
        }
    }
}
