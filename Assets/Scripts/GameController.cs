using Assets.Scripts;
using Assets.Scripts.Models.Users;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public LocalizationManager localizationManager;

    private IUserDataModel _userDataModel;

    // Start is called before the first frame update
    void Awake()
    {
        _userDataModel = new UserDataModel();
        _userDataModel.Fetch();

        localizationManager?.SetLocalization(_userDataModel.LocalizationCode);
    }

    public void ChangeLocalization(string localizationCode)
    {
        _userDataModel.SetLocalizationCode(localizationCode);
        localizationManager.SetLocalization(localizationCode);

        var texts = FindObjectsOfType<LocalizedText>(true);
        //обновить все текста на новый язык
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].Refresh();
        }
    }
}
