using Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public LocalizationManager localizationManager;

    private UserDataController _userDataController;

    // Start is called before the first frame update
    void Start()
    {
        _userDataController = FindObjectOfType<UserDataController>();

        localizationManager?.SetLocalization(_userDataController.UserDataModel.LocalizationCode);
    }

    public void ChangeLocalization(string localizationCode)
    {
        _userDataController.UserDataModel.SetLocalizationCode(localizationCode);
        localizationManager.SetLocalization(localizationCode);

        var texts = FindObjectsOfType<LocalizedText>(true);
        //обновить все текста на новый язык
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].Refresh();
        }
    }
}
