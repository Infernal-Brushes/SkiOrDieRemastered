using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Assets.Scripts;

public class TreesSound : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference TreesEvent;
    FMOD.Studio.EventInstance treesInstance;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        treesInstance = RuntimeManager.CreateInstance(TreesEvent);
        treesInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        treesInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
         treesInstance.setParameterByName("Speed",playerController.VelocityForward);
    }
}
