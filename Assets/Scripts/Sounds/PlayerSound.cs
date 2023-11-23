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
        glideInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        glideInstance.start();
        
    }

    // Update is called once per frame
    void Update()
    {
         glideInstance.setParameterByName("Speed",playerController.speedOfTiltX);
    }

    void StopSoundGlide()
    {
        if(Dead == true)
        {
            glideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
        } 

    }
}
