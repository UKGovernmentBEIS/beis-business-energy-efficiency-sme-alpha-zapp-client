using System;

namespace RemindSME.Desktop.Helpers.Listeners
{
    public class NetworkCountChangeListener : SocketListener
    {
        private readonly IReminderManager reminderManager;

        public NetworkCountChangeListener(IReminderManager reminderManager)
        {
            this.reminderManager = reminderManager;
        }

        public override void Call(params object[] args)
        {
            var count = args.GetIntegerArgument(0);
            reminderManager.HandleNetworkCountChange(count ?? 0);
        }
    }
}
