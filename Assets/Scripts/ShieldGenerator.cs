using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldGenerator : MonoBehaviour
{
    public float patternInterval;
    public Image[] images;
    public Color[] colours;
    List<int> pattern = new List<int>();
    int currentIndex = 0;
    public int maxPatternLength = 5;

    public Slider timerBar;
    public Image fillImage;
    public float timerSpeed = 0.03f;
    float timer;

    bool canClick = true;

    bool playing = true;

    private void Start()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        timer = 1;
        pattern.Clear();

        for (int i = 0; i < 3; i++)
        {
            GenerateNextPattern();
        }

        ShowPattern();
    }

    private void Update()
    {
        timer -= Time.deltaTime * GameManager.inst.globalSpeed * timerSpeed;

        timerBar.value = timer;

        if (timer <= 0)
        {
            Lose();
        }
    }

    public void GenerateNextPattern()
    {
        pattern.Add(Random.Range(0, 4));
    }

    public void ShowPattern()
    {
        if (canClick) StartCoroutine(ShowPatternCoroutine());
    }

    IEnumerator ShowPatternCoroutine()
    {
        canClick = false;

        WaitForSeconds wait = new WaitForSeconds(patternInterval / GameManager.inst.globalSpeed);

        yield return wait;
        for (int i = 0; i < pattern.Count; i++)
        {
            if (pattern[i] == 0) SoundEffectManager.inst.PlaySound("ShieldRed", 1f);
            if (pattern[i] == 1) SoundEffectManager.inst.PlaySound("ShieldGreen", 1f);
            if (pattern[i] == 2) SoundEffectManager.inst.PlaySound("ShieldYellow", 1f);
            if (pattern[i] == 3) SoundEffectManager.inst.PlaySound("ShieldBlue", 1f);

            images[pattern[i]].color = colours[pattern[i]];
            yield return wait;
            images[pattern[i]].color = Color.white;
            yield return wait;
        }

        currentIndex = 0;

        canClick = true;
    }

    public void PressButton(int index)
    {
        if (!canClick) return;

        if (pattern[currentIndex] == index)
        {
            currentIndex++;
            if (currentIndex >= pattern.Count)
            {
                if (pattern.Count < maxPatternLength)
                {
                    GenerateNextPattern();
                    ShowPattern();
                }
                else
                {
                    Win();
                }
            }
        }
        else
        {
            Lose();
        }
    }

    void Lose()
    {
        if (!playing) return;
        playing = false;

        GameManager.inst.RandomEvent();

        fillImage.color = Color.red;
        StartCoroutine(LoseCoroutine());
    }

    void Win()
    {
        if (!playing) return;
        playing = false;

        fillImage.color = Color.green;
        StartCoroutine(WinCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1f);

        fillImage.color = Color.white;
        GameManager.inst.MinigameWon();
    }

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(1f);

        fillImage.color = Color.white;
        GameManager.inst.MinigameLost();
    }
}
