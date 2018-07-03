using System.Linq;
using System.Windows;
using Caliburn.Micro;

namespace RemindSME.Desktop.Helpers
{
    public interface IAppWindowManager
    {
        void OpenOrActivateWindow<TView, TViewModel>();
        void OpenOrActivateDialog<TView, TViewModel>();
    }

    public class AppWindowManager : IAppWindowManager
    {
        private readonly IWindowManager windowManager;

        public AppWindowManager(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void OpenOrActivateWindow<TView, TViewModel>()
        {
            if (!ActivateExistingWindow<TView>())
            {
                var viewModel = IoC.Get<TViewModel>();
                windowManager.ShowWindow(viewModel);
                ActivateExistingWindow<TView>();
            }
        }

        public void OpenOrActivateDialog<TView, TViewModel>()
        {
            if (!ActivateExistingWindow<TView>())
            {
                var viewModel = IoC.Get<TViewModel>();
                windowManager.ShowDialog(viewModel);
                ActivateExistingWindow<TView>();
            }
        }

        private bool ActivateExistingWindow<TView>()
        {
            var existingWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is TView);
            if (existingWindow == null)
            {
                return false;
            }

            if (existingWindow.WindowState == WindowState.Minimized)
            {
                existingWindow.WindowState = WindowState.Normal;
            }
            existingWindow.Activate();
            return true;
        }
    }
}
