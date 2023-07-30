#region
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
[InitializeOnLoad] 
internal static class Autorun
{
    static Autorun() { EditorApplication.update += OnUpdate; }

    static void OnUpdate()
    {
        if (!SetupWindowIsOpen() && UtilityWindow.SetupRequired && EditorPrefs.GetBool("Init")) 
            SetupWindow.OpenSetupWindow();
    }

    static bool SetupWindowIsOpen() => Resources.FindObjectsOfTypeAll<SetupWindow>().Length > 0;
}
}
