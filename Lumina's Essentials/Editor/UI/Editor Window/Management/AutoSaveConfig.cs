using UnityEditor;
using UnityEngine;

namespace Lumina.Essentials.Editor.UI.Management
{
    public static class AutoSaveConfig
    {
        [Tooltip("The frequency in minutes auto save will activate")]
        internal static int Interval = 1;

        [Tooltip("Log a message every time the scene is auto saved")]
        internal static bool Logging = true;

        [Tooltip("Enable auto save functionality")]
        internal static bool Enabled
        {
            get => EditorPrefs.GetBool("AutoSaveEnabled", false);
            set => EditorPrefs.SetBool("AutoSaveEnabled", value);
        }
    }
}
