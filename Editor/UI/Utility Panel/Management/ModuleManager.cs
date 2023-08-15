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
    #region Checks for Modules
        internal static void CheckForInstalledModules()
        {
            string projectDirectory = Application.dataPath;

            if (CheckFullPackageInstalled())
            {
                // Set all keys in InstalledModules to true
                foreach (var module in UtilityPanel.InstalledModules.Keys.ToList())
                {
                    UtilityPanel.InstalledModules[module] = true;

                    if (VersionManager.DebugVersion)
                    {
                        EssentialsDebugger.LogWarning
                        ("Full Package is installed. This means that all modules are installed and any changes to the folders for debugging purposes will be ignored. " +
                         "\nTo test the module installation, please remove the Full Package (Examples folder) from the project.");
                        
                        // Don't print the "Full Package" debug as it will always be false by this implementation.
                        if (module != "Full Package") Debug.Log($"Item '{module}' exists in the project.");
                    }
                }

                // No need to continue if the full package is installed
                return;
            }

            var keys = new List<string>(UtilityPanel.InstalledModules.Keys);

            foreach (var item in keys)
            {
                string itemFile = item + ".cs";

                if (DirectoryUtilities.IsFolderInEssentials(projectDirectory, item) || DirectoryUtilities.IsFileInEssentials(projectDirectory, itemFile))
                {
                    if (VersionManager.DebugVersion) { Debug.Log($"Item '{item}' exists in the project."); }
                    UtilityPanel.InstalledModules[item] = true;
                }
                else
                {
                    if (VersionManager.DebugVersion) { Debug.Log($"Item '{item}' does NOT exist in the project."); }
                    UtilityPanel.InstalledModules[item] = false;
                }
            }
        }

        static bool CheckFullPackageInstalled()
        {
            string       mainDirectory   = Path.Combine("Lumina's Essentials", "Modules");
            const string targetDirectory = "Examples";

            var allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

            foreach (var directory in allDirectories)
            {
                // Get the relative path from Assets
                var relativePath = directory[(Application.dataPath.Length - "Assets".Length)..];

                // If directory is within "Lumina's Essentials/Modules"
                if (Path.GetFullPath(relativePath).EndsWith(mainDirectory))
                {
                    // Get all subdirectories within the main directory.
                    var subDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);

                    // Check if "Examples" directory exists within any of the subdirectories
                    if (subDirectories.Any(sub => Path.GetFileName(sub).Equals(targetDirectory, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (VersionManager.DebugVersion)
                        {
                            Debug.Log("Full package is installed.");
                        }
                        return true;
                    }
                }
            }

            if (VersionManager.DebugVersion) { Debug.Log("Full package is NOT installed."); }
            return false;
        }

        internal static void InstallSelectedModules()
{
    // Before doing anything, check which modules are installed.
    CheckForInstalledModules();

    const string targetDirectory = "Lumina's Essentials/Editor/Packages";
    string       relativePath    = GetRelativePath(targetDirectory);

    if (string.IsNullOrEmpty(relativePath))
    {
        Debug.LogError(targetDirectory + " not found.");
        return;
    }

    SetupImportCallbacks();

    AssetDatabase.StartAssetEditing(); // Start the AssetEditing here.
    try
    {
        // Compare selected and installed modules
        foreach (var module in UtilityPanel.SelectedModules)
        {
            // If the module is selected.
            if (!module.Value) continue;

            // Handle "Full Package" separately
            if (module.Key == "Full Package")
            {
                if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
                {
                    bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
                    if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
                }
                else // Full Package is selected but not installed, install directly.
                {
                    ImportModulePackage(relativePath, module.Key);
                    DirectoryUtilities.DeleteAutorunFiles();
                }
                return; // Stop further processing once Full Package is handled
            }

            // For other modules
            if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
            {
                bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
                if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
            }
            else // If not installed, install directly.
            {
                ImportModulePackage(relativePath, module.Key);
                DirectoryUtilities.DeleteAutorunFiles();
            }
        }
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


        static bool ModuleInstallConfirmation(string moduleName) => 
            EditorUtility.DisplayDialog
            (
            "Module Installation Warning",
            $"The module \"{moduleName}\" is already installed. Would you like to reinstall it?", "Yes", "No"
            );

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
        }

        static void SetupImportCallbacks()
        {
            AssetDatabase.importPackageCompleted += packageName => { Debug.Log("Imported: " + packageName); };
            AssetDatabase.importPackageFailed += (packageName, errorMessage) => { Debug.LogError("Failed to import: " + packageName + "\n" + errorMessage); };
            AssetDatabase.importPackageCancelled += packageName => { Debug.Log("Cancelled importing: " + packageName); };
        }

        internal static void ClearSelectedModules()
        {
            foreach (var module in UtilityPanel.SelectedModules.ToList()) { UtilityPanel.SelectedModules[module.Key] = false; }
        }
        // -- End of Module Checks --
        #endregion
}
}
