using Assets.Scripts.Player;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [Tooltip("Объект для следования камеры")]
    [SerializeField]
    private GameObject _objectToFollow;

    public float smoothTime = 0.2f;
    private Vector3 _relative;
    private Vector3 _offsetPlayerToCamera;
    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _offsetPlayerToCamera = transform.position - _objectToFollow.transform.position;
    }

    private void Update()
    {
        if (_objectToFollow == null)
        {
            return;
        }

        var playerController = _objectToFollow.GetComponentInParent<PlayerController>();

        Vector3 targetPosition = _objectToFollow.transform.TransformPoint(_relative) + _offsetPlayerToCamera;
        _velocity = playerController.playerRigidBody.velocity;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 0.0002f);
    }

    /// <summary>
    /// Задать объект для следования
    /// </summary>
    /// <param name="objectToFollow">Объект для следования</param>
    public void SetObjectToFollow(GameObject objectToFollow)
    {
        _objectToFollow = objectToFollow;
    }
}
