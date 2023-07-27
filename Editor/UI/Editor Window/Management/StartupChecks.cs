using System;
using UnityEditor;

namespace Lumina.Essentials.Editor.UI.Management
{
    public static class StartupChecks
    {
        internal static void DisplayVersionAlert()
        {
            if (!VersionManager.DebugVersion)
            {
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
        
        internal static void DebugBuildWarning()
        {
            if (!VersionManager.DontShow_DebugBuildWarning && VersionManager.DebugVersion) 
            { 
                if (EditorUtility.DisplayDialog
                    ("Debug Version", "You are currently using a Debug Version of Lumina's Essentials. " + "\nThis means the application might not work as intended.", "OK"))
                {
                    VersionManager.DontShow_DebugBuildWarning = true;
                    EditorPrefs.SetBool("DontShowAgain", VersionManager.DontShow_DebugBuildWarning); 
                }
            }
        }
        
        public static string TimeSinceLastUpdate()
        {
            string lastUpdateCheckString = EditorPrefs.GetString("LastUpdateCheck");
            bool   isParsedSuccessfully  = DateTime.TryParse(lastUpdateCheckString, out DateTime lastUpdateCheck);

            if (!isParsedSuccessfully) return "Failed to parse stored date time string.";

            TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;

            (TimeSpan timeSpan, string singularMessage, string pluralMessage)[] timeFrames ={ 
                (TimeSpan.FromHours(1), "{0} minute ago", "{0} minutes ago"),
                (TimeSpan.FromHours(2), "{0} hour ago", "{0} hour ago"),
                (TimeSpan.FromDays(1), "{0} hours ago", "{0} hours ago"),
                (TimeSpan.FromDays(2), "{0} day ago", "{0} day ago"),
                (TimeSpan.FromDays(7), "{0} days ago", "{0} days ago"),
                (TimeSpan.FromDays(30), "{0} weeks ago", "{0} weeks ago"),
                (TimeSpan.MaxValue, "<color=red>More than a week ago</color>",
                "<color=red>More than a week ago</color>") };

            foreach ((TimeSpan timeSpan, string singularMessage, string pluralMessage) in timeFrames)
            {
                if (timeSpanSinceLastUpdate < timeSpan) return FormatTimeMessage(timeSpanSinceLastUpdate, singularMessage, pluralMessage);
            }

            return string.Empty; // unreachable code but added to satisfy C# rules
        }

        static string FormatTimeMessage(TimeSpan deltaTime, string singularMessage, string pluralMessage)
        {
            if (deltaTime.TotalMinutes < 1) { return "Less than a minute ago"; }
            if (deltaTime.TotalMinutes < 2) return string.Format(singularMessage, 1);
            if (deltaTime.TotalHours   < 2) return string.Format(pluralMessage, (int) deltaTime.TotalMinutes);
            if (deltaTime.TotalDays    < 2) return string.Format(pluralMessage, (int) deltaTime.TotalHours);
            if (deltaTime.TotalDays    >= 2) return string.Format(pluralMessage, (int) deltaTime.TotalDays);

            return "";
        }

        static int TimeSinceLastUpdateInDays()
        {
            if (DateTime.TryParse(EssentialsUpdater.LastUpdateCheck, out DateTime lastUpdateCheck))
            {
                TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;
                return (int) timeSpanSinceLastUpdate.TotalDays;
            }

            // Handle parse failure - depends on your requirements
            return -1; // Example: return -1 if the date could not be parsed
        }
    }
}
