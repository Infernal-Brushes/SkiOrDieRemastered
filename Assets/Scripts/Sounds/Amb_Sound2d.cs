using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using FMODUnity;
using UnityEngine;

public class Amb_Sound2d : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference AmbientEvent;
    FMOD.Studio.EventInstance ambInstance;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        ambInstance = RuntimeManager.CreateInstance(AmbientEvent);
        ambInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        ambInstance.setParameterByName("SpeedAmb",playerController.VelocityForward);
    }

     void OnDestroy()
    {
       ambInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
