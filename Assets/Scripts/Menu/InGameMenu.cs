using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject inGameMenuGO;
    public GameObject pauseButton;

    private bool isPaused = false;

    private void Start()
    {
        var player = FindObjectOfType<PlayerController>();
       
    }

    public void BackToMainMenu()
    {
        Resume();
        StartCoroutine(LoadsyncScene("MainMenu"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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

    public void Pause()
    {
        pauseButton.SetActive(false);
        Debug.Log("Paused");
        Time.timeScale = 0;
        isPaused = true;
        inGameMenuGO.SetActive(true);

        FindObjectOfType<Joystick>().gameObject.SetActive(false);
    }

    public void Resume()
    {
        pauseButton.SetActive(true);
        Debug.Log("Unpaused");
        Time.timeScale = 1;
        isPaused = false;
        inGameMenuGO.SetActive(false);

        FindObjectOfType<Joystick>(true).gameObject.SetActive(true);
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
