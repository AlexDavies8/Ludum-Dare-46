using UnityEngine;

public class BounceTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DogController dogController = collision.GetComponent<DogController>();
        if (dogController != null)
        {
            dogController.direction = -dogController.direction;
        }
    }
}
