// Uncomment the #define statement to enable the use of the initialization script.
//#define USING_INITIALIZATION
// I highly recommended you use this script, as it will save you a lot of time and effort struggling with singletons.

#region
using UnityEngine;
using static UnityEngine.Object;
#endregion

/// <summary>
/// Simple class to initialize the game.
/// Instantiates the Systems prefab that includes every manager in your game.
/// Make sure to create a Resources folder in your Assets folder and place the Systems prefab in there (or just use the one provided).
/// The Systems prefab should be the parent of all the managers in your game.
/// Each manager script should be its own child GameObject of the Systems prefab.
/// 
/// Check GitHub for more information:
/// https://github.com/ltsLumina/Unity-Essentials
/// </summary>
public static class Initialization
{
#if USING_INITIALIZATION
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
#endif
}