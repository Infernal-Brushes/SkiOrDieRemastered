using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public GameObject player;
    public GameObject fog;

    public float smoothTime = 0.2f;

    private Vector3 _relative;
    private Vector3 _offsetPlayerToCamera;
    private Vector3 _offsetCameraToFog;
    //private Vector3 _rotation;
    private Vector3 _velocity = Vector3.zero;

    private float fogZ;

    private void Start()
    {
        //if (useInitial)
        //{
        //    _relative = relative;
        //    _offset = transform.position - target.transform.position;
        //    _rotation = transform.rotation.eulerAngles;
        //}
        //else
        //{
        //    _relative = relative;
        //    _offset = offset;
        //    _rotation = rotation;
        //}
        _offsetPlayerToCamera = transform.position - player.transform.position;
        _offsetCameraToFog = fog.transform.position - this.transform.position;
        fogZ = fog.transform.position.z;
        //_offsetCameraToFog = new Vector3(_offsetCameraToFog.x, _offsetCameraToFog.y, 0);
    }

    private void Update()
    {
        var playerController = player.GetComponentInParent<PlayerController>();

        Vector3 targetPosition = player.transform.TransformPoint(_relative) + _offsetPlayerToCamera;
        _velocity = playerController.playerRigidBody.velocity;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 0.0002f);

        if (fog != null)
        {
            targetPosition = transform.TransformPoint(_relative) + _offsetCameraToFog;
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, fogZ);
            _velocity = new Vector3(_velocity.x, 0, 0);
            fog.transform.position = Vector3.SmoothDamp(fog.transform.position, targetPosition, ref _velocity, 0.0002f);
        }
    }
}
