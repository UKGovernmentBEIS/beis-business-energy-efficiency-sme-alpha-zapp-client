using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class ReminderViewModel : NotificationViewModel
    {
        public string Title { get; set;  }
        public string Message { get; set; }
    }
}
