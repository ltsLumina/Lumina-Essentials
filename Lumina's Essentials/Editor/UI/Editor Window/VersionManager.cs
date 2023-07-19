#region
using System;
using UnityEditor;
#endregion

namespace Lumina.Essentials.Editor
{
    public static class VersionManager
    {
        /// <summary> The current version of Lumina's Essentials. </summary>
        public static string CurrentVersion => "1.0.0 Debug";
        /// <summary> The latest version of Lumina's Essentials available on GitHub. </summary>
        public static string LatestVersion => EditorPrefs.GetString("LatestVersion", "Unknown");
        /// <summary> The version of the package that was last opened. </summary>
        public static string LastOpenVersion
        {
            get => EditorPrefs.GetString("LastOpenVersion", "Unknown"); // Default to 3.0.0; first version to use EditorPrefs
            set => EditorPrefs.SetString("LastOpenVersion", value);
        }
        /// <summary> Whether or not the current version is a debug version. </summary>
        public static bool DebugVersion => CurrentVersion.Contains("Debug") || CompareVersions(CurrentVersion, LatestVersion);

        internal static bool dontShowDebugWarningAgain;

        public static void UpdateStatistics(string version)
        { // Update EditorPrefs
            EditorPrefs.SetString
                ("CurrentVersion", CurrentVersion); // This will always be this way as I have to personally set CurrentVersion to the latest version.

            EditorPrefs.SetString("LatestVersion", version);
            EditorPrefs.SetBool("UpToDate", CompareVersions(CurrentVersion, LatestVersion));
            EditorPrefs.SetBool("DebugVersion", DebugVersion);
        }

        /// <summary>
        /// Opens the window on startup if the version has changed.
        /// This method is called automatically on startup.
        /// </summary>
        [InitializeOnLoadMethod]
        static void OpenWindowOnStartup()
        {
            var currentVersion  = CurrentVersion;
            var lastOpenVersion = EditorPrefs.GetString("LastOpenVersion", "Unknown");
            var latestVersion   = LatestVersion;

            // If an update has occured since the last time the window was opened, open the window.
            if (CompareVersions(currentVersion, lastOpenVersion))
            {
                SetupWindow.OpenSetupWindow(true);
                EditorPrefs.SetString("LastOpenVersion", currentVersion);
            }

            // If the current version is newer than the *latest* version, perform update checks (I.e; if the user is up-to-date)
            if (StartupChecks.IsNewVersionAvailable(currentVersion, latestVersion) || !EditorPrefs.GetBool("UpToDate")) //TODO: make sure editor prefs dont mess with this, cause they probably are
                StartupChecks.PerformUpdateChecks(currentVersion, latestVersion);
        }

        /// <summary>
        ///    Compares a version string to different version string.
        ///  If v1 is newer than v2, the action is performed.
        /// </summary>
        /// <param name="v1"> The first version to compare. </param>
        /// <param name="v2"> The second version to compare. </param>
        /// <param name="action"> The action to perform if the current version is newer than the last opened version. </param>
        /// <returns> Whether or not the current version is newer than the last opened version. </returns>
        public static bool CompareVersions(string v1, string v2, Action action = default)
        {
            bool versionsDifferent = string.Compare(v1, v2, StringComparison.Ordinal) > 0;

            if (versionsDifferent) action?.Invoke();

            return versionsDifferent;
        }
    }
}
