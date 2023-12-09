using UnityEngine;
using UnityEngine.UI;

public class UISliderMusic : MonoBehaviour {
    private Slider SliderMusic;
    private FMOD.Studio.VCA MusicVCA;
    private float MusicVolume;
    public string VCAName;

    void Start() {
        SliderMusic = gameObject.GetComponent<Slider>();
        MusicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);
        MusicVCA.getVolume(out MusicVolume);
        SliderMusic.value = MusicVolume;
    }

    void Update() {
        MusicVolume = SliderMusic.value;
        Debug.Log(MusicVolume);
        MusicVCA.setVolume(MusicVolume);
    }
}
