using System.Diagnostics;
using System.Reflection;

namespace RemindSME.Desktop.Helpers
{
    public static class AppInfo
    {
        public static string Title
        {
            get
            {
                var titleAttribute = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute));
                return !string.IsNullOrEmpty(titleAttribute?.Title)
                    ? titleAttribute.Title
                    : null;
            }
        }

        public static string Version
        {
            get
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return !string.IsNullOrEmpty(versionInfo.FileVersion)
                    ? versionInfo.FileVersion
                    : null;
            }
        }

        public static string Location => Assembly.GetExecutingAssembly().Location;
    }
}
