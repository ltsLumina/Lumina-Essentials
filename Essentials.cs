#region
#region
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
#endregion

public static partial class Essentials
{
    internal static class UsefulShortcuts
    {
        /// <summary>
        ///     A collection of debugging shortcuts.
        ///     Includes keyboard shortcuts tied to the F-keys, as well as context menus.
        ///     Note: These methods are local functions, and are only accessible within this method.
        /// </summary>

#if UNITY_EDITOR //!WARNING! This class is only accessible in the Unity Editor, and may cause errors when building the game.
        [Shortcut("Damage Player", KeyCode.F1), ContextMenu("Damage Player")]
        static void DamagePlayer()
        {
            // Damage the player by 10.
            GameManager.Instance.Player.CurrentHealth -= 10;
            Debug.Log("Player damaged.");
        }

        [Shortcut("Heal Player", KeyCode.F2), ContextMenu("Heal Player")]
        static void HealPlayer()
        {
            // Heal the player by 10.
            GameManager.Instance.Player.CurrentHealth += 10;
            Debug.Log("Player healed.");
        }

        [Shortcut("Kill Player", KeyCode.F3), ContextMenu("Kill Player")]
        static void KillPlayer()
        {
            // Kill the player.
            GameManager.Instance.Player.CurrentHealth = 0;
            Debug.Log("Player killed.");
        }

        [Shortcut("Reload Scene", KeyCode.F5), ContextMenu("Reload Scene")]
        static void ReloadScene()
        {
            // Reload Scene
            SceneManagerExtended.ReloadScene();
            Debug.Log("Scene reloaded.");
        }
#endif
    }

    internal static class UsefulMethods
    {
        /// <summary>
        ///     Alt+ C to clear the console.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static void ClearConsole()
        {
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new(), null);
        }

        /// <summary>
        ///     Allows you to call a method after a delay through the use of an asynchronous operation.
        /// </summary>
        /// <example> DelayedTaskAsync(() => action(), delayInSeconds, debugLog, cancellationToken).AsTask(); </example>
        /// <remarks> To run a method after the task is completed: Task delayTask = delayTask.ContinueWith(_ => action();</remarks>
        /// <param name="action">The action or method to run. Use delegate lambda " () => " to run. </param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="debugLog">Whether or not to debug the waiting message.</param>
        /// <param name="cancellationToken"> Token for cancelling the currently running task. Not required. </param>
        internal static async UniTask DelayedTaskAsync(
            Action action, float delayInSeconds, bool debugLog = false, CancellationToken cancellationToken = default)
        {
            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");
            TimeSpan timeSpan = TimeSpan.FromSeconds(delayInSeconds);
            await UniTask.Delay(timeSpan, cancellationToken: cancellationToken);
            action();
            if (debugLog) Debug.Log("Action completed.");
        }

        /// <summary>
        ///     !WARNING! This method is not asynchronous, and will block the main thread, causing the game to freeze.
        ///     However, the purpose of this method is to allow you to call a method after a delay, without having to make the
        ///     method asynchronous.
        ///     Unsure if this method even works in its current state.
        /// </summary>
        /// <param name="action">The action or method to run.</param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="debugLog">Whether or not to debug the waiting message and the completion message.</param>
        /// <param name="onComplete">An action to be completed after the initial action is finished. Not required to be used.</param>
        [Obsolete("This method is not finished or has been deprecated. Use 'DoAfterDelayAsync' instead.")]
        public static void DelayedTask(
            Action action, int delayInSeconds, bool debugLog = false, Action onComplete = null)
        {
            if (debugLog) Debug.Log("Waiting for " + delayInSeconds + " seconds...");

            Task.Delay(delayInSeconds).ContinueWith
            (_ =>
            {
                Thread.Sleep(delayInSeconds);
                action();
                if (debugLog) Debug.Log("Action completed.");
                onComplete?.Invoke();
            });
        }
    }
}
#endregion

public static partial class Essentials
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    /// <summary>
    ///     Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
    ///     Small but useful script, to make your inspectors look pretty and useful :D
    ///     <example> [SerializedField, ReadOnly] int myInt; </example>
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}