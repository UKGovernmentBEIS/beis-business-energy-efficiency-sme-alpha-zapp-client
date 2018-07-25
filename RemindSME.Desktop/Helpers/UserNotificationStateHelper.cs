using System.Runtime.InteropServices;

namespace RemindSME.Desktop.Helpers
{
    public interface IUserNotificationStateHelper
    {
        bool AcceptingNotifications();
    }

    // See: https://docs.microsoft.com/en-us/windows/desktop/api/shellapi/nf-shellapi-shqueryusernotificationstate
    // And: https://www.pinvoke.net/default.aspx/shell32.shqueryusernotificationstate
    public class UserNotificationStateHelper : IUserNotificationStateHelper
    {
        public bool AcceptingNotifications()
        {
            SHQueryUserNotificationState(out var state);
            return state == UserNotificationState.AcceptsNotifications;
        }

        [DllImport("shell32.dll")]
        private static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);
    }

    public enum UserNotificationState
    {
        /// <summary>
        ///     A screen saver is displayed, the machine is locked,
        ///     or a nonactive Fast User Switching session is in progress.
        /// </summary>
        NotPresent = 1,

        /// <summary>
        ///     A full-screen application is running or Presentation Settings are applied.
        ///     Presentation Settings allow a user to put their machine into a state fit
        ///     for an uninterrupted presentation, such as a set of PowerPoint slides, with a single click.
        /// </summary>
        Busy = 2,

        /// <summary>
        ///     A full-screen (exclusive mode) Direct3D application is running.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // Must match external name.
        RunningDirect3dFullScreen = 3,

        /// <summary>
        ///     The user has activated Windows presentation settings to block notifications and pop-up messages.
        /// </summary>
        PresentationMode = 4,

        /// <summary>
        ///     None of the other states are found, notifications can be freely sent.
        /// </summary>
        AcceptsNotifications = 5,

        /// <summary>
        ///     Introduced in Windows 7. The current user is in "quiet time", which is the first hour after
        ///     a new user logs into his or her account for the first time. During this time, most notifications
        ///     should not be sent or shown. This lets a user become accustomed to a new computer system
        ///     without those distractions.
        ///     Quiet time also occurs for each user after an operating system upgrade or clean installation.
        /// </summary>
        QuietTime = 6
    }
}
