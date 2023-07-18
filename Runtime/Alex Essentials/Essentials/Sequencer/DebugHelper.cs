using System;
using UnityEngine;

namespace Lumina.Essentials.Sequencer
{
    internal static class DebugHelper
    {
        const string ErrorMessagePrefix = "<color=orange>[Lumina Essentials] ►</color>";
        const string DefaultErrorMessage = "An Error Has Occurred:";

        /// <summary>
        /// Log error message with specific format.
        /// </summary>
        /// <param name="message">The custom message to be logged.</param>
        internal static void LogError(string message) => Debug.LogError($"{ErrorMessagePrefix} {message ?? DefaultErrorMessage}");

        /// <summary>
        /// Log exception message with specific format.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        internal static void LogError(Exception exception) => Debug.LogError($"{ErrorMessagePrefix} {DefaultErrorMessage} \n {exception}");
    }
}
