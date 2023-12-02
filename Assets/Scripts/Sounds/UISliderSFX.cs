using UnityEngine;

public class UISliderSFX : MonoBehaviour {
    private UnityEngine.UI.Slider SliderNoise;
    private FMOD.Studio.VCA SoundsVCA;
    private float SoundVolume;
    public string VCAName;

    void Start() {
        SliderNoise = gameObject.GetComponent<UnityEngine.UI.Slider>();
        SoundsVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);
        SoundsVCA.getVolume(out SoundVolume);
        SliderNoise.value = SoundVolume;
    }

    void Update() {
        SoundVolume = SliderNoise.value;
        SoundsVCA.setVolume(SoundVolume);
    }
}
