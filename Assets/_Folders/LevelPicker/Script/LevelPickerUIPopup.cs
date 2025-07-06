using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelPickerUIPopup : MonoBehaviour
{
    [SerializeField] private Sprite[] ammoImages;


    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void ShowData(LevelData data)
    {
        levelNameText.text = data.LevelName;
        if (data.levelWorldAndNumber != Vector3Int.zero)
        {
            levelNumberText.text = $"{data.levelWorldAndNumber.x} - {data.levelWorldAndNumber.y}";

        }
        else levelNumberText.gameObject.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {

    }
}
