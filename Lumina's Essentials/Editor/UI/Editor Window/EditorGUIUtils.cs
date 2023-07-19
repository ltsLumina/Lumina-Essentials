using System.IO;
using Lumina.Essentials.Sequencer;
using UnityEditor;
using UnityEngine;

namespace Lumina.Essentials.Editor.UI
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
        internal static GUIStyle toolbarButtonStyle;
        internal static GUIStyle centerLabelStyle;
        internal static GUIStyle subLabelStyle;
        internal static GUIStyle buttonBigStyle; 
        internal static GUIStyle wrapCenterLabelStyle;
        internal static GUIStyle wordWrapRichTextLabelStyle;
        internal static GUIStyle btImgStyle;
        internal static GUIStyle boldLabelStyle;
        internal static GUIStyle setupLabelStyle;
        internal static GUIStyle setupWindowStyle;

        // GUIContent
        internal static GUIContent configureImagesContent;

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
            
            // GUIContent
            configureImagesContent = new("Configure Images",
                "Configures the default settings for images. \n" + 
                "This is useful for when you want to import images with the same settings every time.");
        }

        internal static void ReplaceOldPackage()
        {
            string essentialsFolderInAssets = "Assets/EXAMPLE_FOLDER"; //TODO: Replace with the actual path
            if (AssetDatabase.IsValidFolder(essentialsFolderInAssets)) AssetDatabase.DeleteAsset(essentialsFolderInAssets);

            string newEssentialsPackagePath = "Assets/Lumina's Essentials/Editor/Package/test package.unitypackage"; //TODO: Replace with the actual path

            if (File.Exists(newEssentialsPackagePath)) AssetDatabase.ImportPackage(newEssentialsPackagePath, true);
            else DebugHelper.LogError("New Essentials Package not found at: " + newEssentialsPackagePath);

            AssetDatabase.Refresh();
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
    }
}
