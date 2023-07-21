#region
using System;
using System.Threading;
using System.Threading.Tasks;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
#endregion

namespace Tarodev
{
[CustomEditor(typeof(AutoSave))]
public class AutoSave : Editor
{
    // I'm assuming AutoSaveConfig is a regular class
    //static AutoSaveConfig AutoSave;
    static CancellationTokenSource tokenSource;
    static Task task;

    [InitializeOnLoadMethod]
    static void OnInitialize()
    {
        // Initialize your AutoSaveConfig class
        //AutoSave = new ();

        CancelTask();

        tokenSource = new ();
        task        = SaveInterval(tokenSource.Token);
    }

    static void CancelTask()
    {
        if (task == null) return;
        tokenSource.Cancel();
        task.Wait();
    }

    static async Task SaveInterval(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(AutoSaveConfig.Interval * 1000 * 60, token);

            if (!AutoSaveConfig.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) continue;
            if (!InternalEditorUtility.isApplicationActive) continue;

            EditorSceneManager.SaveOpenScenes();
            if (AutoSaveConfig.Logging) Debug.Log($"Auto-saved at {DateTime.Now:h:mm:ss tt}");
        }
    }
}
}
