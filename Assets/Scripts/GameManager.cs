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

    public PopupManager popupManager;
    public float minPopupInterval = 10f, maxPopupInterval = 20f;
    float popupTimer;

    public float minMinigameInterval = 25f, maxMinigameInterval = 40f;
    public GameObject[] minigames;
    GameObject currentMinigame;
    float minigameTimer;
    public Transform minigameParent;
    public Transform panelParent;
    public Vector2 minPos, maxPos;

    public float minRandomInterval = 15f, maxRandomInterval = 25f;
    float randomTimer;

    public CameraShake cameraShake;

    public Text deadText;
    public CanvasGroup deadGroup;

    public Text timerText;
    float timer;

    public static GameManager inst;

    bool running = true;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);

        ResetTimers();
    }

    private void Update()
    {
        if (!running) return;

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
            SoundEffectManager.inst.PlaySound("Alarm", 1f);

            virusManager.ActivateVirus();
            virusTimer = Random.Range(minVirusInterval, maxVirusInterval);

            cameraShake.trauma = 0.4f;
        }
        virusTimer -= globalSpeed * Time.deltaTime;

        if (minigameTimer <= 0)
        {
            NextMinigame();
            minigameTimer = 1000f;
        }
        minigameTimer -= globalSpeed * Time.deltaTime;
        
        if (randomTimer <= 0)
        {
            RandomEvent();
            randomTimer = Random.Range(minRandomInterval, maxRandomInterval);
        }
        randomTimer -= globalSpeed * Time.deltaTime;
        
        if (popupTimer <= 0)
        {
            popupManager.CreatePopup();
            popupTimer = Random.Range(minPopupInterval, maxPopupInterval);
        }
        popupTimer -= globalSpeed * Time.deltaTime;

        globalSpeed += globalSpeedIncreaseRate * Time.deltaTime;

        timer += Time.deltaTime;
        timerText.text = string.Format("{0}:{1}", ((int)timer/60).ToString("D1"), ((int)timer % 60).ToString("D2"));
    }

    void ResetTimers()
    {
        virusTimer = Random.Range(minVirusInterval, maxVirusInterval);
        breakTimer = Random.Range(minBreakInterval, maxBreakInterval);
        minigameTimer = Random.Range(minMinigameInterval, maxMinigameInterval);
        randomTimer = Random.Range(minRandomInterval, maxRandomInterval);
        popupTimer = Random.Range(minPopupInterval, maxPopupInterval);

        for (int i = 0; i < panelParent.childCount; i++)
        {
            Vector2 newPos = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
            panelParent.transform.GetChild(i).transform.position = newPos;
        }
    }

    public void BreakCrushers()
    {
        if (crushers) return;

        crushersFixed = false;
        fixCrusherImage.color = brokenColour;

        cameraShake.trauma = 0.4f;

        SoundEffectManager.inst.PlaySound("Explosion", 1f);
    }

    public void FixCrushers()
    {
        crushersFixed = true;
        fixCrusherImage.color = Color.white;
    }

    public void BreakDoors()
    {
        if (doors) return;

        doorsFixed = false;
        fixDoorImage.color = brokenColour;

        cameraShake.trauma = 0.4f;

        SoundEffectManager.inst.PlaySound("Explosion", 1f);
    }

    public void FixDoors()
    {
        doorsFixed = true;
        fixDoorImage.color = Color.white;
    }

    public void BreakPlatforms()
    {
        if (platforms) return;

        platformsFixed = false;
        fixPlatformImage.color = brokenColour;

        cameraShake.trauma = 0.4f;

        SoundEffectManager.inst.PlaySound("Explosion", 1f);
    }

    public void FixPlatforms()
    {
        platformsFixed = true;
        fixPlatformImage.color = Color.white;
    }

    public void MinigameWon()
    {
        Destroy(currentMinigame);

        minigameTimer = Random.Range(minMinigameInterval, maxMinigameInterval);
    }

    public void MinigameLost()
    {
        Destroy(currentMinigame);
        RandomEvent();

        minigameTimer = Random.Range(minMinigameInterval, maxMinigameInterval);
    }

    public void NextMinigame()
    {
        currentMinigame = Instantiate(minigames[Random.Range(0, 2)], minigameParent);

        currentMinigame.transform.position = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
    }

    public void RandomEvent()
    {
        int eventIndex = Random.Range(0, 3);

        if (eventIndex == 0)
        {
            SoundEffectManager.inst.PlaySound("Alarm", 1f);

            virusManager.ActivateVirus();
            virusTimer += minVirusInterval * 0.5f / globalSpeed;
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
            breakTimer += minBreakInterval * 0.5f / globalSpeed;
        }
        else if (eventIndex == 2)
        {
            SoundEffectManager.inst.PlaySound("Alarm", 1f);
            for (int i = 0; i < panelParent.childCount; i++)
            {
                Vector2 newPos = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
                panelParent.transform.GetChild(i).transform.position = newPos;
            }
            randomTimer += minRandomInterval * 0.5f / globalSpeed;
        }

        cameraShake.trauma = 0.4f;
    }

    public void Die(string text)
    {
        deadText.text = text;
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        running = false;
        MinigameWon();
        globalSpeed = 0f;

        yield return new WaitForSeconds(1.5f);

        deadGroup.gameObject.SetActive(true);

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            deadGroup.alpha = t * 2;
            yield return null;
        }
        deadGroup.alpha = 1f;

        MusicController.inst.Died();
    }

    private void OnDestroy()
    {
        inst = null;
    }
}
