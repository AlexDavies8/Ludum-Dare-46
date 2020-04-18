using UnityEngine;

public class DogController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public Animator animator;

    public ParticleSystem deadParticles;

    public int direction = 1;

    Rigidbody2D rb;

    public bool stopped;
    public bool dead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetAnimation("Dog_Walk");
    }

    private void Update()
    {
        if (stopped == true)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;

            if (direction == 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
                rb.velocity = new Vector2(movementSpeed, 0);
            }
            else if (direction == -1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                rb.velocity = new Vector2(-movementSpeed, 0);
            }
        }
    }

    public void SetAnimation(string id)
    {
        animator.Play(id, 0);
    }

    public void Die()
    {
        if (dead) return;

        dead = true;
        SetAnimation("Dog_Dead");
        deadParticles.Play();
        stopped = true;
    }
}
