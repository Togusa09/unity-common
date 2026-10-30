using UnityEngine;

namespace Anarksoft.SharedCore
{
    public static class SharedCoreSystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("[SharedCore] SharedCoreSystem initialized successfully.");
        }
    }
}
