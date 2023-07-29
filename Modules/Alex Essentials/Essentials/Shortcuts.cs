#region
using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

namespace Lumina.Essentials
{
    public static class Shortcuts
    {
        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static async void ClearConsole()
        {
            ShortcutPressedWarning("Clearing Console...", KeyCode.C, ShortcutModifiers.Alt);

            // Wait for a little to give the reader time to read the warning message.
            await Task.Delay(1500);
            
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new (), null);
        }

        /// <summary>
        ///     Alt + Backspace to reload the scene in the Unity Editor.
        /// </summary>
        [Shortcut("Reload Scene", KeyCode.Backspace, ShortcutModifiers.Alt)]
        public static async void ReloadScene()
        {
            ShortcutPressedWarning("Reloading Scene...", KeyCode.Backspace, ShortcutModifiers.Alt);

            // Wait for a little to give the reader time to read the warning message.
            await Task.Delay(1500);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }

        // Warning method for when a shortcut is pressed.
        static void ShortcutPressedWarning(string warningMessage, KeyCode shortcutKey, ShortcutModifiers shortcutModifiers)
        {
            Debug.LogWarning($"Shortcut {shortcutModifiers} + {shortcutKey} pressed!" + "\n" + warningMessage);
        }
    }
}
