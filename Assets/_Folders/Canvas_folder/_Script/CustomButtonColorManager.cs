using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CustomButtonColorManager : MonoBehaviour
{
    [SerializeField] private EggAmmoDisplay eggButton;
    [SerializeField] private ButtonManager butMan;
    [SerializeField] private CustomButtonOptions options;
    [SerializeField] private ButtonColorsSO tempSO;

    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Image scopeFill;
    [SerializeField] private Image ringFill;
    private int currentOutlineInd;
    private int currentFillInd;
    private int currentDisableInd;
    private int savedOutlineInd;
    private int savedFillInd;
    private int savedDisableInd;

    private CustomButtonColor[] buttons;

    [SerializeField] private int maxLength;      // The maximum length before moving to a new row
    [SerializeField] private float spacingX;       // Spacing between buttons in the X axis
    [SerializeField] private float spacingY;       // Spacing between rows in the Y axis
    [SerializeField] private Vector2 initialSpacing; // Initial spacing for the first button

    [Header("0 = outline, 1 = fill, 2 = disable")]
    [SerializeField] private int type;

    [SerializeField] private GameObject customButtonColorPrefab; // Prefab to instantiate

    public static Action<int, int> OnSelectNewColor;

    // Start is called before the first frame update
    private void Start()
    {
        currentOutlineInd = tempSO.currentO;
        currentFillInd = tempSO.currentN;
        currentDisableInd = tempSO.currentD;

        savedOutlineInd = tempSO.currentO;
        savedFillInd = tempSO.currentN;
        savedDisableInd = tempSO.currentD;
        RectTransform parentRect = GetComponent<RectTransform>(); // Parent RectTransform (this object)

        float totalWidth = 0;
        if (maxLength == 0)
        {
            if (type == 0) maxLength = options.outlineColorOptions.Length;
            if (type > 0) maxLength = options.fillColorOptions.Length;
        }

        // Determine the number of buttons based on the type


        // Temporary RectTransform to get the button width
        RectTransform tempRect = customButtonColorPrefab.GetComponent<RectTransform>();

        for (int i = 0; i < maxLength; i++)
        {
            totalWidth += tempRect.rect.width;
            if (i < maxLength - 1) totalWidth += spacingX; // Add spacing except after the last button
        }

        // Set initial X spacing to center the buttons within the parent
        initialSpacing.x = totalWidth / 2;

        // Initial positions
        float currentX = -initialSpacing.x;
        float currentY = -initialSpacing.y;

        int currentRowButtonCount = 0; // Tracks the number of buttons in the current row

        if (type == 0)
        {
            buttons = new CustomButtonColor[options.outlineColorOptions.Length];
            for (int i = 0; i < options.outlineColorOptions.Length; i++)
            {
                if (currentRowButtonCount >= maxLength)
                {
                    // Move to the next row
                    currentX = -initialSpacing.x;
                    currentY -= (tempRect.rect.height + spacingY); // Move down by button height + spacingY
                    currentRowButtonCount = 0; // Reset button count for the new row
                }

                // Instantiate the button prefab
                GameObject obj = Instantiate(customButtonColorPrefab, parentRect);
                obj.transform.SetParent(parentRect);

                RectTransform objRect = obj.GetComponent<RectTransform>();

                // Set the position of the button
                objRect.anchoredPosition = new Vector2(currentX, currentY);

                // Increment the X position for the next button and row button count
                currentX += objRect.rect.width + spacingX;
                currentRowButtonCount++;

                // Initialize the button with the required color and type
                var s = obj.GetComponent<CustomButtonColor>();
                buttons[i] = s;
                buttons[i].Initialize(options.outlineColorOptions[i], i, type);
                if (i == currentOutlineInd) buttons[i].Selected(true);
                else buttons[i].Selected(false);
            }
        }
        else if (type == 1 || type == 2)
        {
            buttons = new CustomButtonColor[options.fillColorOptions.Length];

            for (int i = 0; i < options.fillColorOptions.Length; i++)
            {
                if (type == 2 && i == 0) continue;

                if (currentRowButtonCount >= maxLength)
                {
                    // Move to the next row
                    currentX = -initialSpacing.x;
                    currentY -= (tempRect.rect.height + spacingY); // Move down by button height + spacingY
                    currentRowButtonCount = 0; // Reset button count for the new row
                }

                // Instantiate the button prefab
                GameObject obj = Instantiate(customButtonColorPrefab, parentRect);
                obj.transform.SetParent(parentRect);

                RectTransform objRect = obj.GetComponent<RectTransform>();

                // Set the position of the button
                objRect.anchoredPosition = new Vector2(currentX, currentY);

                // Increment the X position for the next button and row button count
                currentX += objRect.rect.width + spacingX;
                currentRowButtonCount++;

                // Initialize the button with the required color and type
                Color c = new Color(options.fillColorOptions[i].r, options.fillColorOptions[i].g, options.fillColorOptions[i].b, .95f);
                var s = obj.GetComponent<CustomButtonColor>();
                buttons[i] = s;
                buttons[i].Initialize(c, i, type);

                if (i == currentFillInd && type == 1) buttons[i].Selected(true);
                else if (type == 1) buttons[i].Selected(false);

                if (i == currentDisableInd && type == 2) buttons[i].Selected(true);
                else if (type == 2) buttons[i].Selected(false);
            }
        }

        if (type == 0)
        {
            foreach (var t in texts)
            {
                t.color = tempSO.disabledButtonColorFull;
            }
        }
    }
    public void SetDefault()
    {
        AudioManager.instance.PlaySprayPaintSound();
        if (type == 0)
        {
            buttons[0].SetManually();
        }
        else if (type == 1)
        {
            buttons[0].SetManually();

        }
        else if (type == 2)
        {
            buttons[3].SetManually();

        }


    }

    public void SetSavedColors(bool saved)
    {
        if (saved)
        {
            if (type == 0)
            {
                savedOutlineInd = currentOutlineInd;

            }
            else if (type == 1)
            {
                savedFillInd = currentFillInd;

            }
            else if (type == 2)
            {
                savedDisableInd = currentDisableInd;

            }


        }
        else
        {
            if (type == 0)
            {
                buttons[savedOutlineInd].SetManually();
            }
            else if (type == 1)
            {
                buttons[savedFillInd].SetManually();

            }
            else if (type == 2)
            {
                buttons[savedDisableInd].SetManually();

            }

        }


    }

    public void SetNewColor(int t, int i)
    {
        if (type == 0)
        {
            foreach (var tt in texts)
            {
                tt.color = tempSO.disabledButtonColorFull;

            }
            if (eggButton.ReturnCurrentAmmoType() == 1)
                ringFill.color = tempSO.disabledButtonColor;
            else
                scopeFill.color = tempSO.normalButtonColor;


        }
        if (t == type)
        {
            int p = 0;
            if (type == 0)
            {
                p = currentOutlineInd;
                currentOutlineInd = i;

            }
            else if (type == 1)
            {
                p = currentFillInd;
                currentFillInd = i;

            }
            else if (type == 2)
            {
                p = currentDisableInd;
                currentDisableInd = i;

            }

            buttons[p].Selected(false);
            buttons[i].Selected(true);

        }
    }

    private void OnEnable()
    {
        CustomButtonColorManager.OnSelectNewColor += SetNewColor;
        ButtonColorManager.OnSaveCurrentSelectedColors += SetSavedColors;
        ButtonColorManager.OnSetDefault += SetDefault;
    }
    private void OnDisable()
    {
        CustomButtonColorManager.OnSelectNewColor -= SetNewColor;
        ButtonColorManager.OnSaveCurrentSelectedColors -= SetSavedColors;
        ButtonColorManager.OnSetDefault -= SetDefault;


    }
}