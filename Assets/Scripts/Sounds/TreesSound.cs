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
        treesInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
         treesInstance.setParameterByName("Speed",playerController.speedOfTiltX);
    }
}
