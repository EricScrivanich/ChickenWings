using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelPickerUIStatBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI liveText;
    [SerializeField] private GameObject ammoDisplayPrefab;
    [SerializeField] private Transform ammoDisplayParent;
    [SerializeField] private TextMeshProUGUI checkPointNumber;
    private TextMeshProUGUI[] texts;
    private Image[] images;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    public void CreateSelf(LevelPickerUIPopup p, int numb, Vector2Int current_maxLives, short[] ammoAmounts, bool shown)
    {
        List<Image> imageList = new List<Image>();
        List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();
        checkPointNumber.text = numb.ToString() + ".";
        var image = GetComponent<Image>();
        liveText.text = $"{current_maxLives.x}/{current_maxLives.y}";
        imageList.Add(image);
        textList.Add(liveText);


        for (int i = 0; i < ammoAmounts.Length; i++)
        {
            if (ammoAmounts[i] <= 0) continue;

            var obj = Instantiate(ammoDisplayPrefab, ammoDisplayParent);
            var im = obj.GetComponent<Image>();
            im.sprite = p.ammoImages[i];
            var t = obj.GetComponentInChildren<TextMeshProUGUI>();
            t.text = ammoAmounts[i].ToString();
            imageList.Add(im);
            textList.Add(t);


        }
        texts = textList.ToArray();
        images = imageList.ToArray();

        SetColor(shown);

    }

    public void CreateSelfNew(CheckPointSelection p, int numb, Vector2Int current_maxLives, short[] ammoAmounts, bool shown)
    {
        List<Image> imageList = new List<Image>();
        List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();
        checkPointNumber.text = numb.ToString() + ".";
        var image = GetComponent<Image>();
        liveText.text = $"{current_maxLives.x}/{current_maxLives.y}";
        imageList.Add(image);
        textList.Add(liveText);


        for (int i = 0; i < ammoAmounts.Length; i++)
        {
            if (ammoAmounts[i] <= 0) continue;

            var obj = Instantiate(ammoDisplayPrefab, ammoDisplayParent);
            var im = obj.GetComponent<Image>();
            im.sprite = p.ammoImages[i];
            var t = obj.GetComponentInChildren<TextMeshProUGUI>();
            t.text = ammoAmounts[i].ToString();
            imageList.Add(im);
            textList.Add(t);


        }
        texts = textList.ToArray();
        images = imageList.ToArray();

        SetColor(shown);

    }

    public void SetColor(bool shown)
    {
        Color color = shown ? Color.white : Color.white * 0.65f;
        foreach (var image in images)
        {
            image.color = color;
        }
        foreach (var text in texts)
        {
            text.color = color;
        }
        if (checkPointNumber != null)
        {
            checkPointNumber.color = color;
        }
    }
}
