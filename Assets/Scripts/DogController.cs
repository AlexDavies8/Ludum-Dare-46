using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public Animator animator;

    public ParticleSystem deadParticles;

    public int direction = 1;

    public float explodeScale = 1.5f;
    public float implodeScale = 0.7f;

    public CameraShake cameraShake;

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
        if (stopped || dead)
        {
            rb.velocity = new Vector2(0, 0);
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;

            if (direction == 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
                rb.velocity = new Vector2(movementSpeed * GameManager.inst.globalSpeed, rb.velocity.y);
            }
            else if (direction == -1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                rb.velocity = new Vector2(-movementSpeed * GameManager.inst.globalSpeed, rb.velocity.y);
            }
        }

        animator.speed = GameManager.inst.globalSpeed;
    }

    public void SetAnimation(string id, bool ignoreDead = false)
    {
        if (!dead || ignoreDead) animator.Play(id, 0);
    }

    public void Die(bool ignoreDead = false)
    {
        if (dead && !ignoreDead) return;

        dead = true;
        SetAnimation("Dog_Dead", true);
        deadParticles.Play();
        stopped = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        cameraShake.trauma = 1f;

        SoundEffectManager.inst.PlaySound("Die");

        Destroy(this);
    }

    public void Explode()
    {
        if (dead) return;
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        dead = true;
        stopped = true;
        SetAnimation("Dog_Idle");

        SoundEffectManager.inst.PlaySound("Stretch");

        Vector3 startScale = transform.localScale;
        Vector3 endScale = transform.localScale * explodeScale;
        endScale.z = startScale.z;
        for (float t = 0; t < 1f; t += Time.deltaTime * 1.5f)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        transform.localScale = startScale;
        GameManager.inst.Die("Your dog exploded!");
        Die(true);
    }

    public void Implode()
    {
        if (dead) return;
        StartCoroutine(ImplodeCoroutine());
    }

    IEnumerator ImplodeCoroutine()
    {
        dead = true;
        stopped = true;
        SetAnimation("Dog_Idle");

        SoundEffectManager.inst.PlaySound("Stretch");
        
        Vector3 startScale = transform.localScale;
        Vector3 endScale = transform.localScale * implodeScale;
        endScale.z = startScale.z;
        for (float t = 0; t < 1f; t += Time.deltaTime * 1.5f)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        transform.localScale = startScale;
        GameManager.inst.Die("Your dog imploded!");
        Die(true);
    }
}
