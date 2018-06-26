using System.Linq;
using System.Windows;
using Caliburn.Micro;

namespace RemindSME.Desktop.Helpers
{
    public interface ISingletonWindowManager
    {
        void OpenOrFocusSingletonWindow<TView, TViewModel>();
    }

    public class SingletonWindowManager : ISingletonWindowManager
    {
        private readonly IWindowManager windowManager;

        public SingletonWindowManager(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void OpenOrFocusSingletonWindow<TView, TViewModel>()
        {
            var existingWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is TView);

            if (existingWindow != null)
            {
                if (existingWindow.WindowState == WindowState.Minimized)
                {
                    existingWindow.WindowState = WindowState.Normal;
                }
                existingWindow.Activate();
            }
            else
            {
                var viewModel = IoC.Get<TViewModel>();
                windowManager.ShowWindow(viewModel);
            }
        }
    }
}
