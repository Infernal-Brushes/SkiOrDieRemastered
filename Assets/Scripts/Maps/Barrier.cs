using Assets.Scripts;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerController))
        {
            playerController.Lose(PlayerController.LoseCause.barrier);
        }
    }
}
