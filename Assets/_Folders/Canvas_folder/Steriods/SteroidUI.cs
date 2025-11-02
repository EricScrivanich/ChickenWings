using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
public class SteroidUI : MonoBehaviour
{
    [SerializeField] private GameObject steroidVialPrefab;
    private Sequence steroidLiquidSeq;
    [SerializeField] private SteroidSO[] steroidsData;
    [SerializeField] private Transform vialParentTransform;
    [SerializeField] private Image[] syringeLiquidImages;
    private int usedSyringeIndex = 0;
    [SerializeField] private float baseSyringeLiqWidth;
    [SerializeField] private float addedSyringeLiqWidth;
    [SerializeField] private TextMeshProUGUI steroidTitleText;
    [SerializeField] private TextMeshProUGUI steroidDescriptionText;
    [SerializeField] private TextMeshProUGUI slotCountText;
    [SerializeField] private TextMeshProUGUI unlockedAmountText;

    [SerializeField] private RectTransform pusherTransform;

    private float basePusherX;
    [SerializeField] private float addedPusherXPerSlot;



    [SerializeField] private int maxSyringeSlots = 4;

    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject unequipButton;

    private List<SteroidVial> vials;

    [SerializeField] private float baseVialScale;
    [SerializeField] private float selectedVialScale;


    private List<int> equippedSteroids = new List<int>();
    private int currentSyringeSlots = 0;
    [SerializeField] private float BGFadeAmount = .7f;
    [SerializeField] private GameObject selectedVialPrefab;
    [SerializeField] private RectTransform selectedVialStartPos;
    [SerializeField] private RectTransform selectedVialEndPos;
    [SerializeField] private float vialUpDuration;
    [SerializeField] private float vialRotDuration;
    [SerializeField] private RectTransform equppedVialParent;
    [SerializeField] private List<SteroidVial> equippedVials = new List<SteroidVial>();

    private SteroidVial currentEquippedVial;



    private int selectedIndex;


    [SerializeField] private Image backGround;
    void Awake()
    {
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 1200);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {




        steroidTitleText.text = "";

        GetComponent<RectTransform>().DOAnchorPosY(0, 1.3f).SetEase(Ease.OutBack).SetUpdate(true);
        backGround.DOFade(BGFadeAmount, .5f).From(0).SetEase(Ease.OutCubic).SetUpdate(true);

        HandleSlotCountText();
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        basePusherX = pusherTransform.localPosition.x;
        vials = new List<SteroidVial>();

        foreach (var img in syringeLiquidImages)
        {
            Debug.Log("Initializing syringe liquid image");
            img.rectTransform.sizeDelta = new Vector2(0, img.rectTransform.sizeDelta.y);
            img.color = new Color(img.color.r, img.color.g, img.color.b, .3f);

        }

        var savedData = LevelDataConverter.instance.LoadSteroidData();
        if (savedData == null)
        {
            Debug.LogError("No saved steroid data found.");
            return;
        }

        unlockedAmountText.text = "(" + savedData.unlockedSteroids.Length.ToString() + "/" + steroidsData.Length.ToString() + ")";
        List<ushort> equippedList = new List<ushort>(savedData.equippedSteroids);
        List<ushort> unlockedList = new List<ushort>(savedData.unlockedSteroids);
        for (int i = vialParentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(vialParentTransform.GetChild(i).gameObject);
        }

        for (int i = 0; i < steroidsData.Length; i++)
        {
            if (!unlockedList.Contains((steroidsData[i].ID)))
            {
                continue;
            }


            var vial = Instantiate(steroidVialPrefab, vialParentTransform);
            vial.transform.localScale = Vector3.one * baseVialScale;
            if (equippedList.Contains(steroidsData[i].ID))
            {
                selectedIndex = i;
                var equippedVial = Instantiate(steroidVialPrefab, equppedVialParent);
                equippedVial.transform.localScale = Vector3.one;
                var s = equippedVial.GetComponent<SteroidVial>();
                s.SetData(steroidsData[i], i);
                equippedVials.Add(s);
                EquipVial(true);
            }
            var vialComponent = vial.GetComponent<SteroidVial>();
            if (vialComponent != null)
            {
                vials.Add(vialComponent);
                vialComponent.SetData(steroidsData[i], i);


                vialComponent.OnSteroidSelected += SelectVial;
            }
        }
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);

