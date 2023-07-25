using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lumina.Essentials.Editor.UI.Management
{
    public static class EditorGUIUtils
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

        // -- GUIContent -- //
        internal static GUIContent safeModeWarningContent;
        internal static GUIContent createDefaultProjectContent;
        internal static GUIContent configureImagesContent;
        internal static GUIContent createSubfolderContent; // Deprecated. Kept for reference.
        internal static GUIContent enterPlaymodeOptionsContent;
        

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
            "If disabled, the user is not protected from performing unsafe actions that may perform unintended actions." +
             " \n Namely, this is used to ensure the user doesn't accidentally delete their files when performing certain operations."
             );
            
            createDefaultProjectContent = new 
            (
                "Create Default Project Structure",
                "Creates a project structure with the recommended default folders such as Art, Scripts, etc."
            );
            
            configureImagesContent = new
            (
            "Configure Images",
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
        }

        [Obsolete] // Deprecated. Kept for reference.
        internal static void UpdateModulePackages()
        {
            // Get the selected asset or directory path
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            // Validation
            if (string.IsNullOrEmpty(selectedPath) || (!Directory.Exists(selectedPath) && !File.Exists(selectedPath)))
            {
                EditorUtility.DisplayDialog("Error", "Please, select a folder or asset to export.", "Ok");
                return;
            }

            string luminaOriginalPath = "Assets/Header (GitHub front page)/Lumina's Essentials";
            string luminaTempPath     = "Assets/Lumina's Essentials";
            string tempSelectedPath   = selectedPath.Replace(luminaOriginalPath, luminaTempPath);

            bool isDirectory = Directory.Exists(selectedPath);
            bool isFile      = File.Exists(selectedPath);

            // Move Lumina's Essentials folder to Assets
            AssetDatabase.MoveAsset(luminaOriginalPath, luminaTempPath);
            AssetDatabase.Refresh();

            // Export the selected object (folder or asset)
            string packageName = EditorUtility.SaveFilePanel("Save as Unity Package", "", Path.GetFileNameWithoutExtension(selectedPath), "unitypackage");

            if (!string.IsNullOrEmpty(packageName))
            {
                AssetDatabase.ExportPackage(tempSelectedPath, packageName, ExportPackageOptions.Recurse);
                AssetDatabase.Refresh();
            }

            // Move Lumina's Essentials folder back to original place
            AssetDatabase.MoveAsset(luminaTempPath, luminaOriginalPath);
            AssetDatabase.Refresh();
        }

        internal static void ReplaceOldPackage()
        {
            string essentialsFolderInAssets = "Assets/_EXAMPLE_FOLDER"; //TODO: Replace with the actual path
            
            if (AssetDatabase.IsValidFolder(essentialsFolderInAssets)) { AssetDatabase.DeleteAsset(essentialsFolderInAssets); }
            else //TODO: doesnt work. idk why
            {
                DebugHelper.LogError("Essentials folder not found at: " + essentialsFolderInAssets);
                //return; //TODO: fix warning or something
            }
        }

        internal static void InstallModules()
        {
            //string packagesPath = "Assets/Lumina's Essentials/Editor/Packages/"; Kept here for reference in case something goes wrong.
            string relativePath = "";

            const string folderNameToFind = "Packages"; // replace with the name of your folder
            const string assetsPath       = "Assets";
            string       physicalPath     = Path.Combine(Directory.GetCurrentDirectory(), assetsPath);

            string[] directories = Directory.GetDirectories(physicalPath, folderNameToFind, SearchOption.AllDirectories);

            foreach (string directory in directories)
            {
                relativePath = directory[Directory.GetCurrentDirectory().Length..].Replace('\\', '/').TrimStart('/') + "/";
                Debug.Log(relativePath);
            }

            foreach (var module in UtilityWindow.installedModules)
            {
                AssetDatabase.importPackageCompleted += packageName =>
                {
                    Debug.Log("Imported: " + packageName);
                };
                
                AssetDatabase.importPackageFailed += (packageName, errorMessage) =>
                {
                    Debug.LogError("Failed to import: " + packageName + "\n" + errorMessage);
                };
                
                AssetDatabase.importPackageCancelled += packageName =>
                {
                    Debug.Log("Cancelled importing: " + packageName);
                };
                
                // Combine the path of the module with the path of the packages folder
                string modulePath = Path.Combine(relativePath, module.Key);
                modulePath += ".unitypackage";
                Debug.Log(modulePath);

                AssetDatabase.ImportPackage(modulePath, false);
            }
        }
        
        internal static void CreateDirectories(string root, params string[] directories)
        {
            var fullpath = Path.Combine(Application.dataPath, root);

            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
                DebugHelper.Log("Successfully created directory: " + fullpath);
            }

            foreach (var newDirectory in directories)
            {
                var newFullPath = Path.Combine(fullpath, newDirectory);

                if (!Directory.Exists(newFullPath))
                {
                    Directory.CreateDirectory(newFullPath);
                    DebugHelper.Log("Successfully created directory: " + newFullPath);
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
        
        internal static void SelectAllModules()
        {
            // Get the state of "Full Package", which is always the first one
            bool isFullPackageSelected = UtilityWindow.AvailableModules.First().Value;

            // Go through each module. If "Full Package" is selected, select all modules, otherwise unselect them.
            foreach (var module in UtilityWindow.AvailableModules.Keys.ToList())
            {
                UtilityWindow.AvailableModules[module] = isFullPackageSelected;
            }
        }

        internal static void ClearSelectedModules()
        {
            foreach (var module in UtilityWindow.AvailableModules.ToList()) { UtilityWindow.AvailableModules[module.Key] = false; }
        }
    }
}
