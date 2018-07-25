using Caliburn.Micro;
using Zapp.Desktop.Events;

namespace Zapp.Desktop.Helpers.Listeners
{
    public class CompanyCountChangeListener : SocketListener
    {
        public CompanyCountChangeListener(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public override void Call(params object[] args)
        {
            var count = args.GetIntegerArgument(0);
            EventAggregator.PublishOnUIThread(new NetworkCountChangeEvent(count ?? 0));
        }
    }
}
