using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class ScenesManager : MonoBehaviour
    {
        public delegate void OnSceneChangingDelegate(string sceneName);

        public event OnSceneChangingDelegate OnSceneChanging;

        public void StartScene(string sceneName)
        {
            StartCoroutine(LoadsyncScene(sceneName));
        }

        public void StartMainMenuScene()
        {
            StartScene("MainMenu");
        }

        public void StartShopScene()
        {
            StartScene("Shop");
        }

        public void StartFreerideScene()
        {
            StartScene("Freeride");
        }

        private IEnumerator LoadsyncScene(string sceneName)
        {
            OnSceneChanging?.Invoke(sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}