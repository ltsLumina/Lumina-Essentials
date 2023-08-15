#region
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
[InitializeOnLoad] // The entirety of this class gets deleted upon installing any module.
internal static class Autorun
{
    static bool alreadyCheckedForUpdate; // Check for updates once

    static Autorun() => EditorApplication.update += OnUpdate;

    static void OnUpdate()
    {
        if (!UpgradeWindowIsOpen() && !UtilityPanelIsOpen() && UtilityPanel.SetupRequired && UpgradeWindow.WindowClosedCount <= 4)
        {
            UpgradeWindow.Open();

            if (alreadyCheckedForUpdate)
                return; // Only run the update check once. This is to prevent a GitHub API call every time the user opens the Utility Panel, eventually leading to a rate limit.

            VersionUpdater.CheckForUpdates();
            alreadyCheckedForUpdate = true;
        }
    }

    static bool UpgradeWindowIsOpen() => Resources.FindObjectsOfTypeAll<UpgradeWindow>().Length > 0;

    static bool UtilityPanelIsOpen() => Resources.FindObjectsOfTypeAll<UtilityPanel>().Length > 0;
}
}
