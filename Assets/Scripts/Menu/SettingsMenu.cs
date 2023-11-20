using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsGO;

    private void Start()
    {
        settingsGO.SetActive(false);
    }

    public void Show()
    {
        settingsGO.SetActive(true);
    }

    public void Hide()
    {
        settingsGO.SetActive(false);
    }
}
