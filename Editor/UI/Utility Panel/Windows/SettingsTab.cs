#region
using System.Linq;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
#endregion

namespace Lumina.Essentials.Editor.UI
{
internal sealed partial class UtilityPanel
{
    #region Settings variables
    // The variables to be shown under the settings tab.

    /// <summary> Shows the more advanced settings. </summary>
    bool advancedSettings;

    /// <summary> The position of the scroll view. </summary>
    Vector2 scrollPos;

    /// <summary> Whether or not the user has to set up Lumina's Essentials to the latest version. </summary>
    internal static bool SetupRequired
    {
        get => EditorPrefs.GetBool("SetupRequired", true);
        set
        {
            value = !InstalledModules.Values.Any(module => module);
            EditorPrefs.SetBool("SetupRequired", value);
        }
    }

    /// <summary> Enabled by default. Prevents the user from accidentally doing something they didn't intend to do. </summary>
    bool SafeMode
    {
        get => EditorPrefs.GetBool("SafeMode", true);
        set => EditorPrefs.SetBool("SafeMode", value);
    }

    // End of preferences variables //
    #endregion

    /// <summary>
    ///     Draws the header of the settings panel.
    /// </summary>
    void DrawSettingsHeader()
    {
        const float spacer = 5.5f; // Has to be 5.5 to ensure that the text is at the same height as the Utilities panel.
        GUILayout.Space(spacer);

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("", GUILayout.Width(20)); // Pushes the label to the right to center it.
            GUILayout.Label("Settings", centerLabelStyle);
            GUILayout.FlexibleSpace();

            // Checkbox to show the advanced settings
            var advancedSettingsContent = new GUIContent("", "Shows the advanced settings.");
            advancedSettings = GUILayout.Toggle(advancedSettings, advancedSettingsContent);
        }

        GUILayout.Label("Various settings to customize your experience.", subLabelStyle);

        // Draw a horizontal line (separator)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    ///     Draws the Reset button onto the Utility Window.
    /// </summary>
    void DrawResetSettingsButton()
    {
        GUILayout.Space(6);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(resetButtonContent, GUILayout.Width(100), GUILayout.Height(35), GUILayout.ExpandWidth(true)))
            if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to reset the Utility Window?", "Yes", "No"))
            {
                if (!SafeMode)
                {
                    // Reset the EditorPrefs
                    foreach (var pref in VersionManager.EssentialsPrefs)
                    {
                        if (!EditorPrefs.HasKey(pref))
                        {
                            if (VersionManager.DebugVersion) EssentialsDebugger.LogWarning("Couldn't find Key: " + pref);
                        }
                        else { EditorPrefs.DeleteKey(pref); }
                    }
                    
                    

                    // Reset all Utility Panel variables.
                    SafeMode                                  = true;
                    SetupRequired                             = true;
                    VersionManager.DontShow_DebugBuildWarning = false;
                    EssentialsDebugger.LogBehaviour           = EssentialsDebugger.LogLevel.Verbose;
                    imageConverterPath                        = "";

                    EssentialsDebugger.LogWarning("Settings and EditorPrefs have been reset.");

                    // Display a warning if the user is in a debug build
                    StartupChecks.DebugBuildWarning();

                    // Check for updates to set up everything again.
                    VersionUpdater.CheckForUpdates();

                    Close();
                    UpgradeWindow.Open();
                }
                else { EssentialsDebugger.LogAbort(SafeMode); }
            }
            else { EssentialsDebugger.LogAbort(); }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    ///     Draws all the extra advanced settings to the Utility Window.
    /// </summary>
    void DrawAdvancedSettingsGUI()
    { // Displays all the advanced settings.
        if (advancedSettings)
        {
            GUILayout.Label("Debug Options", centerLabelStyle);
            GUILayout.Label("Various buttons and settings for debugging.", subLabelStyle);
            GUILayout.Space(5);

            if (GUILayout.Button("Show All EditorPrefs", GUILayout.Height(30))) ShowAllEditorPrefs();

            #region Deprecated
            if (GUILayout.Button("Update Module Packages", GUILayout.Height(30)) && advancedSettings && !SafeMode)
                EssentialsDebugger.LogWarning("This feature is deprecated and will be removed in a future update. \n It was once used to update the modules individually.");

            //UpdateModulePackages(); // Kept for reference.
            #endregion

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Display the image configuration options
            GUILayout.Label("Read-only values", centerLabelStyle);
            GUILayout.Label("These are only here for debugging purposes.", subLabelStyle);
            GUILayout.Space(5);

            // Group of view only fields
            EditorGUI.BeginDisabledGroup(true);

            // Box view the SetupRequired bool
            SetupRequired                             = EditorGUILayout.Toggle("Setup Required", SetupRequired);
            VersionManager.DontShow_DebugBuildWarning = EditorGUILayout.Toggle("Don't Show Debug Alert", VersionManager.DontShow_DebugBuildWarning);

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Space(3);

            GUILayout.Label("Installed Modules", centerLabelStyle);
            GUILayout.Label("These are the modules that are installed.", subLabelStyle);

            GUILayout.Space(5);

            // Display the installed modules from the dictionary
            foreach (var module in InstalledModules)
            {
                EditorGUILayout.Toggle(module.Key, module.Value);

                if (module.Key == "Full Package")
                    using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.LabelField("└ Extras"); }
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }

    /// <summary>
    ///     Draws the settings labels that display the current settings.
    /// </summary>
    void DrawSettingsLabels()
    {
        GUILayout.Space(10);

        // Display the value of SafeMode as a readonly checkbox and tooltip
        SafeMode = EditorGUILayout.Toggle(safeModeWarningContent, SafeMode);

        // The user can choose the logging level. This is used to determine what is logged to the console.
        if (!SafeMode && advancedSettings)
            EssentialsDebugger.LogBehaviour = (EssentialsDebugger.LogLevel) EditorGUILayout.EnumPopup
                (new GUIContent("└  Logging Behaviour", "Determines what (how much) is logged to the console."), EssentialsDebugger.LogBehaviour);

        GUILayout.Space(3);

        // Displays the last version of the Essentials package that was opened
        EditorGUILayout.LabelField("Last Open Version", VersionManager.LastOpenVersion);

        // Displays whether the user is up to date or not
        EditorGUILayout.LabelField("Up To Date", EditorPrefs.GetBool("UpToDate").ToString());

        // Displays if this is a debug build or not
        EditorGUILayout.LabelField("Debug Version", VersionManager.DebugVersion.ToString());

        // Draw a horizontal line (separator)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
}
