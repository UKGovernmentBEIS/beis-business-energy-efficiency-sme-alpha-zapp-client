using Caliburn.Micro;
using RemindSME.Desktop.Helpers;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : PropertyChangedBase
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationManager hibernationManager;

        public HibernationPromptViewModel(IActionTracker actionTracker, IHibernationManager hibernationManager)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
        }

        public void DoItNow()
        {
            actionTracker.Log("User clicked 'Do it now!' on hibernation prompt.");
            hibernationManager.Hibernate();
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
