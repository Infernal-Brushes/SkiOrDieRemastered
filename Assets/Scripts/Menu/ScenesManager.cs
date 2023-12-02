using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter _studioEventEmitter;

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
        _studioEventEmitter.Stop();
        StartScene("Freeride");
    }

    IEnumerator LoadsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
