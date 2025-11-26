using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
public class SteroidUI : MonoBehaviour, INavigationUI
{

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip openMenuSound;
    [SerializeField] private AudioClip equipSound;
    [SerializeField] private AudioClip unequipSound;
    [SerializeField] private AudioClip selectSound;

    [SerializeField] private float baseAudioVolume = 1;
    [SerializeField] private float openMenuSoundVolume;

    [SerializeField] private float equipSoundVolume;
    [SerializeField] private float unequipSoundVolume;
    [SerializeField] private float selectSoundVolume;
    [SerializeField] private float selectSouundPitchOffset;


    [Header("Syringe Variables")]
    [SerializeField] private bool useUnlockTestArray;
    [SerializeField] private ushort[] unlockedSyringes;


    [SerializeField] private float syringeRotAmount;
    [SerializeField] private float syringeRotEachDuration;
    [SerializeField] private int syringeRotLoops;
    [SerializeField] private GameObject steroidVialPrefab;
    [SerializeField] private GameObject selectedVialPrefab;
    [SerializeField] private GameObject equippedVialPrefab;
    private Sequence steroidLiquidSeq;
    [SerializeField] private SteroidSO[] steroidsData;
    [SerializeField] private int slotsPerRow = 6;
    [SerializeField] private Transform vialParentTransform1;
    [SerializeField] private Transform vialParentTransform2;
    [SerializeField] private Image[] syringeLiquidImages;
    private int usedSyringeIndex = 0;
    [SerializeField] private float baseSyringeLiqWidth;
    [SerializeField] private float addedSyringeLiqWidth;
    [SerializeField] private TextMeshProUGUI steroidTitleText;
    [SerializeField] private TextMeshProUGUI steroidDescriptionText;
    [SerializeField] private TextMeshProUGUI slotCountText;
    [SerializeField] private TextMeshProUGUI unlockedAmountText;

    [SerializeField] private RectTransform syringeRect;
    [SerializeField] private RectTransform pusherTransform;

    private float basePusherX;
    [SerializeField] private float addedPusherXPerSlot;



    [SerializeField] private int maxSyringeSlots = 4;


    // [SerializeField] private GameObject unequipButton;

    private List<SteroidVial> vials;




    private List<int> equippedSteroids = new List<int>();
    private int currentSyringeSlots = 0;
    [SerializeField] private float BGFadeAmount = .7f;

    [SerializeField] private RectTransform selectedVialStartPos;
    [SerializeField] private RectTransform selectedVialEndPos;
    [SerializeField] private float vialUpDuration;
    [SerializeField] private float vialRotDuration;


    private SteroidVial currentSelectedVial;



    private int selectedIndex;

    [Header("Equiped Vial Variables")]
    [SerializeField] private RectTransform equippedVialParent;
    private List<SteroidVial> equippedVials = new List<SteroidVial>();
    [field: SerializeField] public float equippedVialScale { get; private set; }
    [field: SerializeField] public float equippedVialScaleSelected { get; private set; }
    [field: SerializeField] public Color equippedVialBGBlurColor { get; private set; }

    [SerializeField] private RectTransform[] equippedVialPositions;
    private Sequence equippedVialSeq;
    [SerializeField] private float equippedVialMoveDuration;
    [SerializeField] private float moveOtherVialDuration;
    [SerializeField] private Ease moveVialEase;


    [Header("Equip Button Variables")]
    [SerializeField] private float equipButtonSeqDuration;
    [SerializeField] private RectTransform equipButton;
    [SerializeField] private float equipButtonWidth;
    [SerializeField] private float unequipButtonWidth;
    [SerializeField] private float equipButtonScale;
    private bool equipShown = false;
    private Sequence equipButtonSeq;
    [SerializeField] private float unequipButtonScale;


    [Header("Vial Variables")]
    [field: SerializeField] public float baseVialScale { get; private set; }
    [field: SerializeField] public float selectedVialScale { get; private set; }
    [field: SerializeField] public float lockedVialScale { get; private set; }
    [field: SerializeField] public Color normalVialBGFillColor { get; private set; }
    [field: SerializeField] public Color normalVialBGOutlineColor { get; private set; }
    [field: SerializeField] public Color highlightVialBGFillColor { get; private set; }
    [field: SerializeField] public Color selectedVialBGFillColor { get; private set; }
    [field: SerializeField] public Color selectedVialBGOutlineColor { get; private set; }
    [field: SerializeField] public Color equippedVialBGOutlineColor { get; private set; }
    [field: SerializeField] public Color cannotBeEquippedVialBGOutlineColor { get; private set; }

