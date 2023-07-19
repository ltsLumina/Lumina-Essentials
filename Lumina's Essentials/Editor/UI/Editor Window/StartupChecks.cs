using System;
using UnityEditor;

namespace Lumina.Essentials.Editor
{
    public static class StartupChecks
    {
        internal static void PerformUpdateChecks(string currentVersion, string latestVersion)
        {
            if (IsNewVersionAvailable(currentVersion, latestVersion)) DisplayNewVersionAlert(latestVersion);

            if (!VersionManager.DebugVersion)
            {
                AlertOnMajorUpdateIfAvailable(latestVersion);
                CheckForUpdatesAfterOneWeek();
            }
            DebugBuildWarning();
        }
        
        /// <summary>
        /// Checks if a new version is available by comparing the current version with the latest version.
        /// </summary>
        /// <param name="currentVersion"> The current version of Lumina's Essentials. </param>
        /// <param name="comparisonVersion"> The version to compare the current version with. </param>
        /// <returns> True if the current version is older than the comparison version. </returns>
        internal static bool IsNewVersionAvailable(string currentVersion, string comparisonVersion) => 
            !VersionManager.CompareVersions(currentVersion, comparisonVersion);
        
        /// <summary>
        /// Checks if a major update is available by comparing the current version with the latest version.
        /// If it returns true it logs a warning to the console that a major update is available.
        /// </summary>
        /// <param name="latestVersion"> The latest version of Lumina's Essentials. </param>
        static void DisplayNewVersionAlert(string latestVersion) => 
            DebugHelper.LogWarning($"A new version of Lumina's Essentials is available! (v{latestVersion})\nYou can download it from the GitHub repository.");
        /// <summary>
        /// Checks if a major update is available by comparing the current version with the latest version.
        /// </summary>
        /// <param name="latestVersion"></param>
        static void AlertOnMajorUpdateIfAvailable(string latestVersion)
        {
            if (MajorUpdateAvailable() && !VersionManager.DebugVersion)
            {
                EditorUtility.DisplayDialog
                ("Major Update Available",
                 $"There is a new version of Lumina's Essentials available on GitHub. \n Latest Version: v{latestVersion}. \n" +
                 "Please check the changelog for more information.", "OK");
            }
        }
        
        static void CheckForUpdatesAfterOneWeek()
        {
            if (TimeSinceLastUpdateInDays() > 7)
            {
                EssentialsUpdater.CheckForUpdates();
                
                // If there is an update available, display a warning to the user.
                if (IsNewVersionAvailable(VersionManager.CurrentVersion, VersionManager.LatestVersion))
                {
                    EditorUtility.DisplayDialog
                    ("Update Available",
                     "A new version of Lumina's Essentials is available. \n" +
                     "Please check the changelog for more information.", "OK");
                }
            }
        }
        static void DebugBuildWarning()
        {
            VersionManager.dontShowDebugWarningAgain = EditorPrefs.GetBool("DontShowAgain");
            
            if (!VersionManager.dontShowDebugWarningAgain && VersionManager.DebugVersion) 
            { 
                if (EditorUtility.DisplayDialog
                    ("Debug Version", "You are currently using a Debug Version of Lumina's Essentials. " + "\nThis means the application might not work as intended.", "OK"))
                {
                    VersionManager.dontShowDebugWarningAgain = true;
                    EditorPrefs.SetBool("DontShowAgain", VersionManager.dontShowDebugWarningAgain); 
                }
            }
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

            return !VersionManager.CompareVersions(VersionManager.CurrentVersion, VersionManager.LatestVersion);
        }
    }
}
