using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTrigger : MonoBehaviour
{
    public float chance = 0.5f;

    public float minTime = 3f;
    public float maxTime = 5f;

    public float bounceChance = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DogController dogController = collision.GetComponent<DogController>();
        if (dogController != null)
        {
            if (Random.Range(0f, 1f) < chance) StartCoroutine(WaitCoroutine(dogController));
        }
    }

    IEnumerator WaitCoroutine(DogController dogController)
    {
        dogController.stopped = true;
        dogController.SetAnimation("Dog_Idle");

        yield return new WaitForSeconds(Random.Range(minTime / GameManager.inst.globalSpeed, maxTime / GameManager.inst.globalSpeed));

        if (Random.Range(0f, 1f) < bounceChance) dogController.direction = -dogController.direction;

        dogController.SetAnimation("Dog_Walk");
        dogController.stopped = false;
    }
}
