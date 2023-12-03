using Assets.Scripts.Player;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    /// <summary>
    /// Триггер риска
    /// </summary>
    private CapsuleCollider _riskCollider;

    private void Awake()
    {
        _riskCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerController))
        {
            playerController.Lose(PlayerController.LoseCause.barrier);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerController playerController = collider.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            playerController.EarnMoneyForRisk();
            _riskCollider.enabled = false;
        }
    }
}
