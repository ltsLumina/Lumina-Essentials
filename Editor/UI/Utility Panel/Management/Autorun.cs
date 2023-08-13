#region
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
[InitializeOnLoad]
internal static class Autorun
{
    static Autorun()
    {
        Debug.Log("Auto-Run Initialized.");
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        Debug.Log("setup window open: " + SetupWindowIsOpen());
        Debug.Log("utility window open: " + UtilityWindowIsOpen());
        Debug.Log("setup required: " + UtilityWindow.SetupRequired);
        
        if (!SetupWindowIsOpen() && !UtilityWindowIsOpen() && UtilityWindow.SetupRequired)
        {
            SetupWindow.OpenSetupWindow();
        }
    }

    static bool SetupWindowIsOpen() => Resources.FindObjectsOfTypeAll<SetupWindow>().Length > 0;

    static bool UtilityWindowIsOpen() => Resources.FindObjectsOfTypeAll<UtilityWindow>().Length > 0;
}
}
