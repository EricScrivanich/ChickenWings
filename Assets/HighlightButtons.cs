using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightButtons : MonoBehaviour
{
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color normalColor;
    public PlayerID player;
    [SerializeField] private Button dashButton;

    [SerializeField] private Button dropButton;

    [SerializeField] private Button eggButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HighlightDash(bool isPressed)
    {
        Debug.Log("Pressed");
        if (isPressed)
        {
            HighlightButton(dashButton);
        }
        else
        {
            ResetButtonColor(dashButton);
        }

    }
    private void HighlightDrop(bool isPressed)
    {
        if (isPressed)
        {
            HighlightButton(dropButton);
        }
        else
        {
            ResetButtonColor(dropButton);
        }

    }
    private void HighlightEgg(bool isPressed)
    {
        if (isPressed)
        {
            HighlightButton(eggButton);
        }
        else
        {
            ResetButtonColor(eggButton);
        }

    }

    void HighlightButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlightedColor; // Change normal color to pressed color
        button.colors = colors;
    }

    void ResetButtonColor(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor; // Change normal color to pressed color
        button.colors = colors;
    }

    private void OnEnable()
    {
        player.globalEvents.HighlightDash += HighlightDash;
        player.globalEvents.HighlightDrop += HighlightDrop;
        player.globalEvents.HighlightEgg += HighlightEgg;


    }
    private void OnDisable()
    {
        player.globalEvents.HighlightDash -= HighlightDash;
        player.globalEvents.HighlightDrop -= HighlightDrop;
        player.globalEvents.HighlightEgg -= HighlightEgg;

    }
}
