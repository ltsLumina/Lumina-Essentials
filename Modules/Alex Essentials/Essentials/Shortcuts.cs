#region
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif
#endregion

#pragma warning disable CS1998

namespace Lumina.Essentials
{
public static class Shortcuts
{
#if UNITY_EDITOR
        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
#endif
    public static async void ClearConsole()
    {
#if UNITY_EDITOR
        ShortcutPressedWarning("Clearing Console...", KeyCode.C, ShortcutModifiers.Alt);

        // Wait for a little to give the reader time to read the warning message.
        await Task.Delay(1500);

        var        assembly = Assembly.GetAssembly(typeof(SceneView));
        Type       type     = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method   = type.GetMethod("Clear");
        method?.Invoke(new (), null);
#endif
    }

#if UNITY_EDITOR
        /// <summary>
        ///     Alt + Backspace to reload the scene in the Unity Editor.
        /// </summary>
        [Shortcut("Reload Scene", KeyCode.Backspace, ShortcutModifiers.Alt)]
#endif
    public static async void ReloadScene()
    {
#if UNITY_EDITOR
        ShortcutPressedWarning("Reloading Scene...", KeyCode.Backspace, ShortcutModifiers.Alt);

        // Wait for a little to give the reader time to read the warning message.
        await Task.Delay(1500);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
    }

#if UNITY_EDITOR
    // Warning method for when a shortcut is pressed.
    static void ShortcutPressedWarning(string warningMessage, KeyCode shortcutKey, ShortcutModifiers shortcutModifiers)
    {
        Debug.LogWarning($"Shortcut {shortcutModifiers} + {shortcutKey} pressed!" + "\n" + warningMessage);
    }
#endif
}
}
