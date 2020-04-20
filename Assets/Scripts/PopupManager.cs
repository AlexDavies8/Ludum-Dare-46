using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject[] popupPrefabs;
    public Vector2 min, max;

    public void CreatePopup()
    {
        int index = Random.Range(0, popupPrefabs.Length);
        GameObject popupGO = Instantiate(popupPrefabs[index], transform);

        popupGO.transform.position = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

        SoundEffectManager.inst.PlaySound("Select", 1f);
    }
}
