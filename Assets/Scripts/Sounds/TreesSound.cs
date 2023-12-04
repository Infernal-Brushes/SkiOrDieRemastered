using Assets.Scripts.Player;
using FMODUnity;
using UnityEngine;

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

    private void OnApplicationPause(bool pause)
    {
        treesInstance.setPaused(pause);
    }

    private void OnApplicationFocus(bool focus)
    {
        treesInstance.setPaused(!focus);
    }
}