        selectedIndex = -1;











    }

    private void HandleSlotCountText()
    {
        slotCountText.text = "(" + currentSyringeSlots.ToString() + "/" + maxSyringeSlots.ToString() + ")" + " mL.";
    }
    void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        foreach (var v in vials)
        {
            v.OnSteroidSelected -= SelectVial;
        }
    }

    void SelectVial(int index)
    {
        if (selectedIndex == index)
            return;

        if (selectedIndex != -1)
        {
            vials[selectedIndex].transform.localScale = Vector3.one * baseVialScale;
        }

        vials[index].transform.localScale = Vector3.one * selectedVialScale;
        selectedIndex = index;
        var selectedSteroid = steroidsData[index];
        steroidTitleText.text = selectedSteroid.Title;

        if (equippedSteroids.Contains(index))
        {
            equipButton.SetActive(false);
            unequipButton.SetActive(true);

        }
        else
        {
            equipButton.SetActive(true);
            unequipButton.SetActive(false);

            if (currentSyringeSlots + steroidsData[index].Spaces > maxSyringeSlots)
            {
                equipButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                equipButton.GetComponent<Button>().interactable = true;
            }

        }
        if (currentEquippedVial != null)
        {
            currentEquippedVial.HideSelectedVial(false);
        }
        var o = Instantiate(selectedVialPrefab, selectedVialStartPos.position, Quaternion.identity, transform.parent);
        var s = o.GetComponent<SteroidVial>();
        s.SetData(selectedSteroid, index, true, selectedVialStartPos);
        s.DoSelectAnimation(selectedVialEndPos, vialUpDuration, vialRotDuration);
        currentEquippedVial = s;

    }
    public void Exit()
    {

        if (GameObject.Find("Player") != null)
        {
            var player = GameObject.Find("Player").GetComponent<PlayerStateManager>();
            player.HandleSteroids();

        }
        if (steroidLiquidSeq != null && steroidLiquidSeq.IsActive())
        {
            steroidLiquidSeq.Complete();
        }

        backGround.DOFade(0, .4f).SetEase(Ease.OutCubic).SetUpdate(true);

        GetComponent<RectTransform>().DOAnchorPosY(1200, .6f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            Destroy(backGround.gameObject);
        });

    }

    public void UnEquipVial()
    {
        equipButton.SetActive(true);
        unequipButton.SetActive(false);
        int index = selectedIndex;
        int equippedIndex = equippedSteroids.IndexOf(index);
        var selectedSteroid = steroidsData[index];
        currentSyringeSlots -= (int)selectedSteroid.Spaces;

        if (steroidLiquidSeq != null && steroidLiquidSeq.IsActive())
        {
            steroidLiquidSeq.Complete();
        }

        steroidLiquidSeq = DOTween.Sequence();
        int pusherCount = (equippedSteroids.Count - 1);
        if (pusherCount < 0) pusherCount = 0;

        // for (int i = equippedIndex; i < equippedSteroids.Count; i++)
        for (int i = equippedSteroids.Count - 1; i >= equippedIndex; i--)
        {
            float subtractedWidth = 0;
            if (i == 0) subtractedWidth = baseSyringeLiqWidth;
            var l = syringeLiquidImages[i];

            if (i == equippedSteroids.Count - 1)
            {
                steroidLiquidSeq.Join(l.rectTransform.DOSizeDelta(new Vector2(0, l.rectTransform.sizeDelta.y), 0.5f));
                steroidLiquidSeq.Join(l.DOColor(Color.clear, 0.4f));
                if (i == equippedIndex) steroidLiquidSeq.Join(pusherTransform.DOLocalMoveX(basePusherX + (pusherCount * addedPusherXPerSlot), 0.5f));



            }
            else if (i == equippedIndex)
            {
                steroidLiquidSeq.Join(l.rectTransform.DOSizeDelta(new Vector2(syringeLiquidImages[i + 1].rectTransform.sizeDelta.x - subtractedWidth, l.rectTransform.sizeDelta.y), 0.5f));
                steroidLiquidSeq.Join(l.DOColor(syringeLiquidImages[i + 1].color, 0.5f));
                steroidLiquidSeq.Join(pusherTransform.DOLocalMoveX(basePusherX + (pusherCount * addedPusherXPerSlot), 0.5f));


            }
            else
            {
                steroidLiquidSeq.Join(l.rectTransform.DOSizeDelta(new Vector2(syringeLiquidImages[i + 1].rectTransform.sizeDelta.x - subtractedWidth, l.rectTransform.sizeDelta.y), 0.5f));
                steroidLiquidSeq.Join(l.DOColor(syringeLiquidImages[i + 1].color, 0.5f));

            }
        }
        HandleSlotCountText();

        steroidLiquidSeq.Play().SetUpdate(true);
        equippedVials[equippedIndex].HideOrShowEquippedVial(true, equppedVialParent);
        equippedVials.RemoveAt(equippedIndex);
        equippedSteroids.Remove(index);

        LevelDataConverter.instance.SaveSteroidData(equippedSteroids.ConvertAll(i => (ushort)steroidsData[i].ID).ToArray());




    }
    public void EquipVial(bool start = false)
    {
        equipButton.SetActive(false);
        unequipButton.SetActive(true);
        int index = selectedIndex;
        var selectedSteroid = steroidsData[index];
        currentSyringeSlots += (int)selectedSteroid.Spaces;
        usedSyringeIndex++;
        equippedSteroids.Add(index);
        int pusherCount = (equippedSteroids.Count - 1);
        if (pusherCount < 0) pusherCount = 0;
        float syringeWidth = (selectedSteroid.Spaces * addedSyringeLiqWidth) + 5;
        HandleSlotCountText();

        if (equippedSteroids.Count == 1)
        {
            syringeWidth -= baseSyringeLiqWidth;
        }
        if (steroidLiquidSeq != null && steroidLiquidSeq.IsActive())
        {
            steroidLiquidSeq.Complete();
        }

        steroidLiquidSeq = DOTween.Sequence();
        steroidLiquidSeq.Append(syringeLiquidImages[equippedSteroids.Count - 1].rectTransform.DOSizeDelta(new Vector2(syringeWidth, syringeLiquidImages[equippedSteroids.Count - 1].rectTransform.sizeDelta.y), 0.5f));
        steroidLiquidSeq.Join(syringeLiquidImages[equippedSteroids.Count - 1].DOColor(selectedSteroid.LiquidColor, 0.5f));
        steroidLiquidSeq.Join(pusherTransform.DOLocalMoveX(basePusherX + (pusherCount * addedPusherXPerSlot), 0.5f));

        if (!start)
        {
            LevelDataConverter.instance.SaveSteroidData(equippedSteroids.ConvertAll(i => (ushort)steroidsData[i].ID).ToArray());
            var equippedVial = Instantiate(steroidVialPrefab, equppedVialParent);
            equippedVial.transform.localScale = new Vector3(0, 1, 1);
            var s = equippedVial.GetComponent<SteroidVial>();
            s.SetData(selectedSteroid, index);
            s.HideOrShowEquippedVial(false, equppedVialParent);
            equippedVials.Add(s);
        }



        // pusherTransform.DOAnchorPosX(basePusherX - (currentSyringeSlots * addedSyringeLiqWidth), 0.5f);
        steroidLiquidSeq.Play().SetEase(Ease.InOutSine).SetUpdate(true);


    }




}



