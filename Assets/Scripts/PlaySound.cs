using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public string soundName;
    public float volume;

    public void Play()
    {
        SoundEffectManager.inst.PlaySound(soundName, volume);
    }
}
