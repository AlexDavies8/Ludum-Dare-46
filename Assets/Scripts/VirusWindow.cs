using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusWindow : MonoBehaviour
{
    public VirusManager manager;

    public void DestroyWindow()
    {
        Destroy(gameObject);
    }

    public void Recurse()
    {
        manager.SpawnWindow(0);
    }
}
