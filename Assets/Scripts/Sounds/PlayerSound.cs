using Assets.Scripts.Player;
using FMODUnity;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private EventReference GlideEvent;
    [SerializeField] private EventReference BodyEvent;
    FMOD.Studio.EventInstance glideInstance;
    FMOD.Studio.EventInstance bodyInstance;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        glideInstance = RuntimeManager.CreateInstance(GlideEvent);
        bodyInstance = RuntimeManager.CreateInstance(BodyEvent);
        StartGlideSound();
        playerController.OnGroundOn += StartGlideSound;
        playerController.OnLose += StopSoundGlide;
        playerController.OnGroundOff += StopSoundGlide;
        playerController.OnBarrierCollision += SoundFallBody;
        
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

    void SoundFallBody()
    {
       bodyInstance.start();
    }

    void OnDestroy()
    {
        playerController.OnLose -= StopSoundGlide;
        playerController.OnGroundOff -= StopSoundGlide; 
        playerController.OnGroundOn -= StartGlideSound;
        playerController.OnBarrierCollision -= SoundFallBody;
        glideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bodyInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
