using System;
using UnityEngine;

namespace Lumina.Essentials.Editor
{
    public static class DebugHelper
    {
        const string ErrorMessagePrefix = "<color=orange>[Lumina Essentials] ►</color>";
        const string DefaultErrorMessage = "An Error Has Occurred:";

        public static void Log(string message) => Debug.Log($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");
        
        public static void LogWarning(string message) => Debug.LogWarning($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");
        
        public static void LogAbort(bool safeMode) => Debug.LogWarning(
            $"{ErrorMessagePrefix} The action was aborted. " +
            $"\n{(safeMode ? "Safe Mode is enabled." : "")}");
        
        /// <summary>
        /// Log error message with specific format.
        /// </summary>
        /// <param name="message">The custom message to be logged.</param>
        public static void LogError(string message) => Debug.LogError($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");

        /// <summary>
        /// Log exception message with specific format.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        public static void LogError(Exception exception) => Debug.LogError($"{ErrorMessagePrefix} {DefaultErrorMessage} \n {exception}");
    }
}
