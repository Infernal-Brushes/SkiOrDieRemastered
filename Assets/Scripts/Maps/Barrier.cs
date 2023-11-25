using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController _))
        //if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponentInParent<PlayerController>().Lose(PlayerController.LoseCause.barrier);
        }
    }
}
