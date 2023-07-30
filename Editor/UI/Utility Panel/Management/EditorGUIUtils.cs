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
            buttonStyle.padding = new (0, 0, 10, 10);
            buttonSetup          = new (buttonStyle);
            buttonSetup.padding  = new (10, 10, 6, 6);
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

        internal static bool IsPlaymodeActive()
        { // If the user is in play mode, display a message telling them that the utility panel is not available while in play mode.
            if (!EditorApplication.isPlaying) return false;
            
            GUILayout.Space(40);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("The Utility Panel \nis disabled while in play mode.", wrapCenterLabelStyle, GUILayout.ExpandWidth(true));
            }
            GUILayout.Space(40);
            return true;
        }
        
        internal static void ShowAllEditorPrefs() => EditorPrefsWindow.ShowWindow();
    }
}