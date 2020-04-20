using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public float speedMin = 1f, speedMax = 1.5f;
    public float speedMapMin = 1f, speedMapMax = 1.5f;
    public AudioClip normalMusic;
    public AudioClip deadMusic;

    AudioSource source;

    public static MusicController inst;

    bool dead = false;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);

        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();

        ResetMusic();

        SceneManager.sceneLoaded += (a, b) => ResetMusic();
    }

    private void Update()
    {
        GameManager gm = GameManager.inst;
        if (gm != null && !dead) source.pitch = Mathf.Lerp(speedMin, speedMax, Mathf.InverseLerp(speedMapMin, speedMapMax, gm.globalSpeed));
        else source.pitch = 1;
    }

    public void Died()
    {
        dead = true;

        source.clip = deadMusic;
        source.Play();
    }

    public void ResetMusic()
    {
        if (source.clip == normalMusic) return;

        source.clip = normalMusic;
        source.Play();
    }
}