    [field: SerializeField] public Color lockedVialBGFillColor { get; private set; }
    [field: SerializeField] public Color lockedVialBGOutlineColor { get; private set; }


    [SerializeField] private Image backGround;
    private ushort firstSelectedIndex;
    public GameObject GetFirstSelected()
    {
        return vials[firstSelectedIndex].gameObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        var r = GetComponent<RectTransform>();
        var screenHeight = Screen.height;
        r.anchoredPosition = new Vector2(r.anchoredPosition.x, screenHeight * 1.2f);





        steroidTitleText.text = "";

        GetComponent<RectTransform>().DOAnchorPosY(0, 1.3f).SetEase(Ease.OutBack).SetUpdate(true);
        backGround.DOFade(BGFadeAmount, .5f).From(0).SetEase(Ease.OutCubic).SetUpdate(true);

        HandleSlotCountText();

        // unequipButton.SetActive(false);
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
        List<ushort> unlockedList;

        if (useUnlockTestArray)
        {
            unlockedList = new List<ushort>(unlockedSyringes);

        }
        else
           unlockedList = new List<ushort>(savedData.unlockedSteroids);
        // List<ushort> unlockedList = new List<ushort>(unlockedSyringes);


        for (int i = vialParentTransform1.childCount - 1; i >= 0; i--)
        {
            Destroy(vialParentTransform1.GetChild(i).gameObject);
        }
        for (int i = vialParentTransform2.childCount - 1; i >= 0; i--)
        {
            Destroy(vialParentTransform2.GetChild(i).gameObject);
        }


        for (int i = 0; i < steroidsData.Length; i++)
        {
            bool equipped = false;
            bool unlocked = true;
            if (!unlockedList.Contains((steroidsData[i].ID)))
            {
                unlocked = false;
            }

            Transform vialParentTransform = (i < slotsPerRow) ? vialParentTransform1 : vialParentTransform2;



            var vial = Instantiate(steroidVialPrefab, this.transform);
            if (!unlocked)
                vial.transform.localScale = Vector3.one * lockedVialScale;
            else
                vial.transform.localScale = Vector3.one * baseVialScale;

            vial.transform.SetParent(vialParentTransform);
            if (unlocked && equippedList.Contains(steroidsData[i].ID))
            {
                selectedIndex = i;
                equipped = true;
                var equippedVial = Instantiate(equippedVialPrefab, equippedVialParent);
                var rect = equippedVial.GetComponent<RectTransform>();
                rect.localScale = Vector3.one * equippedVialScale;
                rect.anchoredPosition = equippedVialPositions[equippedVials.Count].anchoredPosition;
                var s = equippedVial.GetComponent<SteroidVial>();
                s.SetData(this, steroidsData[i], i, isEquippedSlot: true);
                s.SetIsEquipped(true);
                equippedVials.Add(s);
                EquipVial(true);
            }
            var vialComponent = vial.GetComponent<SteroidVial>();
            if (vialComponent != null)
            {
                vials.Add(vialComponent);
                if (!unlocked)
                {

                    vialComponent.SetData(this, steroidsData[i], i, unlocked: false);
                    // vialComponent.OnSteroidSelected += SelectVial;
                    continue;
                }
                else
                {
                    vialComponent.SetData(this, steroidsData[i], i);
                    if (equipped) vialComponent.SetIsEquipped(true);


                    // vialComponent.OnSteroidSelected += SelectVial;
                }


            }
        }

        for (int i = 0; i < vials.Count; i++)
        {
            vials[i].SetCanBeEquipped(GetAvailableSyringeSlots());
        }
        equipButton.gameObject.SetActive(false);


        selectedIndex = -1;
        firstSelectedIndex = savedData.lastEditedIndex;


    }
    void Start()
    {
        if (InputSystemSelectionManager.instance != null)
            InputSystemSelectionManager.instance.SetNewWindow(this, true);

        openMenuSoundVolume *= AudioManager.instance.SfxVolume * baseAudioVolume;

        equipSoundVolume *= AudioManager.instance.SfxVolume * baseAudioVolume;
        unequipSoundVolume *= AudioManager.instance.SfxVolume * baseAudioVolume;
        selectSoundVolume *= AudioManager.instance.SfxVolume * baseAudioVolume;

        audioSource.PlayOneShot(openMenuSound, openMenuSoundVolume);
    }

