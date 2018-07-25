using System;
using Caliburn.Micro;

namespace Zapp.Desktop.Logging
{
    public interface IActionLog : ILog
    {
        void Info(TrackedActions action, string format, params object[] args);
        void Warn(TrackedActions action, string format, params object[] args);
        void Error(TrackedActions action, Exception exception);
    }
}
