#region
using System;
using System.Text.RegularExpressions;
using UnityEditor;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
/// <summary>
///    Manages the version of Lumina's Essentials.
/// </summary>
internal static class VersionManager
{
    /// <summary> The current version of Lumina's Essentials. </summary>
    internal static string CurrentVersion => "1.2.3 Beta3 Debug";
    /// <summary> The latest version of Lumina's Essentials available on GitHub. </summary>
    internal static string LatestVersion
    {
        get => EditorPrefs.GetString("LatestVersion", null);
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                EditorPrefs.SetString("LatestVersion", "Error fetching version..." +
                "\n  └ <i>(Are you connected to the internet?)</i>");
            }
            else
            {
                EditorPrefs.SetString("LatestVersion", "ver. " + value);
            }
        }
    }
    
    /// <summary> The version of the package that was last opened. </summary>
    internal static string LastOpenVersion
    {
        get => EditorPrefs.GetString("LastOpenVersion", "Unknown");
        set => EditorPrefs.SetString("LastOpenVersion", value);
    }
    
    /// <summary> Whether or not the current version is a debug version. </summary>
    internal static bool DebugVersion => CurrentVersion.Contains("Debug", StringComparison.OrdinalIgnoreCase);
    internal static bool DontShow_DebugBuildWarning
    {
        get => EditorPrefs.GetBool("DontShow_DebugBuildWarning");
        set => EditorPrefs.SetBool("DontShow_DebugBuildWarning", value);
    }

    internal static void UpdatePrefs()
    {
        EditorPrefs.SetString("CurrentVersion", CurrentVersion);
        EditorPrefs.SetString("LatestVersion", LatestVersion ?? "Error fetching version..");
        EditorPrefs.SetBool("UpToDate", CompareVersions(CurrentVersion, LatestVersion));
        EditorPrefs.SetBool("DebugVersion", DebugVersion);
    }

    /// <summary>
    ///     Opens the window on startup if the version has changed.
    ///     This method is called automatically on startup.
    /// </summary>
    [InitializeOnLoadMethod]
    static void OpenWindowOnStartup()
    {
        string currentVersion  = CurrentVersion;
        string lastOpenVersion = EditorPrefs.GetString("LastOpenVersion", "Unknown");
        string latestVersion   = LatestVersion;

        // If an update has occured since the last time the window was opened, open the window.
        if (CompareVersions(currentVersion, lastOpenVersion))
        {
            SetupWindow.OpenSetupWindow(true);
            EditorPrefs.SetString("LastOpenVersion", currentVersion);
        }

        // If the current version is newer than the *latest* version, perform update checks (I.e; if the user is up-to-date)
        if (StartupChecks.IsNewVersionAvailable(currentVersion, latestVersion) || !EditorPrefs.GetBool("UpToDate")) StartupChecks.DisplayVersionAlert();
    }

    /// <summary>
    ///     Compares a version string to different version string.
    ///     If v1 is newer than v2, the action is performed.
    /// </summary>
    /// <param name="v1"> The first version to compare. </param>
    /// <param name="v2"> The second version to compare. </param>
    /// <param name="action"> The action to perform if the current version is newer than the last opened version. </param>
    /// <returns> Whether or not the current version is newer than the last opened version. </returns>
    internal static bool CompareVersions(string v1, string v2, Action action = default)
    {
        // extract the numeric parts of the versions
        var   regex   = new Regex(@"(\d+(\.\d+){0,3})");
        Match matchV1 = regex.Match(v1);
        Match matchV2 = regex.Match(v2);

        // if either version string doesn't contain a valid version number, return false
        if (!matchV1.Success || !matchV2.Success) return false;

        // convert the numeric parts of the versions to Version objects
        var version1 = new Version(matchV1.Value);
        var version2 = new Version(matchV2.Value);

        // compare the versions
        bool versionsDifferent = version1.CompareTo(version2) > 0;

        if (versionsDifferent) action?.Invoke();

        return versionsDifferent;
    }
}
}
