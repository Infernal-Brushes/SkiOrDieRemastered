using Assets.Scripts;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public GameObject player;
    public float smoothTime = 0.2f;
    private Vector3 _relative;
    private Vector3 _offsetPlayerToCamera;
    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _offsetPlayerToCamera = transform.position - player.transform.position;
    }

    private void Update()
    {
        var playerController = player.GetComponentInParent<PlayerController>();

        Vector3 targetPosition = player.transform.TransformPoint(_relative) + _offsetPlayerToCamera;
        _velocity = playerController.playerRigidBody.velocity;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 0.0002f);
    }
}
