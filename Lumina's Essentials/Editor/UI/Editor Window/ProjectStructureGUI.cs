#region
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.EditorGUIUtils;
using static Lumina.Essentials.Editor.UI.UtilityWindow;
#endregion

namespace Lumina.Essentials.Editor.UI
{
public sealed class ProjectStructureGUI : EditorWindow
{
    static bool loadDir;
    
    static Vector2 _scrollPosition;
    static Dictionary<string, bool> folderToggleStates = new ();
    static Dictionary<string, string> folderChanges = new ();

    static string selectedFolder;

    internal static void Create()
    {
        GUILayout.Space(8);

        if (GUILayout.Button("Load Directory"))
        {
            loadDir = !loadDir;

            if (loadDir)
            {
                folderToggleStates.Clear();

                // If the project path is valid, use it. Otherwise, use the Assets folder.
                AddFolderToStates(Directory.Exists(ProjectPath) ? ProjectPath : Application.dataPath);
            }
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
        foreach (string folder in folderToggleStates.Keys.ToArray())
        {
            folderToggleStates[folder] = EditorGUILayout.Toggle(Path.GetFileName(folder), folderToggleStates[folder]);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create Subfolder"))
        {
            selectedFolder = GetSelectedFolder();

            if (selectedFolder != null)
            {
                string newFolderPath = Path.Combine(selectedFolder, "New Folder");

                // if the string already exists, add a number to the end of it
                if (Directory.Exists(newFolderPath))
                {
                    int i = 1;
                    while (Directory.Exists(newFolderPath + $" ({i})")) i++;
                    newFolderPath += $" ({i})";
                }
                
                folderChanges.Add(newFolderPath, selectedFolder);
            }
        }

        // GUILayout.Label //TODO: I can make this remind the user of the utilities panel.
        // ($"{ProjectPath} \n"           +
        //  $"L {subFolderArt}"           +
        //  $"   L {subFolderAnimations}" +
        //  $"L {subFolderScripts}"       +
        //  $"   L {subFolderEditor}"     +
        //  $"   L {subFolderRuntime}"    +
        //  $"L {subfolderScenes}", wordWrapRichTextLabelStyle);

        // if (GUILayout.Button("Rename Selected"))
        // {
        //     selectedFolder = GetSelectedFolder();
        //     
        //     if (selectedFolder != null)
        //     {
        //         string newFolderPath = Path.Combine(Directory.GetParent(selectedFolder).FullName, "New Folder");
        //         folderChanges.Add(newFolderPath, selectedFolder);
        //         AddFolderToStates(newFolderPath);
        //     }
        // }

        if (GUILayout.Button("Apply Changes")) ApplyChanges();
    }

    static void AddFolderToStates(string folderPath)
    {
        folderToggleStates.Add(folderPath, false);

        foreach (string subfolder in Directory.GetDirectories(folderPath))
        {
            folderToggleStates.Add(subfolder, false);
        }
    }

    static string GetSelectedFolder()
    {
        foreach (KeyValuePair<string, bool> kvp in folderToggleStates)
        {
            if (kvp.Value) return kvp.Key;
        }

        return null;
    }

    internal static void ApplyChanges()
    {
        foreach (KeyValuePair<string, string> change in folderChanges)
        {
            // Check if the directory already exists before creating it.
            if (!Directory.Exists(change.Key)) { Directory.CreateDirectory(change.Key); }

            // Reflect these changes to folderToggleStates
            folderToggleStates.TryAdd(change.Key, false);
        }

        folderChanges.Clear();
    }

    internal static void ClearAllChanges()
    {
        folderChanges.Clear();
        folderToggleStates.Clear();
    }
}
}
