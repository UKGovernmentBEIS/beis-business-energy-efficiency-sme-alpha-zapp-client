using RemindSME.Desktop.Helpers;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : NotificationViewModel
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationManager hibernationManager;

        public HibernationPromptViewModel(IActionTracker actionTracker, IHibernationManager hibernationManager)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
        }

        public void Sure()
        {
            actionTracker.Log("User clicked 'Sure!' on hibernation prompt.");
        }

        public void Snooze()
        {
            actionTracker.Log("User clicked 'Snooze' on hibernation prompt.");
            hibernationManager.Snooze();
        }

        public void NotTonight()
        {
            actionTracker.Log("User clicked 'Not tonight' on hibernation prompt.");
            hibernationManager.NotTonight();
        }
    }
}
