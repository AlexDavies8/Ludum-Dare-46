using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float globalSpeed = 1f;
    public float globalSpeedIncreaseRate = 0.01f;

    public float minVirusInterval = 15f, maxVirusInterval = 25f;
    public VirusManager virusManager;
    float virusTimer;

    public Color brokenColour;
    public float minBreakInterval = 15f, maxBreakInterval = 25f;
    float breakTimer;

    public Slider platformSlider;
    [HideInInspector] public bool platforms;
    [HideInInspector] public bool platformsFixed = true;
    public Image fixPlatformImage;

    public Slider crusherSlider;
    [HideInInspector] public bool crushers;
    [HideInInspector] public bool crushersFixed = true;
    public Image fixCrusherImage;

    public Slider doorSlider;
    [HideInInspector] public bool doors;
    [HideInInspector] public bool doorsFixed = true;
    public Image fixDoorImage;

    public static GameManager inst;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);

        ResetTimers();
    }

    private void Update()
    {
        platforms = platformSlider.value > 0 && platformsFixed;
        crushers = crusherSlider.value > 0 && crushersFixed;
        doors = doorSlider.value > 0 && doorsFixed;

        if (breakTimer <= 0)
        {
            int breakItem = Random.Range(0, 4);
            if (breakItem == 0)
            {
                BreakCrushers();
            }
            else if(breakItem == 1)
            {
                BreakDoors();
            }
            else
            {
                BreakPlatforms();
            }
            breakTimer = Random.Range(minBreakInterval, maxBreakInterval);
        }
        breakTimer -= globalSpeed * Time.deltaTime;

        if (virusTimer <= 0)
        {
            virusManager.ActivateVirus();
            virusTimer = Random.Range(minVirusInterval, maxVirusInterval);
        }
        virusTimer -= globalSpeed * Time.deltaTime;

        globalSpeed += globalSpeedIncreaseRate * Time.deltaTime;
    }

    void ResetTimers()
    {
        virusTimer = Random.Range(minVirusInterval, maxVirusInterval);
        breakTimer = Random.Range(minBreakInterval, maxBreakInterval);
    }

    public void BreakCrushers()
    {
        crushersFixed = false;
        fixCrusherImage.color = brokenColour;
    }

    public void FixCrushers()
    {
        crushersFixed = true;
        fixCrusherImage.color = Color.white;
    }

    public void BreakDoors()
    {
        doorsFixed = false;
        fixDoorImage.color = brokenColour;
    }

    public void FixDoors()
    {
        doorsFixed = true;
        fixDoorImage.color = Color.white;
    }

    public void BreakPlatforms()
    {
        platformsFixed = false;
        fixPlatformImage.color = brokenColour;
    }

    public void FixPlatforms()
    {
        platformsFixed = true;
        fixPlatformImage.color = Color.white;
    }

    public void RandomEvent()
    {
        int eventIndex = Random.Range(0, 2);

        if (eventIndex == 0)
        {
            virusManager.ActivateVirus();
        }
        else if (eventIndex == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                int breakItem = Random.Range(0, 4);
                if (breakItem == 0)
                {
                    BreakCrushers();
                }
                else if (breakItem == 1)
                {
                    BreakDoors();
                }
                else
                {
                    BreakPlatforms();
                }
            }
        }
    }
}
