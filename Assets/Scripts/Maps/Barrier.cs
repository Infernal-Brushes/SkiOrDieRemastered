using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Barrier OnCollisionEnter");
        if (collision.gameObject.tag == "Player")
        //if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            collision.gameObject.GetComponentInParent<PlayerController>().Lose(PlayerController.LoseCause.barrier);
        }
    }
}
