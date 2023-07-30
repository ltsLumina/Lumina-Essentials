#region
using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
#endregion

namespace Lumina.Essentials.Editor.UI
{
internal sealed partial class UtilityWindow
{
    #region Modules
    /// <summary> Toggle to enable or disable the installation of Alex' Essentials. (Half the package) </summary>
    static bool attributes;
    /// <summary> Toggle to enable or disable the installation of the Sequencer module of Lumina's Essentials. </summary>
    static bool sequencer;
    /// <summary> Toggle to enable or disable the installation of the Helpers module of Lumina's Essentials. </summary>
    static bool helpers;
    /// <summary> Toggle to enable or disable the installation of the Editor module of Lumina's Essentials. </summary>
    static bool shortcuts;
    /// <summary> Toggle to enable or disable the installation of the Misc module of Lumina's Essentials. </summary>
    static bool misc;

    // List of all modules.
    internal readonly static List<string> AvailableModules = new ()
    { "Full Package",
      "Sequencer",
      "Attributes",
      "Helpers",
      "Shortcuts",
      "Misc" };

    // TODO: Add option to install DOTween from here.

    internal readonly static Dictionary<string, bool> SelectedModules = AvailableModules.ToDictionary(moduleName => moduleName, _ => false);

    internal readonly static Dictionary<string, bool> InstalledModules = AvailableModules.ToDictionary(moduleName => moduleName, _ => false);
    #endregion

    /// <summary>
    ///     Draws the header for the modules panel.
    /// </summary>
    static void DrawModulesHeader()
    {
        GUILayout.Space(5);
        GUILayout.Label("   Add/Remove Modules", mainLabelStyle);
        GUILayout.Space(10);
    }

    /// <summary>
    /// Draws a GUI for the installation of modules.
    /// </summary>
    static void DrawModulesInstallGUI()
    {
        // Iterate over the available modules
        foreach (string module in AvailableModules)
        {
            // Get the old value for the selected state of the current module
            bool oldSelectedState = SelectedModules.ContainsKey(module) && SelectedModules[module];

            // Present a toggle button in UI for user to select or unselect the module
            bool newSelectedState = EditorGUILayout.Toggle(module, oldSelectedState);

            // If the selected state of the module has been changed by user
            if (newSelectedState != oldSelectedState)
            {
                // Update the selected state of current module
                SelectedModules[module] = newSelectedState;

                if (VersionManager.DebugVersion)
                {
                    Debug.Log($"Module '{module}' selected state changed to {newSelectedState}");
                    Debug.Log($"Module '{module}' installed state is {InstalledModules[module]}");
                }

                // If the 'Full Package' module is selected or unselected, toggle the selected state of all other modules to match 'Full Package'
                if (module == AvailableModules.First())
                {
                    foreach (string key in AvailableModules.Where(key => key != "Full Package"))
                    {
                        // Update the selected state of each module, excluding 'Full Package'
                        SelectedModules[key] = newSelectedState;
                    }
                }
                else if (!newSelectedState)
                {
                    // If any other module than 'Full Package' is unselected, also unselect the 'Full Package'
                    SelectedModules["Full Package"] = false;
                }
            }

            // If the 'Full Package' module is selected, display 'Includes Extras' info underneath its toggle
            if (module.Equals("Full Package") && newSelectedState)
                using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.LabelField("└ Includes Extras"); }
        }
    }

    /// <summary>
    ///     Draws the help box that explains what the user should do.
    /// </summary>
    void DrawModulesHelpBox()
    {
        string spacer = Environment.NewLine;

        EditorGUILayout.HelpBox
        ($"{spacer}Please choose the modules you wish to install." + $"{spacer}If you are unsure which one(s) to choose, simply select {spacer}\"Full Package\" "            +
         "and all the recommended modules will be installed."      + $"{spacer}The Full Package also includes an \"Extras\" part which itself includes 'Joel's Essentials' " +
         $"as well as an 'Examples' folder with various tips and guides on how the package works.{spacer}", MessageType.Info);

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Click the checkbox to continue.", GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            SafeMode = !EditorGUILayout.Toggle(!SafeMode);
        }
    }

    /// <summary>
    ///     Draws the Apply and Cancel buttons.
    /// </summary>
    void DrawModulesButtons()
    {
        using (new GUILayout.HorizontalScope())
        { // Only install the modules if the user is not in Safe Mode and if there are modules to install.
            if (!SafeMode)
                if (GUILayout.Button("Install Selected", GUILayout.Height(25)))
                {
                    if (!SafeMode && SelectedModules.Values.Any(module => module))
                    {
                        // Popup to confirm the replacement of the old files
                        if (EditorUtility.DisplayDialog
                        ("Confirmation",
                         "Are you sure you want to replace the old files? \n " + "Please backup any files from the old version that you may want to keep." +
                         "\nIt is recommended to backup Systems.prefab in the Resources folder", "Apply", "Cancel"))
                        {
                            //TODO: check if the selected module(s) are already installed, and prompt the user if they want to reinstall the selected.
                            ModuleInstaller.InstallSelectedModules();
                        }
                        else { EssentialsDebugger.LogAbort(SafeMode); }
                    }
                    else { EssentialsDebugger.LogWarning("Please select at least one module to install."); }
                }

            // "Cancel" button
            if (GUILayout.Button("Cancel Setup", GUILayout.Height(25)))
            {
                // Close the setup panel
                currentPanel = DisplayToolbar;

                // Reset the checkboxes
                ModuleInstaller.ClearSelectedModules();

                // Check which modules are still installed
                ModuleInstaller.CheckForInstalledModules();

                SafeMode = true;
            }
        }
    }
}
}
