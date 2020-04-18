using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2 upOffset;
    public Vector2 downOffset;

    Vector2 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (GameManager.inst.doors)
        {
            Vector2 targetPosition = Vector2.Lerp(transform.localPosition, startPosition + downOffset, Time.deltaTime * 10f);
            transform.localPosition = targetPosition;
        }
        else
        {
            Vector2 targetPosition = Vector2.Lerp(transform.localPosition, startPosition + upOffset, Time.deltaTime * 10f);
            transform.localPosition = targetPosition;
        }
    }
}
