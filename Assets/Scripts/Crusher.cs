using UnityEngine;

public class Crusher : MonoBehaviour
{
    public Vector2 up, down;

    public float speed = 1f;
    public float offset = 1f;
    public ParticleSystem particles;
    public float particleTime = 0.3f;
    public AnimationCurve moveCurve;

    float t;
    bool particlesPlayed;

    private void Update()
    {
        float _t = (t + offset) % 1;

        transform.localPosition = Vector2.Lerp(down, up, moveCurve.Evaluate(_t));

        if(t > particleTime && !particlesPlayed)
        {
            particlesPlayed = true;
            particles.time = 0;
            particles.Play();
        }

        t += Time.deltaTime * speed;

        if (t > 1)
        {
            particlesPlayed = false;
            t -= 1;
        }
    }
}
