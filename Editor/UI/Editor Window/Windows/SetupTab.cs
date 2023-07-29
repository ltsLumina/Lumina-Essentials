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

    internal readonly static Dictionary<string, bool> SelectedModules = new ();

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
    ///     Draws the GUI that allows the user to select which modules to install.
    /// </summary>
    static void DrawModulesInstallGUI()
    {
        foreach (string module in AvailableModules)
        {
            bool oldValue = SelectedModules.ContainsKey(module) && SelectedModules[module];
            bool newValue = EditorGUILayout.Toggle(module, oldValue);

            // Display Extras right under Full Package whenever Full Package is selected
            if (module.Equals("Full Package") && newValue)
                using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.LabelField("└ Includes Extras"); }

            if (newValue != oldValue)
            {
                SelectedModules[module] = newValue;

                // If changing the 'Full Package', change all other modules as well
                if (module == AvailableModules.First())
                {
                    // If 'Full Package' is selected, select all modules
                    // If 'Full Package' is unselected, unselect all modules
                    foreach (string key in AvailableModules.Where(key => key != "Full Package")) { SelectedModules[key] = newValue; }
                }
                else if (!newValue)
                {
                    // If any module is unselected, unselect the 'Full Package'
                    SelectedModules["Full Package"] = false;
                }

                // If current module is 'Full Package' and is selected, display 'Extras'
                if (module.Equals("Full Package") && newValue)
                    using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.LabelField("└ Extras"); }
            }
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
            EditorGUILayout.LabelField("Uncheck the checkbox to continue.", GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            SafeMode = EditorGUILayout.Toggle(SafeMode);
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
                            InstallModules();
                            SetupRequired = false;
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
                ClearSelectedModules();

                // Check which modules are still installed
                CheckForInstalledModules();

                SafeMode = true;
            }
        }
    }
}
}
