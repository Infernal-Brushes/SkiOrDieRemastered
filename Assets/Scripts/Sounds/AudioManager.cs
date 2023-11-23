using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference PressButtonEvent;
    [SerializeField] private FMODUnity.EventReference ReleaseButtonEvent;
    [SerializeField] private FMODUnity.EventReference OnButtonEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void CoursorHovered() {
        RuntimeManager.PlayOneShot(OnButtonEvent);
     }
       

    public void UpMouseButton() {
         RuntimeManager.PlayOneShot(ReleaseButtonEvent);
    }

    public void DownMouseButton() {
        RuntimeManager.PlayOneShot(PressButtonEvent);
    }
}
