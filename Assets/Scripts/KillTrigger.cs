using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    public string deathText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DogController dogController = collision.GetComponent<DogController>();
        if (dogController != null)
        {
            GameManager.inst.Die(deathText);
            dogController.Die();
        }
    }
}
