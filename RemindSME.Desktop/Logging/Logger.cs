using System;
using System.IO;
using System.Reflection;
using Caliburn.Micro;
using NLog;
using NLog.Config;
using NLog.Targets;
using RemindSME.Desktop.Events;
using LogManager = NLog.LogManager;

namespace RemindSME.Desktop.Logging
{
    public class Logger : IActionLog
    {
        private static readonly string LogFilePath = Path.Combine(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName,
            "Zapp.log");

        private readonly IEventAggregator eventAggregator;
        private readonly ILogger fileLogger;

        public Logger(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            ConfigureLogManager();
            fileLogger = LogManager.GetLogger("Logger");
        }

        private void ConfigureLogManager()
        {
            var config = new LoggingConfiguration();
            var logFile = new FileTarget { FileName = LogFilePath };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logFile);
            LogManager.Configuration = config;
        }

        public void Info(string format, params object[] args)
        {
            InfoInternal(null, format, args);
        }

        public void Info(TrackedActions action, string format, params object[] args)
        {
            InfoInternal(action, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            WarnInternal(null, format, args);
        }

        public void Warn(TrackedActions action, string format, params object[] args)
        {
            WarnInternal(action, format, args);
        }

        public void Error(Exception exception)
        {
            ErrorInternal(null, exception);
        }

        public void Error(TrackedActions action, Exception exception)
        {
            ErrorInternal(action, exception);
        }

        private void InfoInternal(TrackedActions? action, string format, params object[] args)
        {
            Log(action, LogLevel.Info, format, args);
        }

        private void WarnInternal(TrackedActions? action, string format, params object[] args)
        {
            Log(action, LogLevel.Warn, format, args);
        }

        private void ErrorInternal(TrackedActions? action, Exception exception)
        {
            Log(action, LogLevel.Error, exception.Message);
            fileLogger.Error(exception);
        }

        private void Log(TrackedActions? action, LogLevel logLevel, string format, params object[] args)
        {
            var message = string.Format(format, args);
            eventAggregator.PublishOnUIThread(new TrackingEvent(action, logLevel, message));
        }
    }
}
