using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lumina.Essentials.Editor.UI.Management
{
    internal static class EditorGUIUtils
    {
        // Image that will be displayed at the top of the window.
        internal static Texture2D headerImg, footerImg;

        // Styles for the title and the buttons.
        internal static GUIStyle middleStyle;
        internal static GUIStyle leftStyle;
        internal static GUIStyle mainLabelStyle;
        internal static GUIStyle buttonStyle;
        internal static GUIStyle buttonSetup;
        internal static GUIStyle centerLabelStyle;
        internal static GUIStyle subLabelStyle;
        internal static GUIStyle buttonBigStyle; 
        internal static GUIStyle wrapCenterLabelStyle;
        internal static GUIStyle wordWrapRichTextLabelStyle;
        internal static GUIStyle btImgStyle;
        internal static GUIStyle boldLabelStyle;
        internal static GUIStyle setupLabelStyle;
        internal static GUIStyle setupWindowStyle;
        internal static GUIStyle dropAreaStyle;
        internal static GUIStyle setupWindowHeaderStyle;

        // -- GUIContent -- //
        internal static GUIContent safeModeWarningContent;
        internal static GUIContent createDefaultProjectContent;
        internal static GUIContent configureImagesContent;
        internal static GUIContent createSubfolderContent; // Deprecated. Kept for reference.
        internal static GUIContent enterPlaymodeOptionsContent;
        internal static GUIContent resetButtonContent;
        
        // -- GUIContent for the Setup Panel -- //
        internal static GUIContent openDocumentationContent;
        internal static GUIContent openChangeLogContent;
        internal static GUIContent checkForUpdatesContent;
        internal static GUIContent openKnownIssuesContent;

        internal static void SetGUIStyles()
        {
            #region Styles
            // Default style
            middleStyle = new ()
            { richText  = true,
              alignment = TextAnchor.MiddleCenter,
              fontSize  = 12,
              fontStyle = FontStyle.Bold,
              wordWrap  = true,
              normal = new ()
              { textColor = new (0.86f, 0.86f, 0.86f) } };
            
            // copy of middle style
            leftStyle = new ()
            { richText  = true,
              alignment = TextAnchor.MiddleLeft,
              fontSize  = 12,
              fontStyle = FontStyle.Bold,
              wordWrap  = true,
              normal = new ()
              { textColor = new (0.86f, 0.86f, 0.86f) } };

            mainLabelStyle = new ()
            { richText  = true,
              alignment = TextAnchor.MiddleLeft,
              fontSize  = 12,
              fontStyle = FontStyle.Bold,
              normal = new ()
              { textColor = new (0.86f, 0.86f, 0.86f) } };
            
            

            // Button Styles
            buttonStyle         = new (GUI.skin.button);
            buttonStyle.padding = new RectOffset(0, 0, 10, 10);

            buttonSetup          = new GUIStyle(buttonStyle);
            buttonSetup.padding  = new RectOffset(10, 10, 6, 6);
            buttonSetup.wordWrap = true;
            buttonSetup.richText = true;

            // Utilities Styles
            centerLabelStyle = new ()
            { richText  = true,
              alignment = TextAnchor.UpperCenter,
              fontSize  = 12,
              fontStyle = FontStyle.Bold,
              normal = new ()
              { textColor = new (0.74f, 0.74f, 0.74f) } };

            subLabelStyle = new ()
            { richText  = true,
              alignment = TextAnchor.UpperCenter,
              fontSize  = 11,
              fontStyle = FontStyle.Normal,
              normal = new ()
              { textColor = new (0.74f, 0.74f, 0.74f) } };
            #endregion

            wrapCenterLabelStyle           = new (GUI.skin.label);
            wrapCenterLabelStyle.wordWrap  = true;
            wrapCenterLabelStyle.alignment = TextAnchor.MiddleCenter;

            wordWrapRichTextLabelStyle          = new (GUI.skin.label);
            wordWrapRichTextLabelStyle.wordWrap = true;
            wordWrapRichTextLabelStyle.richText = true;

            btImgStyle                   = new (GUI.skin.button);
            btImgStyle.normal.background = null;
            btImgStyle.imagePosition     = ImagePosition.ImageOnly;
            btImgStyle.padding           = new (0, 0, 0, 0);
            btImgStyle.fixedHeight       = 35;

            boldLabelStyle           = new (GUI.skin.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;

            setupLabelStyle           = new (boldLabelStyle);
            setupLabelStyle.alignment = TextAnchor.MiddleCenter;

            setupWindowStyle = new()
            { richText  = true,
              alignment = TextAnchor.MiddleLeft,
              fontSize  = 20,
              fontStyle = FontStyle.Bold,
              normal = new ()
              { textColor = new (0.86f, 0.86f, 0.86f) } };

            setupWindowHeaderStyle = new()
            { fontSize  = 20,
              fontStyle = FontStyle.Bold,
              normal = new()
              { textColor = new (1f, 0.64f, 0.54f) } };

            dropAreaStyle = new (GUI.skin.box)
            { normal =
              { textColor = new (0.87f, 0.87f, 0.87f) },
              alignment = TextAnchor.MiddleCenter,
              fontStyle = FontStyle.Bold,
              fontSize  = 12,
              richText  = true,
              wordWrap  = true };
            
            // GUIContent
            safeModeWarningContent = new
            (
            "Safe Mode",
            "If disabled, the user is not able to perform certain operations that could potentially be dangerous or cause unintended behaviour."
             );
            
            createDefaultProjectContent = new 
            (
                "Create Default Project Structure",
                "Creates a project structure with the recommended default folders such as Art, Scripts, etc."
            );
            
            configureImagesContent = new
            (
            "Image Settings Converter",
            "Configures the default settings for images. \n" + 
                "This is useful for when you want to import images with the same settings every time."
            );
            
            createSubfolderContent = new // Deprecated. Kept for reference.
                (
                "Create Subfolder", 
                "Creates a subfolder in the selected folder."
                );

            enterPlaymodeOptionsContent = new
            (
            "Enter Playmode Options", 
            "Enabling \"Enter Playmode Options\" improves Unity's workflow by significantly reducing the time it takes to enter play mode."
            );

            resetButtonContent = new ("Reset", "Resets the settings to their default values.");
            
            // Setup Panel

            openDocumentationContent = new ("Documentation");

            openChangeLogContent = new ("Changelog");
            
            checkForUpdatesContent = new
            (
            "Check for Updates",
            "Checks if a new version of Lumina's Essentials is available. \n(Requires internet connection)"
            );
            
            openKnownIssuesContent = new
            (
            "Known Issues",
            "Opens the known issues page on the GitHub repository. \nPlease add any issues you encounter to the list."
            );
        }

        #region Deprecated
        //[Obsolete] // Deprecated. Kept for reference.
        // internal static void UpdateModulePackages()
        // {
        //     // Get the selected asset or directory path
        //     string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        //
        //     // Validation
        //     if (string.IsNullOrEmpty(selectedPath) || (!Directory.Exists(selectedPath) && !File.Exists(selectedPath)))
        //     {
        //         EditorUtility.DisplayDialog("Error", "Please, select a folder or asset to export.", "Ok");
        //         return;
        //     }
        //
        //     string luminaOriginalPath = "Assets/Header (GitHub front page)/Lumina's Essentials";
        //     string luminaTempPath     = "Assets/Lumina's Essentials";
        //     string tempSelectedPath   = selectedPath.Replace(luminaOriginalPath, luminaTempPath);
        //
        //     bool isDirectory = Directory.Exists(selectedPath);
        //     bool isFile      = File.Exists(selectedPath);
        //
        //     // Move Lumina's Essentials folder to Assets
        //     AssetDatabase.MoveAsset(luminaOriginalPath, luminaTempPath);
        //     AssetDatabase.Refresh();
        //
        //     // Export the selected object (folder or asset)
        //     string packageName = EditorUtility.SaveFilePanel("Save as Unity Package", "", Path.GetFileNameWithoutExtension(selectedPath), "unitypackage");
        //
        //     if (!string.IsNullOrEmpty(packageName))
        //     {
        //         AssetDatabase.ExportPackage(tempSelectedPath, packageName, ExportPackageOptions.Recurse);
        //         AssetDatabase.Refresh();
        //     }
        //
        //     // Move Lumina's Essentials folder back to original place
        //     AssetDatabase.MoveAsset(luminaTempPath, luminaOriginalPath);
        //     AssetDatabase.Refresh();
        // }

        //[Obsolete] // Deprecated. Kept for reference.
        // internal static void ReplaceOldPackage()
        // {
        //     string essentialsFolderInAssets = "Assets/_EXAMPLE_FOLDER";
        //     
        //     if (AssetDatabase.IsValidFolder(essentialsFolderInAssets)) { AssetDatabase.DeleteAsset(essentialsFolderInAssets); }
        //     else
        //     {
        //         EssentialsDebugger.LogError("Essentials folder not found at: " + essentialsFolderInAssets);
        //         return; 
        //     }
        // }
        #endregion

        #region Check for Folder/File
        static bool IsFolderInProject(string baseDirectory, string targetFolderName)
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

        static bool IsFileInProject(string baseDirectory, string targetFileName)
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

        #region Checks for Modules
        internal static void CheckForInstalledModules()
        {
            string projectDirectory = Application.dataPath;

            if (CheckFullPackageInstalled())
            {
                // Set all keys in InstalledModules to true
                foreach (var module in UtilityWindow.InstalledModules.Keys.ToList())
                {
                    UtilityWindow.InstalledModules[module] = true;

                    if (VersionManager.DebugVersion)
                    { // Don't print the "Full Package" debug as it will always be false by this implementation.
                        if (module != "Full Package") Debug.Log($"Item '{module}' exists in the project.");
                    }
                }

                // No need to continue if the full package is installed
                return;
            }

            foreach (var item in UtilityWindow.InstalledModules.Keys)
            {
                string itemFile = item + ".cs";

                if (IsFolderInProject(projectDirectory, item) || IsFileInProject(projectDirectory, itemFile))
                {
                    if (VersionManager.DebugVersion)
                    {
                        Debug.Log($"Item '{item}' exists in the project.");
                    }
                    UtilityWindow.InstalledModules[item] = true;
                }
                else
                {
                    if (VersionManager.DebugVersion)
                    {
                        Debug.Log($"Item '{item}' does not exist in the project.");
                    }
                    UtilityWindow.InstalledModules[item] = false;
                }
            }
        }

        static bool CheckFullPackageInstalled()
        {
            string mainDirectory   = Path.Combine("Lumina's Essentials", "Modules");
            string targetDirectory = "Examples";

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
                        if (VersionManager.DebugVersion) { Debug.Log("Full package is installed."); }
                        return true;
                    }
                }
            }

            if (VersionManager.DebugVersion) { Debug.Log("Full package is not installed."); }
            return false;
        }

        internal static void InstallModules()
        {
            const string targetDirectory = "Lumina's Essentials/Editor/Packages";
            string       relativePath    = GetRelativePath(targetDirectory);

            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.LogWarning(targetDirectory + " not found.");
                return;
            }

            SetupAssetDatabaseCallbacks();

            foreach (KeyValuePair<string, bool> module in UtilityWindow.InstalledModules)
            {
                if (module.Value && UtilityWindow.SelectedModules.ContainsKey(module.Key))
                {
                    bool reInstall = ModuleInstallConfirmation(module.Key);

                    if (!reInstall) continue;

                    ImportModulePackage(relativePath, module.Key);
                }
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

        static void SetupAssetDatabaseCallbacks()
        {
            AssetDatabase.importPackageCompleted += packageName => { Debug.Log("Imported: " + packageName); };
            AssetDatabase.importPackageFailed += (packageName, errorMessage) => { Debug.LogError("Failed to import: " + packageName + "\n" + errorMessage); };
            AssetDatabase.importPackageCancelled += packageName => { Debug.Log("Cancelled importing: " + packageName); };
        }

        internal static void ClearSelectedModules()
        {
            foreach (var module in UtilityWindow.SelectedModules.ToList()) { UtilityWindow.SelectedModules[module.Key] = false; }
        }
        // -- End of Module Checks --
        #endregion
        
        // ReSharper disable Unity.PerformanceAnalysis
        internal static void CreateDirectories(string root, params string[] directories)
        {
            var fullpath = Path.Combine(Application.dataPath, root);

            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
                EssentialsDebugger.Log("Successfully created directory: " + fullpath);
            }

            foreach (var newDirectory in directories)
            {
                var newFullPath = Path.Combine(fullpath, newDirectory);

                if (!Directory.Exists(newFullPath))
                {
                    Directory.CreateDirectory(newFullPath);
                    EssentialsDebugger.Log("Successfully created directory: " + newFullPath);
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
        
        internal static void ShowAllEditorPrefs() => EditorPrefsWindow.ShowWindow();
    }
}
