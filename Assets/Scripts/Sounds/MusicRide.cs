using Assets.Scripts.Player;
using FMODUnity;
using UnityEngine;

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

    private void OnApplicationPause(bool pause)
    {
        musicInstance.setPaused(pause);
    }

    private void OnApplicationFocus(bool focus)
    {
        musicInstance.setPaused(!focus);
    }

    void OnDestroy()
    {
       musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
