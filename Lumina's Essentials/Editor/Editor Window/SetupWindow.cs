using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.EditorGUIUtils;
using static Lumina.Essentials.Editor.VersionManager;

namespace Lumina.Essentials.Editor
{
    /// <summary>
    /// A window that launches upon project launch to help the user get started with Lumina's Essentials.
    /// Provides a list of all the features and a button to open the documentation.
    /// It also provides a button to install the latest version of Lumina's Essentials as well as DOTween (in a future update).
    /// </summary>
    public sealed class SetupWindow : EditorWindow
    {
        [MenuItem("Lumina's Essentials/Setup", false, 0)]
        static void Debug_OpenSetupWindow() => OpenSetupWindow();
        
        /// <summary>
        ///     Opens the setup window that instructs the user on how to get started with Lumina's Essentials.
        /// </summary>
        /// <param name="updateOccured">Whether or not an update has occured since the last time the window was opened.</param>
        public static void OpenSetupWindow(bool updateOccured = false)
        {
            var window = GetWindow<SetupWindow>(true, "New Version of Lumina's Essentials Imported", true);
            window.minSize = new (400, 300);
            window.maxSize = window.minSize;
            window.Show();

            if (!updateOccured) return;
            
            Debug.Log("Update Occured");

            // Close the utility window if it is open.
            var utilityWindow = GetWindow<UtilityWindow>();
            if (utilityWindow != null) utilityWindow.Close();
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
                OpenSetupWindow(true);
                EditorPrefs.SetString("LastOpenVersion", currentVersion);
            }

            // If the current version is newer than the *latest* version, perform update checks (I.e; if the user is up-to-date)
            if (IsNewVersionAvailable(currentVersion, latestVersion) || !EditorPrefs.GetBool("UpToDate")) //TODO: make sure editor prefs dont mess with this, cause they probably are
                PerformUpdateChecks(currentVersion, latestVersion);
        }

        static void PerformUpdateChecks(string currentVersion, string latestVersion)
        {
            if (IsNewVersionAvailable(currentVersion, latestVersion)) DisplayNewVersionAlert(latestVersion);

            AlertOnMajorUpdateIfAvailable(latestVersion);
            DisplayDialoguesInDebugVersion(currentVersion, latestVersion);
            CheckForUpdatesAfterOneWeek();
        }

        /// <summary>
        /// Checks if a new version is available by comparing the current version with the latest version.
        /// </summary>
        /// <param name="currentVersion"> The current version of Lumina's Essentials. </param>
        /// <param name="comparisonVersion"> The version to compare the current version with. </param>
        /// <returns> True if the current version is older than the comparison version. </returns>
        static bool IsNewVersionAvailable(string currentVersion, string comparisonVersion) => 
            !CompareVersions(currentVersion, comparisonVersion);


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
            if (MajorUpdateAvailable())
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
                if (IsNewVersionAvailable(CurrentVersion, LatestVersion))
                {
                    EditorUtility.DisplayDialog
                    ("Update Available",
                     "A new version of Lumina's Essentials is available. \n" +
                     "Please check the changelog for more information.", "OK");
                }
            }
        }

        static void DisplayDialoguesInDebugVersion(string currentVersion, string latestVersion)
        {
            if (CompareVersions(currentVersion, latestVersion))
            {
                EditorUtility.DisplayDialog
                ("Debug Version", "You are currently using a Debug Version of Lumina's Essentials. " +
                "\nThis means the application might behave differently or not work as intended.", "OK");
            }
        }

        void OnGUI()
        {
            // // Top label with the title of the window in large rose gold text
            // GUILayout.Label
            // ("Lumina's Essentials", new GUIStyle
            //  { fontSize  = 25,
            //    fontStyle = FontStyle.Bold,
            //    normal = new GUIStyleState
            //    { textColor = new Color(1f, 0.64f, 0.54f) } });
            //
            // GUILayout.Label
            //     ("Select <color=orange>\"Setup Lumina Essentials\"</color> in the Utility Panel to set it up \n and ensure functionality", defaultStyle);
            //
            // GUILayout.Space(10);
            //
            // // Large header in case of upgrade
            // GUILayout.Label
            // ("IMPORTANT IN CASE OF UPGRADE", new GUIStyle
            //  { fontSize  = 20,
            //    fontStyle = FontStyle.Bold,
            //    normal = new GUIStyleState
            //    { textColor = new Color(1f, 0.64f, 0.54f) } });
            //
            // // Text that asks the user to press the button below in regular text with the "Setup Lumina Essentials" text in orange
            // GUILayout.Label("If you are upgrading from a Lumina Essentials version older than <b>3.0.0</b> \n (<color=orange>before the rework of the VersionManager system</color>) " +
            //                 "\n You will see a lot of errors. This is due to several reworks since then. \n <b>Follow these instructions</b> to fix them: ", defaultStyle);
            //
            // GUILayout.Label("<color=orange>1) Delete the old version </color>of the Lumina Essentials package.", defaultStyle);
            // GUILayout.Label("<color=orange>2)</color> Open the Utility Panel and press <color=orange>\"Setup Essentials\"</color> to set it up.", defaultStyle);
            //
            //
            // // Button to open the Utility Panel
            // const int buttonWidth  = 200;
            // const int buttonHeight = 35;
            // GUILayout.BeginArea(new (Screen.width / 2f - buttonWidth / 2f, Screen.height - buttonHeight - 30, buttonWidth, buttonHeight));
            //
            // if (GUILayout.Button("Open Utility Panel", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            // {
            //     // Open the Utility Panel and close the Setup Window
            //     UtilityWindow.OpenUtilityWindow();
            //     Close();
            // }
            //
            //GUILayout.EndArea();
            
            DrawBackground();
            
            DrawTitleAndInstructions();
            
            DrawUtilityPanelButton();
        }
        
        void DrawBackground()
        {
            EditorGUI.DrawRect(new (0, 0, maxSize.x, maxSize.y), new (0.18f, 0.18f, 0.18f));
        }

        void DrawTitleAndInstructions()
        {
            DrawHeader("Lumina's Essentials");
            DrawBody("Select <color=orange>\"Setup Lumina Essentials\"</color> in the Utility Panel to set it up \n and ensure functionality");

            DrawHeader("IMPORTANT IN CASE OF UPGRADE");

            DrawBody("If you are upgrading from a Lumina Essentials version older than <b>3.0.0</b> \n (<color=orange>before the rework of " +
                     "the VersionManager system</color>) \n You will see a lot of errors. This is due to several reworks since then. \n " +
                     "<b>Follow these instructions</b> to fix them: ");

            DrawBody("<color=orange>1) Delete the old version </color>of the Lumina Essentials package.");
            DrawBody("<color=orange>2)</color> Open the Utility Panel and press <color=orange>\"Setup Essentials\"</color> to set it up.");
        }

        void DrawUtilityPanelButton()
        {
            GUILayout.BeginArea(new (Screen.width / 2f - 100, Screen.height - 35 - 30, 200, 35));
            if (GUILayout.Button("Open Utility Panel", GUILayout.Width(200), GUILayout.Height(35)))
            {
                UtilityWindow.OpenUtilityWindow();
                Close();
            }
            GUILayout.EndArea();
        }

        void DrawHeader(string text)
        {
            GUILayout.Label(text, setupWindowStyle);
        }

        void DrawBody(string text)
        {
            GUILayout.Space(10);
            GUILayout.Label(text, defaultStyle);
        }
    }
}
