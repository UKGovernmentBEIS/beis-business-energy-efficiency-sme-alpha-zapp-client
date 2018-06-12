﻿using System.Windows;
using Caliburn.Micro;
using RemindsSME.Desktop.ViewModels;

namespace RemindsSME.Desktop
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<TaskbarIconViewModel>();
        }
    }
}
