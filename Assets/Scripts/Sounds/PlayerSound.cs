using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using FMODUnity;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference GlideEvent;
    FMOD.Studio.EventInstance glideInstance;
    public PlayerController playerController;
    
    bool Dead;
    // Start is called before the first frame update
    void Start()
    {
        glideInstance = RuntimeManager.CreateInstance(GlideEvent);
       RuntimeManager.AttachInstanceToGameObject(glideInstance, transform, playerController);
        glideInstance.start();
        
    }

    // Update is called once per frame
    void Update()
    {
         glideInstance.setParameterByName("Speed",playerController.VelocityForward);
    }

    void StopSoundGlide()
    {
        if(Dead == true)
        {
            glideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
        } 

    }
    void OnDestroy()
    {
        glideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }
}
