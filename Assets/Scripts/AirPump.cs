using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirPump : MonoBehaviour
{
    public Slider levelSlider;

    public float fillRate = 0.1f;
    public float emptyThreshold = 1f;
    public float fullThreshold = 4f;
    public float minLevel = 0f;
    public float maxLevel = 5f;

    public Text text;
    public string emptyText;
    public string normalText;
    public string fullText;

    public Image fillImage;
    public Color dangerColour;

    public DogController dogController;

    int direction = 1;

    float level = 0f;

    private void Awake()
    {
        level = emptyThreshold;
    }

    private void Update()
    {
        if (level < emptyThreshold)
        {
            text.text = emptyText;
        }
        else if (level > fullThreshold)
        {
            text.text = fullText;
        }
        else
        {
            text.text = normalText;
        }

        if (level < emptyThreshold || level > fullThreshold)
        {
            fillImage.color = dangerColour;
        }
        else
        {
            fillImage.color = Color.white;
        }

        level += direction * fillRate * Time.deltaTime * GameManager.inst.globalSpeed;

        levelSlider.value = level / maxLevel;

        if (level < minLevel)
        {
            dogController.Implode();
        }

        if (level > maxLevel)
        {
            dogController.Explode();
        }
    }

    public void TogglePump()
    {
        direction = -direction;
    }
}
