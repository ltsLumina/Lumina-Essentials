#region
using System;
using JetBrains.Annotations;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.VersionManager;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
    internal static class EssentialsDebugger
    {
        /// <summary>
        ///     The log level to be used when logging messages.
        ///     <param name="Quiet"> Does not log any messages. </param>
        ///     <param name="Verbose"> Logs all messages. </param>
        /// </summary>
        internal static LogLevel LogBehaviour { get; set; } = LogLevel.Verbose;

        internal enum LogLevel
        {
            [UsedImplicitly] // An option in the Settings menu of the Utility Window. 
            Quiet,
            Verbose,
        }

        const string ErrorMessagePrefix = "<color=orange>[Lumina Essentials] ►</color>";
        const string DefaultErrorMessage = "An Error Has Occurred:";

        internal static void Log(string message)
        {
            if (!DebugVersion && LogBehaviour == LogLevel.Verbose) 
                Debug.Log($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        internal static void LogWarning(string message)
        {
            if (!DebugVersion && LogBehaviour == LogLevel.Verbose) 
                Debug.LogWarning($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        internal static void LogAbort(bool safeMode = false) => 
            Debug.LogWarning($"{ErrorMessagePrefix} The action was aborted. " + $"\n{(safeMode ? "Safe Mode is enabled." : "")}");

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        ///     Log error message with specific format.
        /// </summary>
        /// <param name="message">The custom message to be logged.</param>
        internal static void LogError(string message)
        {
            if (!DebugVersion && LogBehaviour == LogLevel.Verbose) 
                Debug.LogError($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");
        }

        /// <summary>
        ///     Log exception message with specific format.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        internal static void LogError(Exception exception)
        {
            if (!DebugVersion && LogBehaviour == LogLevel.Verbose) 
                Debug.LogError($"{ErrorMessagePrefix} {exception.Message ?? DefaultErrorMessage}");
        }
    }
}
