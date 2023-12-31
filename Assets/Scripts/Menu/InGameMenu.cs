using Assets.Scripts;
using Assets.Scripts.Maps;
using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject inGameMenuGO;
    public GameObject pauseButton;

    private bool isPaused = false;

    public void BackToMainMenu()
    {
        Resume();
        StartCoroutine(LoadsyncScene("MainMenu"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player.IsLose)
            {
                return;
            }

            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && player.IsLose)
        {
            Time.timeScale = pause ? 0 : 1;
            return;
        }

        if (isPaused)
        {
            Pause();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && player.IsLose)
        {
            Time.timeScale = focus ? 1 : 0;
            return;
        }

        if (!focus)
        {
            Pause();
        }
    }

    public void Pause()
    {
        pauseButton.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;
        inGameMenuGO.SetActive(true);

        //FindObjectOfType<Joystick>().gameObject.SetActive(false);
    }

    public void Resume()
    {
        pauseButton.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
        inGameMenuGO.SetActive(false);

        //FindObjectOfType<Joystick>(true).gameObject.SetActive(true);
    }

    public void Restart()
    {
        pauseButton.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
        inGameMenuGO.SetActive(false);
        FindObjectOfType<MapController>().Restart();
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
