
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartingStatEditor : MonoBehaviour
{
    // public enum DataType
    // {
    //     Life,


    // }

    public int ammoType;



    // [SerializeField] private DataType dataType;

    private int currentAmount;
    [SerializeField] private Image img;
    [SerializeField] private int minAmount;
    [SerializeField] private int maxAmount;

    [SerializeField] private TextMeshProUGUI textMain;
    public TextMeshProUGUI addedText;

    [SerializeField] private PlusMinusUI plus;
    [SerializeField] private PlusMinusUI minus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (ammoType >= 0)
        // {
        //     Debug.Log("Staring stats is: " + LevelRecordManager.instance.levelData.startingStats.name);
        //     currentAmount = LevelRecordManager.instance.levelData.startingStats.startingAmmos[ammoType];
        //     textMain.text = currentAmount.ToString();
        // }

        if (ammoType == -1)
        {
            currentAmount = LevelRecordManager.instance.levelData.startingStats.StartingLives;
            textMain.text = currentAmount.ToString();


        }

        plus.SetData(this);
        minus.SetData(this);
    }

    public void SetNewEgg(int type, Sprite img, TextMeshProUGUI text)
    {
        Debug.Log("Setting new egg");
        ammoType = type;
        addedText = text;
        this.img.sprite = img;
        currentAmount = LevelRecordManager.instance.levelData.startingStats.startingAmmos[ammoType];
        textMain.text = currentAmount.ToString();
    }




    // Update is called once per frame


    public void OnEdit(int add)
    {

        currentAmount += add;
        if (currentAmount > maxAmount) currentAmount = maxAmount;
        else if (currentAmount < minAmount) currentAmount = minAmount;
        if (ammoType == -1)
        {
            LevelRecordManager.instance.levelData.startingStats.StartingLives = (short)currentAmount;
            textMain.text = currentAmount.ToString();

        }
        else
        {
            LevelRecordManager.instance.levelData.startingStats.startingAmmos[ammoType] = (short)currentAmount;
            textMain.text = currentAmount.ToString();
            addedText.text = currentAmount.ToString();
        }

        // text.text = currentAmount.ToString();

    }
}
