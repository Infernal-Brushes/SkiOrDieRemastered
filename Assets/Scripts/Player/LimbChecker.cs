using Assets.Scripts.Player;
using UnityEngine;

public class LimbChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Debug.Log($"Врезался в препятствие: {this.name}");
            GetComponentInParent<PlayerController>().Lose(PlayerController.LoseCause.barrier);
        }
    }
}
