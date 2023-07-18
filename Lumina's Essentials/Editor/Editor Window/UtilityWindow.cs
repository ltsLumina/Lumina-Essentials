#region
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using static Lumina.Essentials.Editor.EditorGUIUtils;
using static Lumina.Essentials.Editor.VersionManager;
#endregion

namespace Lumina.Essentials.Editor //TODO: Make the installer a git UPM package
{
    public sealed class UtilityWindow : EditorWindow
    {
        readonly static Vector2 winSize = new (370, 650);
        readonly static float buttonSize = winSize.x * 0.5f - 6;
        
        #region Panels
        /// <summary> The panel that will be displayed. </summary>
        int selectedTab;
        /// <summary> The labels of the tabs. </summary>
        readonly string[] tabLabels = { "Setup", "Settings", "Utilities" };
        /// <summary> Used to invoke the setup panel when necessary. (Not the main panel) </summary>
        Action currentPanel;
        #endregion
        
        #region Modules
        /// <summary> Toggle to enable or disable the installation of Lumina's Essentials. </summary>
        bool installEssentials;
        /// <summary> Toggle to enable or disable the installation of Alex' Essentials. (Half the package) </summary>
        bool alexEssentials;
        /// <summary> Toggle to enable or disable the installation of Joel's Essentials. (Half the package) </summary>
        bool joelEssentials;
        /// <summary> Toggle to enable or disable the installation of DOTween. </summary>
        bool installDOTween;
        /// <summary> Toggle to enable or disable the installation of the Attributes module of Lumina's Essentials. </summary>
        bool attributes;
        /// <summary> Toggle to enable or disable the installation of the Sequencer module of Lumina's Essentials. </summary>
        bool sequencer;
        /// <summary> Toggle to enable or disable the installation of the Helpers module of Lumina's Essentials. </summary>
        bool helpers;
        /// <summary> Toggle to enable or disable the installation of the Editor module of Lumina's Essentials. </summary>
        bool shortcuts;
        /// <summary> Toggle to enable or disable the installation of the Misc module of Lumina's Essentials. </summary>
        bool misc;
        #endregion
        
        #region Settings variables
        // The variables to be shown under the settings tab.
        
        /// <summary> Whether or not the user has to set up Lumina's Essentials to the latest version. </summary>
        public static bool SetupRequired { get; set; }
        /// <summary> Used to ensure that the user is aware of the consequences of disabling Safe mode. </summary>
        bool iUnderstand;
        
        

        // Safe mode bool (used to prevent the user from accidentally deleting their files).
        bool safeMode;
        
        /// <summary> Enabled by default. Prevents the user from accidentally doing something they didn't intend to do. </summary>
        bool SafeMode
        {
            get => safeMode;
            set
            {
                safeMode = value;
                EditorPrefs.SetBool("SafeMode", safeMode);

                // Ensures that the user is aware of the consequences of replacing files by displaying a warning message if the checkbox is checked
                if (iUnderstand && !safeMode)
                {
                    DebugHelper.LogWarning($"Safe mode is now {(safeMode ? "enabled" : "disabled")}.");
                }
            }
        }

        // End of preferences variables //
        #endregion

        #region Utilities variables

        bool configureImages;
        SpriteImportMode spriteImportMode = SpriteImportMode.Single;
        FilterMode filterMode = FilterMode.Point;
        TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
        /// <summary> Quick toggle to set the sprite import mode to the recommended settings for importing sprites. </summary>
        bool isSprite;
        
        // End of utilities variables //
        #endregion

        [MenuItem("Tools/Lumina/Open Utility Panel")]
        public static void OpenUtilityWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = (UtilityWindow) GetWindow(typeof(UtilityWindow), true, "Lumina's Essentials Utility Panel");
            window.minSize = winSize;
            window.maxSize = window.minSize;
            window.Show();
        }

