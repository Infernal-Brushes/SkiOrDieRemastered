using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using FMODUnity;

public class MusicRide : MonoBehaviour
{
     [SerializeField] private FMODUnity.EventReference MusicEvent;
    FMOD.Studio.EventInstance musicInstance;
    [SerializeField] PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(MusicEvent);
        musicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
         musicInstance.setParameterByName("SpeedMusic",playerController.VelocityForward);
    }


     void OnDestroy()
    {
       musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
