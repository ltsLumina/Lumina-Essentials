#region
using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UnityEngine;
using UnityEditor;
using static Lumina.Essentials.Editor.UI.EditorGUIUtils;
using static Lumina.Essentials.Editor.UI.VersionManager;
#endregion

namespace Lumina.Essentials.Editor.UI //TODO: Make the installer a git UPM package
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
        
        /// <summary> Enabled by default. Prevents the user from accidentally doing something they didn't intend to do. </summary>
        bool SafeMode
        {
            get => EditorPrefs.GetBool("SafeMode", true);
            set
            {
                EditorPrefs.SetBool("SafeMode", value);

                // Ensures that the user is aware of the consequences of replacing files by displaying a warning message if the checkbox is checked
                if (iUnderstand && !SafeMode)
                {
                    DebugHelper.LogWarning($"Safe mode is now {(SafeMode ? "enabled" : "disabled")}.");
                }
            }
        }

        // End of preferences variables //
        #endregion

        #region Utilities variables

        /// <summary> Opens the configure images options. </summary>
        bool configuringImages;
        static SpriteImportMode spriteImportMode = SpriteImportMode.Single;
        static FilterMode filterMode = FilterMode.Point;
        static TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
        /// <summary> Quick toggle to set the sprite import mode to the recommended settings for importing sprites. </summary>
        bool isSpriteSheet;
        bool dragAndDropFolder;
        bool isCorrectDirectory;
        /// <summary> Shows the path of the selected object. </summary>
        string imageConverterPath = "";
        
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
            SafeMode = true;

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
                    DrawSettingsGUI();
                    break;

                case 2:
                    DrawUtilitiesGUI();
                    break;

                default:
                    DrawSetupGUI();
                    break;
            }
        }

        void DrawSetupGUI()
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
            GUILayout.Space(3);

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
                    currentPanel = DrawModuleGUI;
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
        void DrawModuleGUI() // Not to be confused with the SetupWindow
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
        
        void DrawSettingsGUI()
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
                        
                        // Reset any necessary flags or variables
                        SafeMode                   = true;
                        iUnderstand                = false;
                        imageConverterPath         = "";
                        DontShow_DebugBuildWarning = false;
                        DebugHelper.LogBehaviour   = DebugHelper.LogLevel.Verbose;
                        
                        DebugHelper.LogWarning("Settings and EditorPrefs have been reset.");

                        // Display a warning if the user is in a debug build
                        StartupChecks.DebugBuildWarning();
                        
                        // Check for updates to set up everything again.
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
             ("Safe Mode","If disabled, the user is not protected from performing unsafe actions that may perform unintended actions." +
              " \n Namely, this is used to ensure the user doesn't accidentally delete their old Essentials install when upgrading to a new version."), SafeMode);
            
            // The user can choose the logging level. This is used to determine what is logged to the console.
            if (!SafeMode)
                DebugHelper.LogBehaviour = (DebugHelper.LogLevel) EditorGUILayout.EnumPopup
                    (new GUIContent("└  Logging Behaviour", "Determines what (how much) is logged to the console."), DebugHelper.LogBehaviour);

            GUILayout.Space(3);
            
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
                GUILayout.Label("Debug Options", centerLabelStyle);
                GUILayout.Label("Various buttons and settings for debugging.", subLabelStyle);
                GUILayout.Space(5);

                if (GUILayout.Button("Show All EditorPrefs", GUILayout.Height(30)))
                {
                    ShowAllEditorPrefs();
                }
                
                // Draw a horizontal line (separator)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                
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
                DontShow_DebugBuildWarning = EditorGUILayout.Toggle("Don't Show Debug Alert", DontShow_DebugBuildWarning);

                EditorGUI.EndDisabledGroup();
            }
            
            // End of Settings
            #endregion
        }

        void DrawUtilitiesGUI()
        {
            GUILayout.Space(6);
            GUILayout.Label("Utilities", centerLabelStyle);
            GUILayout.Label("Provides useful features to improve your workflow.", subLabelStyle);

            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            #region Utilities Panel

            #region Buttons and Toggles
            // Checkbox to enable or disable the enter playmode options
            EditorSettings.enterPlayModeOptionsEnabled = EditorGUILayout.Toggle
            (new GUIContent
             ("Enter Playmode Options",
              "Enabling \"Enter Playmode Options\" improves Unity's workflow by significantly reducing the time it takes to enter play mode."),
             EditorSettings.enterPlayModeOptionsEnabled);
            
            //TODO: option for what input system to use (advanced)
            
            
            
            //TODO: more options for cleaner look. even if they are pointless

            GUILayout.Space(10);
            
            // Button that creates a default project directory structure //TODO: allow for selecting a custom directory structure through enum popups
            if (GUILayout.Button("Create Default Project \nDirectory Structure", GUILayout.Height(35))) CreateProjectStructure();

            GUILayout.Space(3);
            
            GUI.backgroundColor = configuringImages ? new Color(0.76f, 0.76f, 0.76f) : Color.white;
            if (GUILayout.Button(configureImagesContent, GUILayout.Height(35)))
            {
                configuringImages = !configuringImages;

                // Reset the image converter path if the user stops configuring the configure images settings.
                if (!configuringImages) imageConverterPath = "";
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(3);
            
            if (GUILayout.Button("Placeholder Button", GUILayout.Height(35)))
            {
                DebugHelper.Log("This does nothing as it's a placeholder.");
                //TODO: if this shows new things like configuring images, make sure to disable configuringImages before continuing
            }

            #endregion
            
            GUILayout.Space(3);
            
            // Draw a horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #region Image Converter
            // Display the image configuration options //TODO: add an advanced button to show more options (allows for a default folder that automatically configures images) //Also add a button to reset the image converter directory.
            GUILayout.Label("Image Configuration", centerLabelStyle); 
            GUILayout.Label("Configure the default settings for images.", subLabelStyle);
            GUILayout.Space(10);

            // Enum popups for the image configuration
            spriteImportMode = (SpriteImportMode) EditorGUILayout.EnumPopup("Sprite Mode", spriteImportMode);
            filterMode       = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", filterMode);
            compression      = (TextureImporterCompression) EditorGUILayout.EnumPopup("Compression", compression);

            // Quick toggle to set the recommended settings for sprites (multiple)
            var spriteModeContent = new GUIContent
            ("Sprite-Sheet",
             "Sets the recommended settings for sprite-sheets \n" + "This will set the sprite mode to multiple, filter mode to point, and compression to uncompressed.");

            isSpriteSheet = EditorGUILayout.Toggle(spriteModeContent, isSpriteSheet);

            if (isSpriteSheet)
            {
                filterMode       = FilterMode.Point;
                compression      = TextureImporterCompression.Uncompressed;
                spriteImportMode = SpriteImportMode.Multiple;
            }
            
            GUILayout.Space(5);

            DrawConfigureImagesGUI();
            
            #endregion
            
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

        void DrawConfigureImagesGUI()
        {
            if (configuringImages) DrawDragAndDropConfig();
        }
        
        void DrawDragAndDropConfig()
        {
            GUILayout.Label("Drag a folder here:", middleStyle);
            GUILayout.Label("The selected folder will be used to convert the images.", subLabelStyle);
            GUILayout.Space(10);

            var dropAreaStyle = new GUIStyle(GUI.skin.box)
            { normal =
              { textColor = new (0.87f, 0.87f, 0.87f) },
              alignment = TextAnchor.MiddleCenter,
              fontStyle = FontStyle.Bold,
              fontSize  = 12,
              richText  = true,
              wordWrap  = true };
            
            float dropAreaHeight = imageConverterPath.Length > 60 ? 45 : 30;
            
            Rect dropArea = GUILayoutUtility.GetRect(0, dropAreaHeight, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, imageConverterPath, dropAreaStyle);

            if (Event.current.type == EventType.DragUpdated && dropArea.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }

            if (Event.current.type == EventType.DragPerform && dropArea.Contains(Event.current.mousePosition))
            {
                DragAndDrop.AcceptDrag();

                foreach (var path in DragAndDrop.paths)
                {
                    // check if the path is directory (folder)
                    if (Directory.Exists(path))
                    {
                        imageConverterPath = path;
                        Event.current.Use();
                        break;
                    }
                }
            }

            GUILayout.Space(10);

            string       folderSelectedMsg   = $"The folder: \"{GetFolderNameFromString(imageConverterPath)}\" will be used to convert all images.";
            const string noFolderSelectedMsg = "No folder selected. \nPlease drag and drop a folder to use.";

            if (!string.IsNullOrEmpty(imageConverterPath))
            {
                GUILayout.Label(folderSelectedMsg, middleStyle);
                GUILayout.Label("Please put any images you wish to convert in said folder.", subLabelStyle);
                GUILayout.Space(4);

                var correctDirectoryContent = new GUIContent("Is this directory correct?", "This will disable safe mode. \nPlease proceed with caution.");
                isCorrectDirectory = EditorGUILayout.Toggle(correctDirectoryContent, isCorrectDirectory);

                if (isCorrectDirectory)
                {
                    SafeMode = !SafeMode;
                    DebugHelper.LogWarning("Safe mode disabled.");
                }

                GUILayout.Space(5);
                
                if (GUILayout.Button("Apply"))
                {
                    if (isCorrectDirectory) ConfigureImages();
                    else DebugHelper.LogWarning("The action was aborted. \nYou haven't checked the confirmation box!");
                }
            }
            else
            {
                GUILayout.Label(noFolderSelectedMsg, middleStyle);
            }
        }

        void ConfigureImages()
        {
            const string imageMsg  = "Are you sure you want to configure the images?";
            const string spriteMsg = "Are you sure you want to configure the sprites?";
            string confirmationMessage = isSpriteSheet
                ? $"{imageMsg}\nThis will set the sprite mode to multiple, filter mode to point, and compression to uncompressed."
                : $"{spriteMsg}\nThis will set the filter mode to point and compression to uncompressed." +
                  " Keep in mind that the 'multiple' sprite mode is used for sprite-sheets and is not recommended for single sprites.";
            
            if (!SafeMode)
            {
                if (EditorUtility.DisplayDialog("Confirmation", confirmationMessage, "Yes", "No"))
                {
                    // Get all files in /Lumina Essentials/Editor/Utilities/Image Converter/
                    string folderPath = imageConverterPath;

                    // Get the search patterns for the files (png, jpg, jpeg)
                    string[] searchPatterns ={ "*.png", "*.jpg", "*.jpeg" };

                    string[] images = searchPatterns.SelectMany(pattern => Directory.GetFiles(folderPath, pattern)).ToArray();

                    // Loop through each file
                    foreach (string image in images)
                    {
                        // Get the asset path and texture importer
                        string assetPath       = Path.Combine(folderPath, Path.GetFileName(image)).Replace('\\', '/');
                        var    textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                        if (textureImporter != null)
                        {
                            // Set the filter mode and compression to point and no compression at the path
                            textureImporter.spriteImportMode   = spriteImportMode;
                            textureImporter.filterMode         = filterMode;
                            textureImporter.textureCompression = compression;
                            textureImporter.SaveAndReimport();

                            foreach (string configuredImage in images)
                            {
                                DebugHelper.Log($"Configured {Path.GetFileName(configuredImage)}");
                            }
                        }
                    }
                }
                else { DebugHelper.LogAbort(false); }
            }
            else { DebugHelper.LogAbort(SafeMode); }
        }
    }
    
    // window to display editor prefs
    public class EditorPrefsWindow : EditorWindow
    {
        [MenuItem("Lumina's Essentials/Editor Preferences")]
        public static void ShowWindow() => GetWindow<EditorPrefsWindow>("Editor Preferences");

        Vector2 scrollPos;
        
        void OnGUI()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Unity Technologies\Unity Editor 5.x\");

            if (key == null)
            {
                EditorGUILayout.LabelField("No EditorPrefs found.");
                return;
            }

            EditorGUILayout.LabelField("All EditorPrefs:");

            // Begin a scroll view. Note the use of the field to store the scroll position.
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var valueName in key.GetValueNames())
            {
                var value = key.GetValue(valueName);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(valueName);
                EditorGUILayout.LabelField(value.ToString(), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }

            // Always do a `EndScrollView` when you do a `BeginScrollView`
            EditorGUILayout.EndScrollView();
        }
    }
}
