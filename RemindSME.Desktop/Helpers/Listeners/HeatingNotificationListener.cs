namespace RemindSME.Desktop.Helpers.Listeners
{
    public class HeatingNotificationListener : SocketListener
    {
        private readonly IReminderManager reminderManager;

        public HeatingNotificationListener(IReminderManager reminderManager)
        {
            this.reminderManager = reminderManager;
        }

        public override void Call(params object[] args)
        {
            var title = args.GetArgument<string>(0);
            var message = args.GetArgument<string>(1);
            reminderManager.ShowHeatingNotificationIfOptedIn(title, message);
        }
    }
}
