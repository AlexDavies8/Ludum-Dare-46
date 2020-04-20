using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Range(0, 1)] public float trauma;
    public float traumaDecreaseSpeed = 0.5f;
    public Transform targetTransform;
    public float rotationMultiplier = 10f;
    public float movementMultiplier = 0.5f;
    public float shakeSpeed = 1f;

    private void Update()
    {
        trauma = Mathf.Clamp(trauma - Time.deltaTime * traumaDecreaseSpeed, 0f, 1f);

        float shakeAmount = trauma * trauma;
        targetTransform.localPosition = new Vector2((Mathf.PerlinNoise(Time.time * shakeSpeed, 0.11f) * 2 - 1), (Mathf.PerlinNoise(0.11f, Time.time * shakeSpeed) * 2 - 1)) * shakeAmount * movementMultiplier;
        Vector3 angles = targetTransform.localEulerAngles;
        angles.z = (Mathf.PerlinNoise(Time.time * 0.77f * shakeSpeed, Time.time * 0.77f * shakeSpeed) * 2 - 1) * shakeAmount * rotationMultiplier;
        targetTransform.localRotation = Quaternion.Euler(angles);
    }
}
