using Caliburn.Micro;
using RemindSME.Desktop.Services;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : Notification
    {
        private readonly IHibernationService hibernationService;
        private readonly ILog log;

        public HibernationPromptViewModel(
            ILog log, 
            IHibernationService hibernationService, 
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.log = log;
            this.hibernationService = hibernationService;
        }

        public void Sure()
        {
            log.Info("User clicked 'Sure!' on hibernation prompt.");
        }

        public void Snooze()
        {
            log.Info("User clicked 'Snooze' on hibernation prompt.");
            hibernationService.Snooze();
        }

        public void NotTonight()
        {
            log.Info("User clicked 'Not tonight' on hibernation prompt.");
            hibernationService.NotTonight();
        }
    }
}
