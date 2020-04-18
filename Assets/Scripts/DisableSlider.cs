using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableSlider : MonoBehaviour
{
    public float increaseAmount = 0.4f;
    public float decreaseSpeed = 0.2f;
    public Slider slider;

    private void Update()
    {
        if (slider.value > 0)
        {
            slider.value -= Time.deltaTime * decreaseSpeed;
        }
    }

    public void Increase()
    {
        slider.value += increaseAmount;
    }
}
