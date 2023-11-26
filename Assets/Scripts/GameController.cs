using Assets.Scripts;
using Assets.Scripts.Player;
using Assets.Scripts.Wears;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public User user;
    public LocalizationManager localizationManager;

    public Wear mockWearTorso;
    public Wear mockWearLegs;
    public Wear mockWearBoots;
    public Wear mockWearHat;
    public Wear mockWearGlases;

    // Start is called before the first frame update
    void Awake()
    {
        //user.MockResetUser();
        user.LoadUser();
        localizationManager?.SetLocalization(user.LocalizationCode);
    }

    private void Start()
    {
        var playerAppearance = FindObjectOfType<PlayerAppearance>();

        if (playerAppearance == null)
        {
            return;
        }

        GameObject go;
        if (mockWearTorso != null)
            playerAppearance.PutOnWear(mockWearTorso, out go);
        if (mockWearLegs != null)
            playerAppearance.PutOnWear(mockWearLegs, out go);
        if (mockWearBoots != null)
            playerAppearance.PutOnWear(mockWearBoots, out go);
        if (mockWearHat != null)
            playerAppearance.PutOnWear(mockWearHat, out go);
        if (mockWearGlases != null)
            playerAppearance.PutOnWear(mockWearGlases, out go);
    }

    public void ChangeLocalization(string localizationCode)
    {
        localizationManager.SetLocalization(localizationCode);

        var texts = FindObjectsOfType<LocalizedText>(true);
        //обновить все текста на новый язык
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].Refresh();
        }
    }
}
