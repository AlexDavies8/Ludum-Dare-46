using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float upAngle = 0f, downAngle = 90f;

    private void Update()
    {
        if (GameManager.inst.platforms)
        {
            float targetAngle = Mathf.LerpAngle(transform.localEulerAngles.z, upAngle, Time.deltaTime * 10f);
            transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else
        {
            float targetAngle = Mathf.LerpAngle(transform.localEulerAngles.z, downAngle, Time.deltaTime * 10f);
            transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }
}
