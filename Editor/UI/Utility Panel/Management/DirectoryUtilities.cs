#region
using System;
using System.IO;
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
                var directories = Directory.GetDirectories(baseDirectory, targetFolderName, SearchOption.AllDirectories);

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
                var files = Directory.GetFiles(baseDirectory, targetFileName, SearchOption.AllDirectories);
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
            var fullpath = Path.Combine(Application.dataPath, root);

            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);

                if (VersionManager.DebugVersion)
                {
                    EssentialsDebugger.Log("Successfully created directory: " + fullpath);
                }
            }

            foreach (var newDirectory in directories)
            {
                var newFullPath = Path.Combine(fullpath, newDirectory);

                if (!Directory.Exists(newFullPath))
                {
                    Directory.CreateDirectory(newFullPath);

                    if (VersionManager.DebugVersion)
                    {
                        EssentialsDebugger.Log("Successfully created directory: " + newFullPath);
                    }
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
}
}
