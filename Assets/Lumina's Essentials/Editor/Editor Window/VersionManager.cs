#region
using System;
using UnityEditor;
#endregion

namespace Lumina.Essentials.Editor
{
    public static class VersionManager
    {
        /// <summary> The current version of Lumina's Essentials. </summary>
        public static string CurrentVersion => "1.0.0"; // Default to 3.0.0 as that is the first version to use EditorPrefs
        /// <summary> The latest version of Lumina's Essentials available on GitHub. </summary>
        public static string LatestVersion => EditorPrefs.GetString("LatestVersion", "3.0.0"); // Default to 3.0.0; first version to use EditorPrefs
        /// <summary> The version of the package that was last opened. </summary>
        public static string LastOpenVersion
        {
            get => EditorPrefs.GetString("LastOpenVersion", "Unknown"); // Default to 3.0.0; first version to use EditorPrefs
            set => EditorPrefs.SetString("LastOpenVersion", value);
        }
        /// <summary> Whether or not the current version is a debug version. </summary>
        public static bool DebugVersion => CurrentVersion.Contains("Debug") || CompareVersions(CurrentVersion, LatestVersion);

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

        public static void UpdateStatistics(string version)
        { // Update EditorPrefs
            EditorPrefs.SetString
                ("CurrentVersion", CurrentVersion); // This will always be this way as I have to personally set CurrentVersion to the latest version.

            EditorPrefs.SetString("LatestVersion", version);
            EditorPrefs.SetBool("UpToDate", CompareVersions(CurrentVersion, LatestVersion));
            EditorPrefs.SetBool("DebugVersion", DebugVersion);
        }

        public static string TimeSinceLastUpdate()
        {
            string lastUpdateCheckString = EditorPrefs.GetString("LastUpdateCheck");
            bool   isParsedSuccessfully  = DateTime.TryParse(lastUpdateCheckString, out DateTime lastUpdateCheck);

            if (!isParsedSuccessfully) return "Failed to parse stored date time string.";

            TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;

            // for less than 1 hour, display minutes
            if (timeSpanSinceLastUpdate < TimeSpan.FromHours(1))
                return $"{timeSpanSinceLastUpdate.Minutes} minute{(timeSpanSinceLastUpdate.Minutes > 1 ? "s" : "")} ago";

            // for less than 2 hours, display "hour"
            if (timeSpanSinceLastUpdate < TimeSpan.FromHours(2)) return "1 hour ago";

            // for less than 24 hours, display hours
            if (timeSpanSinceLastUpdate < TimeSpan.FromDays(1)) return $"{timeSpanSinceLastUpdate.Hours} hours ago";

            // for less than 2 days, display "day"
            if (timeSpanSinceLastUpdate < TimeSpan.FromDays(2)) return "1 day ago";

            // for less than a week, display days
            if (timeSpanSinceLastUpdate < TimeSpan.FromDays(7)) return $"{timeSpanSinceLastUpdate.Days} days ago";

            // for more than a week
            return "More than a week ago.";
        }

        public static int TimeSinceLastUpdateInDays()
        {
            if (DateTime.TryParse(EssentialsUpdater.LastUpdateCheck, out DateTime lastUpdateCheck))
            {
                TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;
                return (int) timeSpanSinceLastUpdate.TotalDays;
            }

            // Handle parse failure - depends on your requirements
            return -1; // Example: return -1 if the date could not be parsed
        }

        public static bool MajorUpdateAvailable()
        {
            EssentialsUpdater.CheckForUpdates();

            return !CompareVersions(CurrentVersion, LatestVersion);
        }
    }
}
