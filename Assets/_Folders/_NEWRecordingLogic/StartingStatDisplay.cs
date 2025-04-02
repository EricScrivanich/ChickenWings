using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StartingStatDisplay : MonoBehaviour, IPointerDownHandler
{
    private PlayEditor playEditor;
    private Image img;
    [SerializeField] private Image eggImage;
    public TextMeshProUGUI text;

    [SerializeField] private float normalScale;
    [SerializeField] private float pressScale;
    [SerializeField] private LevelCreatorColors colorSO;

    [SerializeField] private int type;

    private int currentAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        currentAmount = LevelRecordManager.instance.levelData.startingStats.startingAmmos[type];
        text.text = currentAmount.ToString();

        // if (type == 0)
        // {
        //     // StartCoroutine(Delay());
        //     transform.localScale = Vector3.one * pressScale;
        //     img.color = colorSO.SelctedUIColor;

        // }
        // else
        // {
        //     transform.localScale = Vector3.one * normalScale;
        //     img.color = colorSO.MainUIColor;
        // }



    }
    private void Awake()
    {
        img = GetComponent<Image>();
    }
    private IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (playEditor == null) yield break;

        playEditor.OnSelectAmmo(type, eggImage.sprite, text);
    }

    public void SetPlayEditor(PlayEditor playEditor)
    {
        this.playEditor = playEditor;

        if (type == 0)
        {
            transform.localScale = Vector3.one * pressScale;
            img.color = colorSO.SelctedUIColor;
            playEditor.OnSelectAmmo(type, eggImage.sprite, text);

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playEditor == null) return;
        transform.localScale = Vector3.one * pressScale;
        img.color = colorSO.SelctedUIColor;
        playEditor.OnSelectAmmo(type, eggImage.sprite, text);

    }

    public void SetNewEgg(int nextType)
    {
        if (type != nextType)
        {
            transform.localScale = Vector3.one * normalScale;
            img.color = colorSO.MainUIColor;

        }
    }

    // Update is called once per frame

}
