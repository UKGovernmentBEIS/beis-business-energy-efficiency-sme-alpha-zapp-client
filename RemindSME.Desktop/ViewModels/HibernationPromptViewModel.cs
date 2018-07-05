using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Services;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : Notification
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationService hibernationService;

        public HibernationPromptViewModel(IActionTracker actionTracker, IHibernationService hibernationService)
        {
            this.actionTracker = actionTracker;
            this.hibernationService = hibernationService;
        }

        public void Sure()
        {
            actionTracker.Log("User clicked 'Sure!' on hibernation prompt.");
        }

        public void Snooze()
        {
            actionTracker.Log("User clicked 'Snooze' on hibernation prompt.");
            hibernationService.Snooze();
        }

        public void NotTonight()
        {
            actionTracker.Log("User clicked 'Not tonight' on hibernation prompt.");
            hibernationService.NotTonight();
        }
    }
}
