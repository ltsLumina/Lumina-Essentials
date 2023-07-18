#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Lumina.Essentials.Sequencer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static Lumina.Essentials.Editor.VersionManager;
#endregion

namespace Lumina.Essentials.Editor
{
    public static class EssentialsUpdater
    {
        /// <summary> Whether or not the current version is the latest version. </summary>
        public static string LastUpdateCheck => TimeSinceLastUpdate();
        
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

                // Compare tag with CurrentVersion
                if (!EditorPrefs.GetBool("UpToDate") && !DebugVersion)
                {
                    // Warn user that they are using an outdated version.
                    DebugHelper.Log("You are using an outdated version. \n Latest Version: v" + tag + "\n" + "You are using version: v" + CurrentVersion);

                    // Update LatestVersion, UpToDate, LastUpdateCheck accordingly.
                    UpdateStatistics(tag);
                    EditorPrefs.SetString("LastUpdateCheck", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    // Update LatestVersion, UpToDate, LastUpdateCheck accordingly.
                    UpdateStatistics(tag);
                    EditorPrefs.SetString("LastUpdateCheck", DateTime.Now.ToString(CultureInfo.InvariantCulture));
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
