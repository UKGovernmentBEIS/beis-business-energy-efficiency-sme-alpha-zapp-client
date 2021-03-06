﻿using System.Diagnostics;
using System.Reflection;

namespace Zapp.Desktop.Helpers
{
    public static class AppInfo
    {
        static AppInfo()
        {
            var titleAttribute = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute));
            Title = !string.IsNullOrEmpty(titleAttribute?.Title) ? titleAttribute.Title : null;

            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Version = !string.IsNullOrEmpty(versionInfo.FileVersion) ? versionInfo.FileVersion : null;

            Location = Assembly.GetExecutingAssembly().Location;
        }

        public static string Title { get; }
        public static string Version { get; }
        public static string Location { get; }
    }
}
