#region
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
[InitializeOnLoad]
internal static class Autorun
{
    static Autorun() => EditorApplication.update += OnUpdate;

    static void OnUpdate()
    {
        if (!UpgradeWindowIsOpen() && !UtilityPanelIsOpen() && UtilityPanel.SetupRequired && UpgradeWindow.WindowClosedCount <= 4)
        {
            UpgradeWindow.Open();
            VersionUpdater.CheckForUpdates();
        }
    }

    static bool UpgradeWindowIsOpen() => Resources.FindObjectsOfTypeAll<UpgradeWindow>().Length > 0;

    static bool UtilityPanelIsOpen() => Resources.FindObjectsOfTypeAll<UtilityPanel>().Length > 0;
}
}
