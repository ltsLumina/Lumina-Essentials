#region
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
public class DirectoryUtilities : MonoBehaviour
{
    #region Check for Folder/File
    internal static bool IsFolderInProject(string baseDirectory, string targetFolderName)
    {
        try
        {
            // using SearchOption.AllDirectories to recursively search the whole directory
            string[] directories = Directory.GetDirectories(baseDirectory, targetFolderName, SearchOption.AllDirectories);

            // If the GetDirectories method returns any directories, it means the target folder exist in the project
            return directories.Length > 0;
        } catch (Exception ex)
        {
            // Handle exception, mostly due to lack of access to some directories
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    internal static bool IsFileInProject(string baseDirectory, string targetFileName)
    {
        try
        {
            string[] files = Directory.GetFiles(baseDirectory, targetFileName, SearchOption.AllDirectories);
            return files.Length > 0; // If any files are found, return true
        } catch (Exception ex)
        {
            // Handle exception, mostly due to lack of access to some directories
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
    #endregion

    // ReSharper disable Unity.PerformanceAnalysis
    internal static void CreateDirectories(string root, params string[] directories)
    {
        string fullpath = Path.Combine(Application.dataPath, root);

        if (!Directory.Exists(fullpath))
        {
            Directory.CreateDirectory(fullpath);

            if (VersionManager.DebugVersion) EssentialsDebugger.Log("Successfully created directory: " + fullpath);
        }

        foreach (string newDirectory in directories)
        {
            string newFullPath = Path.Combine(fullpath, newDirectory);

            if (!Directory.Exists(newFullPath))
            {
                Directory.CreateDirectory(newFullPath);

                if (VersionManager.DebugVersion) EssentialsDebugger.Log("Successfully created directory: " + newFullPath);
            }
        }
    }

    internal static string GetFolderNameFromString(string str)
    {
        // Assign a default string in case directoryInfo.Name returns an empty string.
        if (string.IsNullOrEmpty(str)) str = "New Folder";

        var directoryInfo = new DirectoryInfo(str);
        return directoryInfo.Name;
    }

    internal static void DeleteAutorunFiles()
    {
        string       mainDirectory   = Path.Combine("Lumina's Essentials", "Editor");
        const string targetDirectory = "Management";
        const string targetFile      = "Autorun.cs";

        string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string directory in allDirectories)
        {
            // Get the relative path from Assets
            string relativePath = $"Assets{directory[Application.dataPath.Length..]}";

            // If directory is within "Lumina's Essentials/Modules"
            if (!Path.GetFullPath(relativePath).EndsWith(mainDirectory)) continue;

            // Get all subdirectories within the main directory.
            string[] subDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);

            // For each "Management" directory among the subdirectories
            foreach (string sub in subDirectories.Where(sub => Path.GetFileName(sub).Equals(targetDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                // Formulate the path for the Autorun.cs file in the Unity's asset path format
                string autoRunFilePath  = Path.Combine(sub, targetFile).Replace("\\", "/");
                string autoRunAssetPath = $"Assets{autoRunFilePath[Application.dataPath.Length..]}";

                // If Autorun.cs file exists
                if (File.Exists(autoRunFilePath))
                {
                    // Delete the file
                    if (!VersionManager.DebugVersion) AssetDatabase.DeleteAsset(autoRunAssetPath);
                    else EssentialsDebugger.LogWarning("Can't delete Autorun.cs in debug mode.");

                    AssetDatabase.Refresh();

                    if (VersionManager.DebugVersion) EssentialsDebugger.Log($"Autorun.cs deleted from {autoRunFilePath}");
                }
                else if (VersionManager.DebugVersion)
                {
                    EssentialsDebugger.LogError($"Autorun.cs not found in {autoRunFilePath}\n └ <i>(This is not an error. It just means that the file does not exist.)</i>");
                }
            }
        }
    }
}
}
