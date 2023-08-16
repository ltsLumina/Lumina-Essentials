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
        // Clear Console Keys
        const KeyCode clearConsoleKey = KeyCode.C;
        const ShortcutModifiers clearConsoleModifiers = ShortcutModifiers.Shift | ShortcutModifiers.Control;
        
        // Reload Scene Keys
        const KeyCode reloadSceneKey = KeyCode.Backspace;
        const ShortcutModifiers reloadSceneModifiers = ShortcutModifiers.Shift | ShortcutModifiers.Control;

        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", clearConsoleKey, clearConsoleModifiers)]
        public static async void ClearConsole()
        {
            ShortcutPressedWarning("Clearing Console...", clearConsoleKey,clearConsoleModifiers);

            // Wait for a little to give the reader time to read the warning message.
            await Task.Delay(1500);
            
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            if (assembly == null) return;
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new (), null);
        }

        /// <summary>
        ///     Alt + Backspace to reload the scene in the Unity Editor.
        /// </summary>
        [Shortcut("Reload Scene", reloadSceneKey, reloadSceneModifiers)]
        public static async void ReloadScene()
        {
            ShortcutPressedWarning("Reloading Scene...", reloadSceneKey, reloadSceneModifiers);

            // Wait for a little to give the reader time to read the warning message.
            await Task.Delay(1500);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }

        // Warning method for when a shortcut is pressed.
        static void ShortcutPressedWarning(string warningMessage, KeyCode shortcutKey, ShortcutModifiers shortcutModifier1, ShortcutModifiers shortcutModifiers2 = default)
        {
            var shortcutStr = $"{shortcutKey} pressed.";

            if (!Equals(shortcutModifier1, default(ShortcutModifiers))) shortcutStr = $"{shortcutModifier1} + " + shortcutStr;
            if (!Equals(shortcutModifiers2, default(ShortcutModifiers))) shortcutStr = $"{shortcutModifiers2} + " + shortcutStr;

            Debug.LogWarning(shortcutStr + "\n" + warningMessage);
        }
    }
}