    private void HandleSlotCountText()
    {
        slotCountText.text = "(" + currentSyringeSlots.ToString() + "/" + maxSyringeSlots.ToString() + ")" + " mL.";
    }
    void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        // foreach (var v in vials)
        // {
        //     v.OnSteroidSelected -= SelectVial;
        // }
    }

    private bool canEquip = false;

    public void SelectVial(int index)
    {
        if (selectedIndex == index)
            return;

        HapticFeedbackManager.instance.PressUIButton();

        audioSource.Stop();
        audioSource.pitch = 1f + (Random.Range(-selectSouundPitchOffset, selectSouundPitchOffset));
        audioSource.PlayOneShot(selectSound, selectSoundVolume);


        if (equippedVialSeq != null && equippedVialSeq.IsActive())
        {
            equippedVialSeq.Complete();
        }


        for (int i = 0; i < vials.Count; i++)
        {
            vials[i].DoSelectAnimationUnlockedVial(index);
        }
        for (int i = 0; i < equippedVials.Count; i++)
        {
            equippedVials[i].SelectEquippedVial(index);
        }

        selectedIndex = index;
        var selectedSteroid = steroidsData[index];
        steroidTitleText.text = selectedSteroid.Title;
        bool equipped = false;

        if (equippedSteroids.Contains(index))
        {
            equipButton.GetComponent<Button>().interactable = true;
            if (InputSystemSelectionManager.instance != null)
                InputSystemSelectionManager.instance.SetNextSelected(equipButton.gameObject);
            DoEquipButtonAnimation(false);


            equipped = true;

        }
        else
        {
            DoEquipButtonAnimation(true);

            if (currentSyringeSlots + steroidsData[index].Spaces > maxSyringeSlots)
            {
                equipButton.GetComponent<Button>().interactable = false;
                canEquip = false;
            }
            else
            {
                equipButton.GetComponent<Button>().interactable = true;
                canEquip = true;

            }

        }
        if (currentSelectedVial != null)
        {
            currentSelectedVial.HideSelectedVial(false);
        }
        var o = Instantiate(selectedVialPrefab, selectedVialStartPos.position, Quaternion.identity, transform.parent);
        var s = o.GetComponent<SteroidVial>();
        s.SetData(this, selectedSteroid, index, true, selectedVialStartPos, equipped);
        s.DoSelectAnimationBig(selectedVialEndPos, vialUpDuration, vialRotDuration);
        currentSelectedVial = s;

    }

    public void SetEquipButton(bool canPress)
    {
        if (canPress)

            InputSystemSelectionManager.instance.SetNextSelected(equipButton.gameObject);

    }
    public void Exit()
    {
        HapticFeedbackManager.instance.PressUIButton();
        audioSource.pitch = 1;
        audioSource.PlayOneShot(openMenuSound, openMenuSoundVolume);

        InputSystemSelectionManager.instance.HandleGroup(0, true);

        if (GameObject.Find("Player") != null)
        {
            var player = GameObject.Find("Player").GetComponent<PlayerStateManager>();
            player.HandleSteroids();

        }
        if (steroidLiquidSeq != null && steroidLiquidSeq.IsActive())
        {
            steroidLiquidSeq.Complete();
        }
        if (currentSelectedVial != null)
        {
            currentSelectedVial.HideSelectedVial(false);
        }
        var screenHeight = Screen.height;
        steroidLiquidSeq = DOTween.Sequence();
        steroidLiquidSeq.Append(GetComponent<RectTransform>().DOAnchorPosY(screenHeight * 1.2f, .75f).SetEase(Ease.InBack));
        steroidLiquidSeq.Append(backGround.DOFade(0, .55f).SetEase(Ease.OutCubic));
        steroidLiquidSeq.Play().SetUpdate(true).OnComplete(() =>
        {
            Destroy(backGround.gameObject);
        });





    }

    public void HandleEquipButton()
    {

        if (equipShown)
        {
            if (canEquip)

                EquipVial();
            else
            {
                HapticFeedbackManager.instance.PlayerButtonFailure();
                return;
            }

        }
        else
        {
            UnEquipVial();
        }
        HapticFeedbackManager.instance.PressUIButton();
    }

    private void UnEquipVial()
    {
        DoEquipButtonAnimation(true);
        canEquip = true;
        audioSource.pitch = 1;
        audioSource.PlayOneShot(unequipSound, unequipSoundVolume);
        if (currentSelectedVial != null)
        {
            currentSelectedVial.SetEquippedBig(false);
        }
        int index = selectedIndex;
        int equippedIndex = equippedSteroids.IndexOf(index);
        var selectedSteroid = steroidsData[index];
        currentSyringeSlots -= (int)selectedSteroid.Spaces;

        if (steroidLiquidSeq != null && steroidLiquidSeq.IsActive())
        {
            steroidLiquidSeq.Complete();
        }
        if (syringeRotateSeq != null && syringeRotateSeq.IsActive())
        {
            syringeRotateSeq.Complete();
        }


        steroidLiquidSeq = DOTween.Sequence();
        int pusherCount = (equippedSteroids.Count - 1);
        if (pusherCount < 0) pusherCount = 0;

        syringeRotateSeq = DOTween.Sequence();
        float startRot = syringeRect.eulerAngles.z;
        syringeRotateSeq.Append(syringeRect.DORotate(Vector3.forward * (startRot + syringeRotAmount), syringeRotEachDuration).From(Vector3.forward * (startRot - syringeRotAmount)).SetEase(Ease.InOutSine).SetLoops(Mathf.RoundToInt(syringeRotLoops), LoopType.Yoyo));
        syringeRotateSeq.Append(syringeRect.DORotate(Vector3.forward * startRot, .15f).SetEase(Ease.OutBack));

        syringeRotateSeq.Play().SetUpdate(true);

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
        // equippedVials[equippedIndex].HideOrShowEquippedVial(true, equippedVialParent);
        HandleMoveVialForEquipped(false, equippedIndex);

        equippedSteroids.Remove(index);

        LevelDataConverter.instance.SaveSteroidData(equippedSteroids.ConvertAll(i => (ushort)steroidsData[i].ID).ToArray(), selectedIndex);
        vials[index].SetIsEquipped(false);
        for (int i = 0; i < vials.Count; i++)
        {
            vials[i].SetCanBeEquipped(GetAvailableSyringeSlots());
        }




    }

    private void HandleMoveVialForEquipped(bool equipped, int index)
    {
        if (equippedVialSeq != null && equippedVialSeq.IsActive())
        {
            equippedVialSeq.Complete();
        }
        equippedVialSeq = DOTween.Sequence();

        if (equipped)
        {
            var rect = currentSelectedVial.GetSideVialRectForEquip();
            rect.transform.parent = equippedVialParent;
            var s = rect.GetComponent<SteroidVial>();
            s.SelectEquippedVial(-1);


            equippedVialSeq.Append(rect.DOAnchorPos(equippedVialPositions[equippedVials.Count].anchoredPosition, equippedVialMoveDuration).SetEase(moveVialEase));
            equippedVialSeq.Play().SetUpdate(true);
            equippedVials.Add(s);


        }
        else
        {
            var rect = equippedVials[index].GetComponent<RectTransform>();
            Vector2 pos = currentSelectedVial.GetSideVialRectForUnEquip(rect, equippedVialMoveDuration);
            equippedVialSeq.Append(rect.DOScale(Vector3.one * 1.1f, equippedVialMoveDuration));
            equippedVialSeq.Join(rect.DOAnchorPos(pos, equippedVialMoveDuration).SetEase(moveVialEase));
            for (int i = index + 1; i < equippedVials.Count; i++)
            {

                var r = equippedVials[i].GetComponent<RectTransform>();
                equippedVialSeq.Join(r.DOAnchorPos(equippedVialPositions[i - 1].anchoredPosition, moveOtherVialDuration).SetEase(Ease.OutBack));

            }

            equippedVialSeq.Play().SetUpdate(true);





            equippedVials.RemoveAt(index);
        }

    }

    public int GetAvailableSyringeSlots()
    {
        return maxSyringeSlots - currentSyringeSlots;
    }
    private Sequence syringeRotateSeq;
    private void EquipVial(bool start = false)
    {
        DoEquipButtonAnimation(false);
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
        if (syringeRotateSeq != null && syringeRotateSeq.IsActive())
        {
            syringeRotateSeq.Complete();
        }



        steroidLiquidSeq = DOTween.Sequence();
        steroidLiquidSeq.Append(syringeLiquidImages[equippedSteroids.Count - 1].rectTransform.DOSizeDelta(new Vector2(syringeWidth, syringeLiquidImages[equippedSteroids.Count - 1].rectTransform.sizeDelta.y), 0.5f));
        steroidLiquidSeq.Join(syringeLiquidImages[equippedSteroids.Count - 1].DOColor(selectedSteroid.LiquidColor, 0.5f));
        steroidLiquidSeq.Join(pusherTransform.DOLocalMoveX(basePusherX + (pusherCount * addedPusherXPerSlot), 0.5f));

        if (!start)
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(equipSound, equipSoundVolume);

            if (currentSelectedVial != null)
            {
                currentSelectedVial.SetEquippedBig(true);
            }
            syringeRotateSeq = DOTween.Sequence();
            float startRot = syringeRect.eulerAngles.z;
            syringeRotateSeq.Append(syringeRect.DORotate(Vector3.forward * (startRot + syringeRotAmount), syringeRotEachDuration).From(Vector3.forward * (startRot - syringeRotAmount)).SetEase(Ease.InOutSine).SetLoops(Mathf.RoundToInt(syringeRotLoops), LoopType.Yoyo));
            syringeRotateSeq.Append(syringeRect.DORotate(Vector3.forward * startRot, .15f).SetEase(Ease.OutBack));

            syringeRotateSeq.Play().SetUpdate(true);
            LevelDataConverter.instance.SaveSteroidData(equippedSteroids.ConvertAll(i => (ushort)steroidsData[i].ID).ToArray(), selectedIndex);
            // var equippedVial = Instantiate(equippedVialPrefab, equippedVialParent);
            // equippedVial.transform.localScale = new Vector3(0, 1, 1);
            // var s = equippedVial.GetComponent<SteroidVial>();
            // s.SetData(this, selectedSteroid, index, isEquippedSlot: true);
            // s.HideOrShowEquippedVial(false, equippedVialParent);
            // equippedVials.Add(s);
            HandleMoveVialForEquipped(true, index);
            vials[index].SetIsEquipped(true);
            for (int i = 0; i < vials.Count; i++)
            {
                vials[i].SetCanBeEquipped(GetAvailableSyringeSlots());
            }
        }



        // pusherTransform.DOAnchorPosX(basePusherX - (currentSyringeSlots * addedSyringeLiqWidth), 0.5f);
        steroidLiquidSeq.Play().SetEase(Ease.InOutSine).SetUpdate(true);


    }

    public void DoEquipButtonAnimation(bool equip)
    {
        if (!equipButton.gameObject.activeInHierarchy)
            equipButton.gameObject.SetActive(true);

        else if (equipShown == equip) return;

        equipShown = equip;

        if (equipButtonSeq != null && equipButtonSeq.IsActive())
        {
            equipButtonSeq.Complete();
        }

        equipButtonSeq = DOTween.Sequence();
        if (!equip)
        {
            equipButtonSeq.Append(equipButton.DOSizeDelta(new Vector2(unequipButtonWidth, equipButton.sizeDelta.y), equipButtonSeqDuration));
            equipButtonSeq.Join(equipButton.DOScale(unequipButtonScale, equipButtonSeqDuration).SetEase(Ease.OutBack));
        }
        else
        {
            equipButtonSeq.Append(equipButton.DOSizeDelta(new Vector2(equipButtonWidth, equipButton.sizeDelta.y), equipButtonSeqDuration));
            equipButtonSeq.Join(equipButton.DOScale(equipButtonScale, equipButtonSeqDuration).SetEase(Ease.OutBack));
        }
        equipButtonSeq.Play().SetUpdate(true);

    }

    public GameObject GetNextSelected()
    {
        throw new System.NotImplementedException();
    }
}