        void OnEnable()
        { // Repaint the window every frame
            
            // Set the last open version to the current version
            LastOpenVersion = CurrentVersion;
            SafeMode      = true;

            // Enable the Toolbar.
            currentPanel = DisplayToolbar;
            
            // Warns the user on opening the Utility Panel if they are using an outdated version of Lumina's Essentials.
            if (!CompareVersions(CurrentVersion, LatestVersion))
            {
                DebugHelper.LogWarning(
                    $"You are using an outdated version of Lumina's Essentials. The latest version is {LatestVersion} and you are using {CurrentVersion}." +
                    "\n Please update to the latest version to ensure that you have the latest features and bug fixes.");
            }

            const string headerGUID = "7a1204763dac9b142b9cd974c88fdc8d";
            const string footerGUID = "22cbfe0e1e5aa9a46a9bd08709fdcac6";
            string       headerPath = AssetDatabase.GUIDToAssetPath(headerGUID);
            string       footerPath = AssetDatabase.GUIDToAssetPath(footerGUID);

            if (headerPath != null && footerPath != null)
            {
                headerImg = AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);
                footerImg = AssetDatabase.LoadAssetAtPath<Texture2D>(footerPath);
            }

            Repaint();
        }

        /// <summary>
        ///     Displays the toolbar at the top of the window that toggles between the two panels.
        /// </summary>
        void OnGUI()
        {
            SetGUIStyles();
            
            // If the user is in play mode, display a message telling them that the utility panel is not available while in play mode.
            if (EditorApplication.isPlaying)
            { 
                GUILayout.Space(40);
                GUILayout.BeginHorizontal();
                GUILayout.Label("The Utility Panel \nis disabled while in play mode.", wrapCenterLabelStyle, GUILayout.ExpandWidth(true)); //TODO using the wrapCenterLabelStyle from DOTween
                GUILayout.Space(40);
                GUILayout.EndHorizontal();
                return;
            }
            
            // Don't show the toolbar if the user in the setup panel
            currentPanel();
        }

        void DisplayToolbar()
        {
            var areaRect = new Rect(0, 0, 370, 30);
            selectedTab = GUI.Toolbar(areaRect, selectedTab, tabLabels);
            
            GUILayout.Space(30);

            switch (selectedTab)
            {
                case 1:
                    SettingsPanel();
                    break;

                case 2:
                    UtilitiesPanel();
                    break;

                default:
                    MainPanel();
                    break;
            }
        }

        void MainPanel()
        {
            var areaRect = new Rect(0, 30, 370, 118);
            GUI.DrawTexture(areaRect, headerImg, ScaleMode.StretchToFill, false);
            GUILayout.Space(areaRect.y + 90);
            
            #region Main Labels (Version, Update Check, etc.)
            // Display current version in bold
            GUILayout.Label($"  Essentials Version: {CurrentVersion}", mainLabelStyle);

            // Display the latest version in bold
            GUILayout.Label($"  Latest Version: {LatestVersion}", mainLabelStyle);

            // Display the time since the last update check
            GUILayout.Label($"  Last Update Check: {EssentialsUpdater.LastUpdateCheck}", mainLabelStyle);
            
            // End of Main Labels
            #endregion

            #region Setup Lumina Essentials Button
            GUILayout.Space(3);
            if (SetupRequired)
            {
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("SETUP REQUIRED", setupLabelStyle);
                GUILayout.EndVertical();
                GUI.backgroundColor = Color.white;
            }
            // ReSharper disable once EnforceIfStatementBraces
            else GUILayout.Space(8);

            GUI.color = Color.green;

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("<b>Setup Essentials...</b>\n(add/remove Modules)", buttonSetup, GUILayout.Width(200)))
                {
                    // Select Setup Panel (not main panel)
                    currentPanel = SetupPanel;
                }
                GUILayout.FlexibleSpace();
            }

            GUI.color = new Color(0.89f, 0.87f, 0.87f);

            GUI.backgroundColor = Color.white;
            GUILayout.Space(4);
            // End of Setup Lumina Essentials Button
            #endregion
            GUILayout.Space(3);

            #region Text Box (Description)
            
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUI.color = new Color(1f, 0.75f, 0.55f);
                    if (GUILayout.Button("Something!", buttonSetup, GUILayout.Width(200)))
                    {
                        Debug.Log("testing");
                    }
                    
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                }
                
                GUILayout.Label //TODO: I can make this remind the user of the utilities panel.
                ("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi ultricies mauris nec posuere hendrerit." +
                 " In consequat lacus erat, eu feugiat mauris consequat condimentum." +
                 " Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae;" +
                 " Sed pulvinar nisl condimentum felis hendrerit facilisis.", wordWrapRichTextLabelStyle);
            }
            #endregion
            GUILayout.Space(3);

            #region Grid of Buttons (Open Documentation, Open Changelog, etc.)
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button
                    ("Open Documentation", GUILayout.Width(buttonSize), GUILayout.Height(40)))
                    Application.OpenURL("https://github.com/ltsLumina/Unity-Essentials");

                if (GUILayout.Button
                    ("Open Changelog", GUILayout.Width(buttonSize), GUILayout.Height(40)))
                    Application.OpenURL("https://github.com/ltsLumina/Unity-Essentials/releases/latest");
            }
            
            using (new GUILayout.HorizontalScope())
            {
                // Display the button to check for updates
                if (GUILayout.Button("Check for Updates", GUILayout.Width(buttonSize), GUILayout.Height(40)))
                {
                    EssentialsUpdater.CheckForUpdates();

                    // if there is a new version available, open the GitHub repository's releases page
                    if (!EditorPrefs.GetBool("UpToDate"))
                    {
                        //DebugHelper.LogWarning("There is a new version available!"); //TODO: look at this
                        Application.OpenURL("https://github.com/ltsLumina/Unity-Essentials/releases/latest");
                    }
                }

                if (GUILayout.Button
                    ("Install DOTween", GUILayout.Width(buttonSize), GUILayout.Height(40))) Application.OpenURL("https://dotween.demigiant.com/download.php");
                
            }
            #endregion
            GUILayout.Space(4);
            
            // Footer/Developed by Lumina
            if (GUILayout.Button(footerImg, btImgStyle)) Application.OpenURL("https://github.com/ltsLumina/");
        }

        /// <summary>
        ///    Displays the setup panel that allows the user to replace the old files with the new ones.
        /// </summary>
        void SetupPanel() // Not to be confused with the SetupWindow
        {
            GUILayout.Label("   Add/Remove Modules", mainLabelStyle);
            
            GUILayout.Space(10);
            
            #region Checkboxes
            installEssentials = EditorGUILayout.Toggle("Full Package", installEssentials);
            installDOTween    = EditorGUILayout.Toggle("DOTween", installDOTween);
            
            // End of Checkboxes
            #endregion
            
            #region Apply/Cancel Buttons (For the Setup Panel)
            GUILayout.BeginHorizontal();
            // "Apply" button
            if (GUILayout.Button("Apply"))
            {
                if (installEssentials && !SafeMode)
                {
                    // Popup to confirm the replacement of the old files
                    if (EditorUtility.DisplayDialog
                    ("Confirmation",
                     "Are you sure you want to replace the old files? \n " + "Please backup any files from the old version that you may want to keep." +
                     "It is recommended to backup Systems.prefab in the Resources folder", "Apply", "Cancel"))
                    {
                        // Delete the old files and replace them with the new ones
                        ReplaceOldPackage();
                    }
                    else
                    {
                        DebugHelper.LogWarning("The files were not replaced. (Safe mode is on)");
                    }
                    
                    // Delete the old files and replace them with the new ones
                    ReplaceOldPackage();
                }
            }
            
            // "Cancel" button
            if (GUILayout.Button("Cancel"))
            {
                // Close the setup panel
                currentPanel = DisplayToolbar;
                
                // Reset the checkboxes
                installEssentials = false;
                iUnderstand       = false;
                SafeMode        = true;
            }
            
            GUILayout.EndHorizontal();
            // End of Apply/Cancel Buttons
            #endregion
            
            #region Help box and Confirmation (Safe Mode)
            EditorGUILayout.HelpBox
            ("This will attempt to delete your previous install of Lumina's Essentials as to properly install the new version. " +
             "\n Please backup any files from the old version that you may want to keep such as 'Systems.prefab' if you are using it. ", MessageType.Warning);
            
            // checkbox to confirm
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("I understand the consequences.");
            iUnderstand = EditorGUILayout.Toggle(iUnderstand);

            if (iUnderstand) SafeMode = false;

            EditorGUILayout.EndHorizontal();
            // End of Help box and Confirmation
            #endregion
        }
        
        void SettingsPanel()
        {
            GUILayout.Space(6);
            GUILayout.Label("Settings", centerLabelStyle);
            GUILayout.Label("Various settings for the Lumina Essentials package.", subLabelStyle);

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            #region Reset Button (Resets the EditorPrefs)
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(35), GUILayout.ExpandWidth(true)))
                if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to reset?", "Yes", "No"))
                {
                    if (!SafeMode)
                    {
                        // Reset the EditorPrefs
                        EditorPrefs.DeleteAll();
                        DebugHelper.LogWarning("Settings and EditorPrefs have been reset.");
                        EssentialsUpdater.CheckForUpdates();

                        Close();
                        SetupWindow.OpenSetupWindow();
                    }
                    else { DebugHelper.LogAbort(SafeMode); }
                }

            GUILayout.EndHorizontal();
            // End of Reset Button
            #endregion
            
            GUILayout.Space(10);

            #region Settings
            // Display the value of SafeMode as a readonly checkbox and tooltip
            SafeMode = EditorGUILayout.Toggle 
            (new GUIContent
             ("Safe Mode","If disabled, the user is not protected from performing unsafe actions that may perform unintended actions."                             +
              " \n Namely, this is used to ensure the user doesn't accidentally delete their old Essentials install when upgrading to a new version."), SafeMode);
            
            // Displays the last version of the Essentials package that was opened
            EditorGUILayout.LabelField("Last Open Version", EditorPrefs.GetString("LastOpenVersion"));
            
            // Displays whether the user is up to date or not
            EditorGUILayout.LabelField("Up To Date", EditorPrefs.GetBool("UpToDate").ToString());
            
            // Displays if this is a debug build or not
            EditorGUILayout.LabelField("Debug Version", EditorPrefs.GetBool("DebugVersion").ToString());

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            
            if (!SafeMode)
            {
                // Display the image configuration options
                GUILayout.Label("Read-only values", centerLabelStyle);
                GUILayout.Label("These are only here for debugging purposes.", subLabelStyle);
                GUILayout.Space(5);
                
                // Normal fields that the user may edit.
                // Example
                
                // Group of view only fields
                EditorGUI.BeginDisabledGroup(true);
                // Box view the SetupRequired bool
                SetupRequired = EditorGUILayout.Toggle("Setup Required", SetupRequired);
                
                
                EditorGUI.EndDisabledGroup();
            }
            
            // End of Settings
            #endregion
        }

        void UtilitiesPanel()
        {
            GUILayout.Space(6);
            GUILayout.Label("Utilities", centerLabelStyle);
            GUILayout.Label("Provides useful features to improve your workflow.", subLabelStyle);

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            #region Utilities Panel
            // Checkbox to enable or disable the enter playmode options
            EditorSettings.enterPlayModeOptionsEnabled = EditorGUILayout.Toggle
            (new GUIContent
            ("Enter Playmode Options",
            "Enabling \"Enter Playmode Options\" improves Unity's workflow by significantly reducing the time it takes to enter play mode."),
             EditorSettings.enterPlayModeOptionsEnabled);
            
            
            //TODO: more options for cleaner look. even if they are pointless

            GUILayout.Space(10);
            
            // Button that creates a default project directory structure
            if (GUILayout.Button("Create Default Project \nDirectory Structure", GUILayout.Height(35)))
            {
                CreateProjectStructure();
            }

            GUI.backgroundColor = configureImages ? new Color(0.76f, 0.76f, 0.76f) : Color.white;
            if (GUILayout.Button(configureImagesContent, GUILayout.Height(35)))
            {
                configureImages = !configureImages;
            }
            GUI.backgroundColor = Color.white;
            
            if (GUILayout.Button("Placeholder Button", GUILayout.Height(35)))
            {
                DebugHelper.Log("This does nothing as it's a placeholder.");
                //TODO: if this shows new things like configuring images, make sure to disable configureImages before continuing
            }

            if (configureImages)
            {
                // Draw a horizontal line (separator)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                
                // Display the image configuration options
                GUILayout.Label("Image Configuration", centerLabelStyle);
                GUILayout.Label("Configure the default settings for images.", subLabelStyle);
                GUILayout.Space(5);

                // Enum popups for the image configuration
                spriteImportMode = (SpriteImportMode) EditorGUILayout.EnumPopup("Sprite Mode", spriteImportMode);
                filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", filterMode);
                compression = (TextureImporterCompression) EditorGUILayout.EnumPopup("Compression", compression);

                // Quick toggle to set the recommended settings for sprites (multiple)
                GUIContent spriteModeContent = new GUIContent
                ("Is Sprite", "Sets the recommended settings for sprites. \n" +
                "This will set the sprite mode to multiple, filter mode to point, and compression to uncompressed.");
                
                isSprite = EditorGUILayout.Toggle(spriteModeContent, isSprite);

                if (isSprite)
                {
                    filterMode = FilterMode.Point;
                    compression = TextureImporterCompression.Uncompressed;
                    spriteImportMode = SpriteImportMode.Multiple;
                }

                // Apply button
                if (GUILayout.Button("Apply")) ConfigureImages();
            }
            
            
            #endregion
            GUILayout.Space(10);
        }

        void CreateProjectStructure()
        {
            if (!SafeMode)
            {
                // Confirmation pop-up
                if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to create the default project structure?", "Yes", "No"))
                {
                    // Create the default folders in the root of the project 
                    CreateDirectories("_Project", "Scripts", "Art", "Audio", "Scenes", "PREFABS", "Materials", "Plugins"); // "DEL" to put it at the bottom.
                    CreateDirectories("Art" ,"Animations");
                    CreateDirectories("Audio", "SFX", "Music");
                    AssetDatabase.Refresh();
                }
            }
            else { DebugHelper.LogAbort(SafeMode); }
        }

        void ConfigureImages()
        {
            const string imageMsg  = "Are you sure you want to configure the images?";
            const string spriteMsg = "Are you sure you want to configure the sprites?";
            string confirmationMessage = isSprite 
                ? imageMsg + "\nThis will set the sprite mode to multiple, filter mode to point, and compression to uncompressed." 
                : spriteMsg + "\nThis will set the filter mode to point and compression to uncompressed."
                + " Keep in mind that the 'multiple' sprite mode is used for sprite-sheets and is not recommended for single sprites.";
            
            if (!SafeMode)
            {
                if (EditorUtility.DisplayDialog("Confirmation", confirmationMessage, "Yes", "No"))
                {
                    // Get all files in /Lumina Essentials/Editor/Utilities/Image Converter/
                    string relativePath = "Lumina's Essentials/Editor/Utilities/Image Converter";
                    string fullPath     = Path.Combine(Application.dataPath, relativePath);

                    // Get the search patterns for the files (png, jpg, jpeg)
                    string[] searchPatterns ={ "*.png", "*.jpg", "*.jpeg" };

                    string[] files = searchPatterns.SelectMany(pattern => Directory.GetFiles(fullPath, pattern)).ToArray();

                    // Loop through each file
                    foreach (string file in files)
                    {
                        // Get the asset path and texture importer
                        string assetPath       = "Assets/" + Path.Combine(relativePath, Path.GetFileName(file)).Replace('\\', '/');
                        var    textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                        if (textureImporter != null)
                        {
                            // Set the filter mode and compression to point and no compression at the path
                            textureImporter.spriteImportMode   = spriteImportMode;
                            textureImporter.filterMode         = filterMode;
                            textureImporter.textureCompression = compression;
                            textureImporter.SaveAndReimport();
                        }
                    }
                }
                else { DebugHelper.LogAbort(SafeMode); }
            }
            else { DebugHelper.LogAbort(SafeMode); }
        }
    }
}
