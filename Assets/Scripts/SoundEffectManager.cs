using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] List<SoundEffect> SoundEffects = new List<SoundEffect>();
    Dictionary<string, AudioClip> soundEffects;

    AudioSource source;

    public static SoundEffectManager inst;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);

        source = GetComponent<AudioSource>();

        soundEffects = new Dictionary<string, AudioClip>();
        foreach (SoundEffect sfx in SoundEffects)
        {
            soundEffects.Add(sfx.name, sfx.clip);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string name)
    {
        if (soundEffects.ContainsKey(name))
        {
            source.PlayOneShot(soundEffects[name], 1f);
        }
    }

    public void PlaySound(string name, float volume = 1f)
    {
        if (soundEffects.ContainsKey(name))
        {
            source.PlayOneShot(soundEffects[name], volume);
        }
    }

    private void OnDestroy()
    {
        inst = null;
    }

    [System.Serializable]
    struct SoundEffect
    {
        public string name;
        public AudioClip clip;
    }
}
