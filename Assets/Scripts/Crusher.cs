using UnityEngine;

public class Crusher : MonoBehaviour
{
    public Vector2 up, down;

    public float speed = 1f;
    public float offset = 1f;
    public ParticleSystem particles;
    public float particleTime = 0.3f;
    public AnimationCurve moveCurve;

    public Transform tracked;
    public float soundRangeMin = 3f;
    public float soundRangeMax = 6f;

    Vector2 startPosition;

    float t;
    bool particlesPlayed;

    private void Awake()
    {
        startPosition = transform.localPosition;

        t = offset;
    }

    private void Update()
    {
        Vector2 targetPos = Vector2.Lerp(startPosition + up, startPosition + down, moveCurve.Evaluate(t));

        if (GameManager.inst.crushers)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, startPosition + up, Time.deltaTime * 10f);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, targetPos, Time.deltaTime * 20f);

            if (t > particleTime && !particlesPlayed)
            {
                particlesPlayed = true;
                particles.time = 0;
                particles.Play();

                SoundEffectManager.inst.PlaySound("Hit", Mathf.InverseLerp(soundRangeMax, soundRangeMin, Vector2.Distance(tracked.position, transform.position)));
            }
        }

        t += Time.deltaTime * speed;

        if (t > 1)
        {
            particlesPlayed = false;
            t -= 1;
        }
    }
}
