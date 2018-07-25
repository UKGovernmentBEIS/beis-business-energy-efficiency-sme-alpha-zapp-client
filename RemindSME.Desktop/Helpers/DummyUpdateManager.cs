using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32;
using NuGet;
using Squirrel;

namespace RemindSME.Desktop.Helpers
{
    /// <summary>
    ///     Replaces the instance of Squirrel.Windows.UpdateManager when running locally.
    /// </summary>
    public class DummyUpdateManager : IUpdateManager
    {
        private const string DummyReleaseEntry = @"0000000000000000000000000000000000000000 Zapp-1.0.nupkg 000000";

        public void Dispose() { }

        public Task<UpdateInfo> CheckForUpdate(bool ignoreDeltaUpdates = false, Action<int> progress = null)
        {
            var releaseEntry = ReleaseEntry.ParseReleaseEntry(DummyReleaseEntry);
            var updateInfo = UpdateInfo.Create(releaseEntry, new[] { releaseEntry }, "dummy");
            return Task.FromResult(updateInfo);
        }

        public Task DownloadReleases(IEnumerable<ReleaseEntry> releasesToDownload, Action<int> progress = null)
        {
            return Task.CompletedTask;
        }

        public Task<string> ApplyReleases(UpdateInfo updateInfo, Action<int> progress = null)
        {
            return Task.FromResult("dummy");
        }

        public Task FullInstall(bool silentInstall, Action<int> progress = null)
        {
            return Task.CompletedTask;
        }

        public Task FullUninstall()
        {
            return Task.CompletedTask;
        }

        public SemanticVersion CurrentlyInstalledVersion(string executable = null)
        {
            return SemanticVersion.Parse("0.1.0.0");
        }

        public Task<RegistryKey> CreateUninstallerRegistryEntry(string uninstallCmd, string quietSwitch)
        {
            return Task.FromResult<RegistryKey>(null);
        }

        public Task<RegistryKey> CreateUninstallerRegistryEntry()
        {
            return Task.FromResult<RegistryKey>(null);
        }

        public void RemoveUninstallerRegistryEntry() { }

        public void CreateShortcutsForExecutable(string exeName, ShortcutLocation locations, bool updateOnly, string programArguments, string icon) { }

        public void RemoveShortcutsForExecutable(string exeName, ShortcutLocation locations) { }
    }
}
