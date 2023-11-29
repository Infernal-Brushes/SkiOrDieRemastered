using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISliderMusic : MonoBehaviour {
    private UnityEngine.UI.Slider SliderMusic;
    private FMOD.Studio.VCA MusicVCA;
    private float MusicVolume;
    public string VCAName;

    void Start() {
        SliderMusic = gameObject.GetComponent<UnityEngine.UI.Slider>();
        MusicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);
        MusicVCA.getVolume(out MusicVolume);
        SliderMusic.value = MusicVolume;
    }

    void Update() {
        MusicVolume = SliderMusic.value;
        MusicVCA.setVolume(MusicVolume);
    }
}
