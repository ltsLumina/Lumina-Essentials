using System;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;

namespace Lumina.Essentials.Editor.UI
{
[Obsolete("This class has been deprecated. It remains for reference.")]
public class ProjectStructureWindow : EditorWindow
{
#if DEBUG_BUILD
    [MenuItem("Lumina's Essentials/Show Project Structure Window")]
    public static void ShowWindow()
    {
        DebugHelper.LogError($"You are using a deprecated class!" + "(ProjectStructureWindow)");
        
        var window = GetWindow<ProjectStructureWindow>("Project Structure");
        window.minSize = new Vector2(350, 500);
        window.Show();
    }
#endif

    void OnGUI()
    {
        DebugHelper.LogError($"You are using a deprecated class!" + "(ProjectStructureWindow)");
        
#pragma warning disable CS8321 // Local function is declared but never used
        void DrawCreateProjectStructureConfig()
#pragma warning restore CS8321 // Local function is declared but never used
        {
            GUILayout.Label("Create Project Structure", centerLabelStyle);
            GUILayout.Label("Creates the default project structure.", subLabelStyle);
            GUILayout.Space(10);

            // Drag and drop for folder to use

            // From the Utilities Window
            //UtilityWindow.DrawDragAndDropConfig(createProjectEnum);
        }

        ProjectStructureGUI.Create();
    }

    void OnDestroy() { ProjectStructureGUI.ClearAllChanges(); }
}
}
