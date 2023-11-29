using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using FMODUnity;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference GlideEvent;
    [SerializeField] private FMODUnity.EventReference BodyEvent;
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
