using System;
using Caliburn.Micro;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.Logging
{
    public class Logger : ILog
    {
        private readonly IEventAggregator eventAggregator;

        public Logger(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        public void Error(Exception exception)
        {
            Log(LogLevel.Error, exception.Message);
        }

        private void Log(LogLevel logLevel, string format, params object[] args)
        {
            var message = string.Format(format, args);
            eventAggregator.PublishOnUIThread(new TrackingEvent(logLevel, message));
        }
    }
}
