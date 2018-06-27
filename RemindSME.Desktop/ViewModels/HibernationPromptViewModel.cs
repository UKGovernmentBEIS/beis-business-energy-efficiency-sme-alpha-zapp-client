using System;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : PropertyChangedBase
    {
        private readonly IHibernationManager hibernationManager;

        public HibernationPromptViewModel(IHibernationManager hibernationManager)
        {
            this.hibernationManager = hibernationManager;
        }

        public void DoItNow()
        {
            hibernationManager.Hibernate();
        }

        public void Snooze()
        {
            hibernationManager.Snooze();
        }

        public void NotTonight()
        {
            hibernationManager.NotTonight();
        }
    }
}
