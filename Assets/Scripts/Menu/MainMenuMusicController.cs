using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MainMenuMusicController : MonoBehaviour
    {
        private static MainMenuMusicController _instance;

        [SerializeField]
        private StudioEventEmitter _studioEventEmitter;

        private ScenesManager _scenesManager;

        private void Awake()
        {
            _scenesManager = FindObjectOfType<ScenesManager>();
            _scenesManager.OnSceneChanging += StopMusicOnSpecifiedSceneLoad;

            if (_instance != null)
            {
                _studioEventEmitter = _instance._studioEventEmitter;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            _instance = this;
        }

        private void OnApplicationPause(bool pause)
        {
            _studioEventEmitter.EventInstance.setPaused(pause);
        }

        private void OnApplicationFocus(bool focus)
        {
            _studioEventEmitter.EventInstance.setPaused(!focus);
        }

        private void StopMusicOnSpecifiedSceneLoad(string sceneName)
        {
            if (sceneName == "Freeride")
            {
                _studioEventEmitter.Stop();
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _scenesManager.OnSceneChanging -= StopMusicOnSpecifiedSceneLoad;
        }
    }
}
