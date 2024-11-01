using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonColorManager : MonoBehaviour
{
    private string savePath;

    [SerializeField] private bool inMainMenu;
    [SerializeField] private ButtonColorsSO mainColorSO;

    [SerializeField] private Image[] saveResetImages;
    [SerializeField] private Image[] saveResetOutlineImages;
    [SerializeField] private TextMeshProUGUI[] saveResetOutlineText;
    [SerializeField] private Button[] saveResetButtons;

    [SerializeField] private Color nomralSaveResetTextOutlineColor;
    [SerializeField] private Color nomralSaveResetFillColor;
    [SerializeField] private Color disabledSaveResetTextOutlineColor;
    [SerializeField] private Color disabledSaveResetFillColor;






    [SerializeField] private SpecialStateInputSystem inputSystem;
    public static Action<int, bool> OnSetSaveResetButtonPressable;
    public static Action<bool> OnSaveCurrentSelectedColors;
    public static Action OnSetDefault;
    public static Action OnHideSaveDefaultButtons;


    public bool checkSave { get; private set; }

    [SerializeField] private ButtonColorsSO tempColorSO;

    private int tempN;
    private int tempO;
    private int tempD;

    private int initialN;
    private int initialO;
    private int initialD;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "buttonColors.json");
        checkSave = false;

        if (!inMainMenu)
        {
            LoadButtonColors(mainColorSO);
            return;
        }


        if (File.Exists(savePath))
        {
            // Read the JSON file
            string json = File.ReadAllText(savePath);

            // Deserialize the JSON data into ButtonColorData
            ButtonColorData colorData = JsonUtility.FromJson<ButtonColorData>(json);

            // Load the colors into temporary variables or your colorSO scriptable object
            tempN = colorData.normalButtonColor;
            tempO = colorData.outlineColor;
            tempD = colorData.disabledButtonColor;
            initialN = colorData.normalButtonColor;
            initialO = colorData.outlineColor;
            initialD = colorData.disabledButtonColor;

            if (tempColorSO != null)
                // Load the options into tempColorSO or whatever object manages the colors
                tempColorSO.LoadOptions(colorData.normalButtonColor, colorData.outlineColor, colorData.disabledButtonColor);
        }
        else
        {
            // If the file doesn't exist, set default values
            tempN = 0; // Or any default value
            tempO = 0; // Or any default value
            tempD = 3; // Or any default value
            initialN = 0;
            initialO = 0;
            initialD = 3;

            // Optionally, you can still call the LoadOptions method with default values
            tempColorSO.LoadOptions(tempN, tempO, tempD);

            // You might want to log this for debugging purposes
            Debug.LogWarning("Save data not found, reverting to default values.");
        }
        if (tempColorSO != null)
        {
            tempColorSO.currentO = tempO;
            tempColorSO.currentN = tempN;
            tempColorSO.currentD = tempD;

        }

    }


    private void Start()
    {

        OnChangeCheckSave(0, false);


        // Check if the file exists

    }

    public void DontSaveColors()
    {
        ButtonColorManager.OnSaveCurrentSelectedColors?.Invoke(false);
        OnChangeCheckSave(0, false);

    }



    public void SaveButtonColors()
    {


        ButtonColorData colorData = new ButtonColorData
        {
            outlineColor = tempO,
            normalButtonColor = tempN,
            disabledButtonColor = tempD
        };



        string json = JsonUtility.ToJson(colorData);
        File.WriteAllText(savePath, json);
        ButtonColorManager.OnSaveCurrentSelectedColors?.Invoke(true);
        OnChangeCheckSave(0, false);
    }

    public void SetDefault()
    {
        ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(0, true);
        ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(1, false);
        ButtonColorManager.OnSetDefault?.Invoke();

    }

    public bool CheckIfColorsMatch(int type, int ind)
    {
        if (type == 1)
        {
            if (ind == tempD)
            {
                ButtonColorManager.OnHideSaveDefaultButtons?.Invoke();
                return false;
            }
            else return true;
        }
        else if (type == 2)
        {
            if (ind == tempN)
            {
                ButtonColorManager.OnHideSaveDefaultButtons?.Invoke();
                return false;
            }
            else return true;
        }
        else return true;
    }

    public void ShowNewColors(int o, int n, int d)
    {
        if (n > -1)
            tempN = n;
        if (o > -1)
            tempO = o;
        if (d > -1)
            tempD = d;

        // if (tempN == initialN && tempD == initialD && tempO == initialO)
        //     OnChangeCheckSave(0, false);

        // else OnChangeCheckSave(0, true);
        OnChangeCheckSave(0, true);

        TempLoadColors();
    }


    private void OnChangeCheckSave(int t, bool needsCheck)
    {
        ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(t, needsCheck);
        // ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(1, true);

        if (tempN == 0 && tempD == 3 && tempO == 0)
            ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(1, false);
        else
            ButtonColorManager.OnSetSaveResetButtonPressable?.Invoke(1, true);


        // if (needsCheck)
        // {
        //     saveResetImages[0].color = nomralSaveResetFillColor;
        //     saveResetOutlineImages[0].color = nomralSaveResetTextOutlineColor;
        //     saveResetOutlineText[0].color = nomralSaveResetTextOutlineColor;
        // }
        // else
        // {
        //     saveResetImages[0].color = disabledSaveResetFillColor;
        //     saveResetOutlineImages[0].color = disabledSaveResetTextOutlineColor;
        //     saveResetOutlineText[0].color = disabledSaveResetTextOutlineColor;

        // }


    }




    public void TempLoadColors()
    {

        tempColorSO.LoadOptions(tempN, tempO, tempD);

        inputSystem.UpdateCooldownColor();

    }

    public void LoadButtonColors(ButtonColorsSO colorSO)
    {
        Debug.LogError("Trying to load colors");
        if (File.Exists(savePath))
        {
            Debug.LogError("found data");

            string json = File.ReadAllText(savePath);
            ButtonColorData colorData = JsonUtility.FromJson<ButtonColorData>(json);

            // colorSO.normalButtonColor = colorData.normalButtonColor;
            // colorSO.OutLineColor = colorData.outlineColor;
            // colorSO.highlightButtonColor = colorData.highlightButtonColor;

            colorSO.LoadOptions(colorData.normalButtonColor, colorData.outlineColor, colorData.disabledButtonColor);

            // ApplyColorsFromSO();
        }
        else
        {
            Debug.LogError("No data found");
        }
    }

    // private void ApplyColorsFromSO()
    // {
    //     // Apply the loaded colors to the buttons, same as in Start()
    //     pauseFill.color = colorSO.normalButtonColor;
    //     pauseOutline.color = colorSO.OutLineColor;

    //     foreach (Image img in outlines)
    //     {
    //         img.color = colorSO.OutLineColor;
    //     }

    //     foreach (Image img in fills)
    //     {
    //         img.color = colorSO.normalButtonColor;
    //     }

    //     foreach (Image img in fullFills)
    //     {
    //         img.color = colorSO.normalButtonColorFull;
    //     }
    // }
}