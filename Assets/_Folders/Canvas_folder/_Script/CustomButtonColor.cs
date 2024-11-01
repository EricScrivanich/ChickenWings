using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CustomButtonColor : MonoBehaviour
{
    [SerializeField] private bool isOutlineColor;
    [SerializeField] private Image ColorIMG;
    [SerializeField] private Image outline;
    private ButtonManager manager;
    private RectTransform rect;
    private bool isPressed = false;
    private int type;

    private int index;

    private Color colorData;

    private void Awake()
    {
        manager = GameObject.Find("Buttons").GetComponent<ButtonManager>();
        rect = GetComponent<RectTransform>();
    }
    private void Start()
    {


        // colorData = ColorIMG.color;
    }

    public void Pressed()
    {
        if (isPressed) return;

        if (manager.CheckColors(type, index))
        {
            manager.SetNewColors(index, index, index, type);
            CustomButtonColorManager.OnSelectNewColor?.Invoke(type, index);
        }
        

    }

    // public void SetColor(Color col)
    // {
    //     ColorIMG.color = col;
    // }

    public void Initialize(Color col, int ind, int t)
    {
        ColorIMG.color = col;
        index = ind;
        type = t;
    }
    public void Selected(bool isSelected)
    {
        if (isSelected)
        {
            isPressed = true;
            outline.color = Color.white;
            rect.localScale = BoundariesManager.vectorThree1 * 1.3f;
        }
        else
        {
            isPressed = false;
            outline.color = Color.black;
            rect.localScale = BoundariesManager.vectorThree1 * .95f;

        }
    }


}
