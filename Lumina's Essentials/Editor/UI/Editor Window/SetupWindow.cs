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
            //     ("Select <color=orange>\"Setup Lumina Essentials\"</color> in the Utility Panel to set it up \n and ensure functionality", middleStyle);
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
            //                 "\n You will see a lot of errors. This is due to several reworks since then. \n <b>Follow these instructions</b> to fix them: ", middleStyle);
            //
            // GUILayout.Label("<color=orange>1) Delete the old version </color>of the Lumina Essentials package.", middleStyle);
            // GUILayout.Label("<color=orange>2)</color> Open the Utility Panel and press <color=orange>\"Setup Essentials\"</color> to set it up.", middleStyle);
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
            GUILayout.Label(text, middleStyle);
        }
    }
}
