#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
public class ModuleInstaller : MonoBehaviour
{
    static string relativePath;
    
    #region Checks for Modules
    internal static void CheckForInstalledModules()
    {
        bool isDebugVersion = VersionManager.DebugVersion;

        if (FullPackageIsInstalled())
        {
            SetAllModulesTo(true, isDebugVersion);
            return;
        }

        CheckIndividualModules(isDebugVersion);
    }

    static void SetAllModulesTo(bool state, bool debugVersion)
    {
        foreach (string module in UtilityPanel.InstalledModules.Keys.ToList())
        {
            UtilityPanel.InstalledModules[module] = state;

            if (!debugVersion) return;

            EssentialsDebugger.LogWarning
            ("Full Package is installed. This means that all modules are installed and any changes to the folders for debugging purposes will be ignored. " +
             "\nTo test the module installation, please remove the Full Package (Examples folder) from the project.");

            if (module != "Full Package") DebugLogState(debugVersion, state, module);
        }
    }

    static void CheckIndividualModules(bool debugVersion)
    {
        foreach (string item in new List<string>(UtilityPanel.InstalledModules.Keys)) { SetModuleInstalledState(item, debugVersion); }
    }

    static void SetModuleInstalledState(string item, bool debugVersion)
    {
        string projectDirectory = Application.dataPath;
        string itemFile         = item + ".cs";

        bool exists = DirectoryUtilities.IsFolderInEssentials(projectDirectory, item) || DirectoryUtilities.IsFileInEssentials(projectDirectory, itemFile);

        DebugLogState(debugVersion, exists, item);
        UtilityPanel.InstalledModules[item] = exists;
    }

    static void DebugLogState(bool debugVersion, bool state, string module)
    {
        if (!debugVersion) return;

        string stateMessage = state ? "exists in the project." : "does NOT exist in the project.";
        Debug.Log($"Item '{module}' {stateMessage}");
    }

    static bool FullPackageIsInstalled()
    {
        string       mainDirectory   = Path.Combine("Lumina's Essentials", "Modules");
        const string targetDirectory = "Examples";

        string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string directory in allDirectories)
        {
            // Get the relative path from Assets
            relativePath = directory[(Application.dataPath.Length - "Assets".Length)..];

            // If directory is within "Lumina's Essentials/Modules" (Why does this work?)
            if (Path.GetFullPath(relativePath).EndsWith(mainDirectory))
            {
                // Get all subdirectories within the main directory.
                string[] subDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);

                // Check if "Examples" directory exists within any of the subdirectories
                if (subDirectories.Any(sub => Path.GetFileName(sub).Equals(targetDirectory, StringComparison.OrdinalIgnoreCase)))
                {
                    if (VersionManager.DebugVersion) Debug.Log("Full package is installed.");
                    return true;
                }
            }
        }

        if (VersionManager.DebugVersion) Debug.Log("Full package is NOT installed.");
        return false;
    }

    internal static void InstallSelectedModules()
    {
        // Before doing anything, check which modules are installed.
        CheckForInstalledModules();

        // Find the path to the "Lumina's Essentials/Editor/Packages" folder
        const string targetDirectory = "Lumina's Essentials/Editor/Packages";
        relativePath = GetRelativePath(targetDirectory);

        if (string.IsNullOrEmpty(relativePath))
        {
            Debug.LogError(targetDirectory + " not found.");
            return;
        }

        SetupImportCallbacks();

        AssetDatabase.StartAssetEditing();

        try
        {
            InstallModules();
        } 
        catch (Exception e)
        {
            EssentialsDebugger.LogError("Failed to install module(s)" + "\n" + e.Message + "\n" + e.StackTrace);
            throw;
        }
        finally
        {
            // Stop the AssetEditing at the end, even if some error was thrown.
            AssetDatabase.StopAssetEditing();
        }
    }

    static void InstallModules()
    {
        // Compare selected and installed modules
        foreach (KeyValuePair<string, bool> module in UtilityPanel.SelectedModules.Where(module => module.Value))
        {
            if (HandleFullPackage(module)) return;

            HandleOtherModules(module);
        }
    }

    static bool HandleFullPackage(KeyValuePair<string, bool> module)
    { // Handle "Full Package" separately
        if (module.Key != "Full Package") return false;

        if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
        {
            bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
            if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
        }
        else // Full Package is selected but not installed, install directly.
        {
            ImportModulePackage(relativePath, module.Key);
        }

        return true;
    }
    static void HandleOtherModules(KeyValuePair<string, bool> module)
    { // For other modules
        if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
        {
            bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
            if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
        }
        else // If not installed, install directly.
        {
            ImportModulePackage(relativePath, module.Key);
        }
    }

    static bool ModuleInstallConfirmation(string moduleName) => EditorUtility.DisplayDialog
        ("Module Installation Warning", $"The module \"{moduleName}\" is already installed. Would you like to reinstall it?", "Yes", "No");

    static string GetRelativePath(string targetDirectory)
    {
        string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string directory in allDirectories)
        {
            string pathFromAssets = directory.Replace(Application.dataPath, "Assets");

            if (pathFromAssets.Replace("\\", "/").EndsWith(targetDirectory)) return pathFromAssets + "/";
        }

        return string.Empty;
    }

    static void ImportModulePackage(string relativePath, string moduleName)
    {
        string modulePath = Path.Combine(relativePath, moduleName) + ".unitypackage";
        AssetDatabase.ImportPackage(modulePath, false);

        // Delete the Autorun.cs file from the project
        DirectoryUtilities.DeleteAutorunFiles();
    }

    static void SetupImportCallbacks()
    {
        AssetDatabase.importPackageCompleted += packageName => { Debug.Log("Imported: "                              + packageName); };
        AssetDatabase.importPackageFailed    += (packageName, errorMessage) => { Debug.LogError("Failed to import: " + packageName + "\n" + errorMessage); };
        AssetDatabase.importPackageCancelled += packageName => { Debug.Log("Cancelled importing: "                   + packageName); };
    }

    internal static void ClearSelectedModules()
    {
        foreach (KeyValuePair<string, bool> module in UtilityPanel.SelectedModules.ToList()) { UtilityPanel.SelectedModules[module.Key] = false; }
    }

    // -- End of Module Checks --
    #endregion
}
}
