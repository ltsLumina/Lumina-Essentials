#region
using System;
using UnityEngine;
using static UnityEngine.Object;
using Random = UnityEngine.Random;
#endregion

namespace Lumina.Essentials
{
    /// <summary>
    ///     General helper methods that don't fit into any other category.
    /// </summary>
    public static class Helpers
    {
        #region Camera
        static Camera cameraMain;

        /// <summary>
        ///     Allows you to call camera.main without it being expensive, as we cache it here after the first call.
        ///     <example>Helpers.Camera.transform.position.etc </example>
        /// </summary>
        public static Camera Camera
        {
            get
            {
                if (cameraMain == null) cameraMain = Camera.main;
                return cameraMain;
            }
        }
        #endregion

        #region Audio
        /// <summary>
        ///     Plays the given audio clip on the given audio source with a random pitch between the given min and max pitch.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="audioSource"></param>
        /// <param name="minPitch"></param>
        /// <param name="maxPitch"></param>
        public static void PlayRandomPitch(AudioClip audioClip, AudioSource audioSource, float minPitch, float maxPitch)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.PlayOneShot(audioClip);
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        ///     Destroys all children of the given transform.
        ///     Can be used as extension method.
        /// </summary>
        /// <param name="parent"></param>
        public static void DeleteAllChildren(this Transform parent)
        {
            foreach (Transform child in parent) { Destroy(child.gameObject); }
        }

        /// <summary>
        ///     Returns a random Vector2 between the given min and max values.
        /// </summary>
        /// <param name="Vector2"> The Vector2 to be used as the base for the random Vector2.</param>
        /// <param name="min"> The minimum value for the random Vector2.</param>
        /// <param name="max"> The maximum value for the random Vector2.</param>
        /// <returns></returns>
        public static Vector2 RandomVector(this Vector2 Vector2, float min, float max) 
            => new (Random.Range(min, max), Random.Range(min, max));

        /// <summary>
        /// Overload of the RandomVector method that takes a Vector3 instead of a Vector2.
        /// </summary>
        /// <param name="Vector3"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector3 RandomVector(this Vector3 Vector3, float min, float max) 
            => new (Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        #endregion
    }
}
