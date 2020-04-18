using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirusSweeperCell : MonoBehaviour
{
    public Text numberText;
    public Button button;
    public Image image;
    public Sprite revealedSprite;

    public Color[] colours;

    public int index;

    public VirusSweeper controller;
    
    public void SetController(VirusSweeper controller)
    {
        this.controller = controller;

        button.onClick.AddListener(Reveal);
    }

    public void Reveal()
    {
        controller.RevealCell(index);
    }

    public void SetNumber(int num)
    {
        image.sprite = revealedSprite;
        if (num == -1)
        {
            numberText.text = "X";
        }
        else if (num != 0)
        {
            numberText.text = num.ToString();
            numberText.color = colours[num-1];
        }
    }
}
