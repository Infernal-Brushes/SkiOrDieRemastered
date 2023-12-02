using Assets.Scripts.Player;
using FMODUnity;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference GlideEvent;
    FMOD.Studio.EventInstance glideInstance;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        glideInstance = RuntimeManager.CreateInstance(GlideEvent);
        StartGlideSound();
        playerController.OnRestarted += StartGlideSound;
        playerController.OnLose += StopSoundGlide;
    }

    // Update is called once per frame
    void Update()
    {
         glideInstance.setParameterByName("Speed",playerController.VelocityForward);
         glideInstance.setParameterByName("AngleDrift",playerController.AngleOfCurrentTurning);
    }

    void StartGlideSound()
    {
        glideInstance.start();
    }

    void StopSoundGlide()
    {
        glideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
    }

    void OnDestroy()
    {
        playerController.OnRestarted -= StartGlideSound;
        playerController.OnLose -= StopSoundGlide;
    }

}
