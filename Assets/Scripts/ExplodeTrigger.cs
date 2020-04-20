using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DogController dogController = collision.GetComponent<DogController>();
        if (dogController != null)
        {
            GameManager.inst.Die("Your dog exploded in space!");
            dogController.Explode();
        }
    }
}
