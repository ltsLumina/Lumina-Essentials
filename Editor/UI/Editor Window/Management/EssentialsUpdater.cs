#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static Lumina.Essentials.Editor.UI.Management.VersionManager;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
    public static class EssentialsUpdater
    {
        /// <summary> Whether or not the current version is the latest version. </summary>
        public static string LastUpdateCheck => StartupChecks.TimeSinceLastUpdate();
        
        readonly static Queue<IEnumerator> coroutineQueue = new ();

        public static void CheckForUpdates()
        {
            EditorApplication.update += Update;
            coroutineQueue.Enqueue(RequestUpdateCheck());
        }

        static void Update()
        {
            if (coroutineQueue.Count > 0)
            {
                IEnumerator coroutine = coroutineQueue.Peek();
                if (!coroutine.MoveNext()) coroutineQueue.Dequeue();
            }
            else { EditorApplication.update -= Update; }
        }

        static IEnumerator RequestUpdateCheck()
        {
            using UnityWebRequest www = UnityWebRequest.Get("https://api.github.com/repos/ltsLumina/Unity-Essentials/releases/latest");

            yield return www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null; // Wait for the request to complete
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("UnityWebRequest failed with result: " + www.result);
                Debug.LogError("Error message: "                      + www.error);
            }
            else
            {
                string jsonResult = Encoding.UTF8.GetString(www.downloadHandler.data);
                string tag        = JsonUtility.FromJson<Release>(jsonResult).tag_name;

                // Update LatestVersion, UpToDate, LastUpdateCheck accordingly.
                UpdateStatistics(tag);
                EditorPrefs.SetString("LastUpdateCheck", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                
                // Compare tag with CurrentVersion
                if (!EditorPrefs.GetBool("UpToDate"))
                {
                    // Warn user that they are using an outdated version.
                    DebugHelper.Log("You are using an outdated version. \n Latest Version: v" + tag + "\n" + "You are using version: v" + CurrentVersion);
                }
            }
        }

    }

    [Serializable]
    public class Release
    {
        public string tag_name;
    }
}
