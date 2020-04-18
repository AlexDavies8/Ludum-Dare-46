using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : MonoBehaviour
{
    public GameObject[] virusPrefabs;
    public Vector2 min, max;
    public float interval = 0.1f;

    public void ActivateVirus()
    {
        SpawnWindows(Random.Range(4, 8));
    }

    public void SpawnWindows(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnWindow(i);
        }
    }

    public void SpawnWindow(int i)
    {
        StartCoroutine(SpawnWindowCoroutine(i));
    }

    IEnumerator SpawnWindowCoroutine(int i)
    {
        yield return new WaitForSeconds(interval * i);

        Vector2 spawnPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        int prefabIndex = Random.Range(0, virusPrefabs.Length);

        GameObject virusWindow = Instantiate(virusPrefabs[prefabIndex], spawnPos, Quaternion.identity, transform);
        VirusWindow virusWindowScript = virusWindow.GetComponent<VirusWindow>();
        virusWindowScript.manager = this;
    }
}
