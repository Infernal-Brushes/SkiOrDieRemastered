using UnityEngine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace Assets.Scripts
{
    /// <summary>
    /// Контроллер готовности игры
    /// </summary>
    public class GameReadyController : MonoBehaviour
    {
        private GameReadyController _instance;

        private void Start()
        {
            if (_instance == this)
            {
                return;
            }

            _instance = this;

#if UNITY_WEBGL
            OnGameReady();
#endif
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void OnGameReady();
#endif
    }
}
