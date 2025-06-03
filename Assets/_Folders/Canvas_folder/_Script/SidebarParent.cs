using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class SidebarParent : MonoBehaviour
{

    [SerializeField] private PlayerID player;
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private Transform parentMask;
    [SerializeField] private bool inCustomizeWindow = false;


    private Sequence seq;
    private Sequence rotateSidebarSeq;

    private bool tweeningEggs = false;
    private int currentEquippedEgg;


    private Vector2 fullSize; // Full size of the sidebar
    private Vector2 buttonShownSize; // Sidebar size when button is shown

    [SerializeField] private Vector2 hiddenSizeDelta;

    [Header("Sidebar Settings")]
    [SerializeField] private Vector2 startEndScale;
    [SerializeField] private Vector2 fullSizePos; // Position of the sidebar when fully visible
    [SerializeField] private Vector2 buttonShownPos; // Position of the sidebar when the button is shown

    [SerializeField] private Vector2 cageEquippedPosition;
    [SerializeField] private int baseWidth; // Base width of the sidebar
    [SerializeField] private int basePosition; // Base position (X) for the sidebar
    [SerializeField] private float baseYPosition; // Base position (X) for the sidebar
    [SerializeField] private int addedWidthPerAmmo; // Additional width per ammo type
    [SerializeField] private float showButtonDuration;
    [SerializeField] private float hideButtonDuration;
    private Image mainImage;

    [Header("Ammo Settings")]
    [SerializeField] private Sprite[] ammoImages; // Sprites for the different ammo types
    [SerializeField] private GameObject sidebarAmmoPrefab; // Prefab for sidebar ammo items

    private Vector2[] positions; // Positions for the ammo rects
    private Vector2 endRightPosition;
    private RectTransform[] ammoRects; // RectTransforms for ammo objects
    private int[] eggOrderByType; // Order of eggs by type (used for sorting)
    [SerializeField] private Ease equipEase;
    [SerializeField] private float duration;

    private RectTransform mainRect;
    [SerializeField] private Transform UnequippedParentPostionOnAmmoSwitch;
    private Vector3 mainRectStartPos;
    [SerializeField] private float startAmmoSwitchTweenDuration = 0.5f;
    [SerializeField] private float endAmmoSwitchTweenDuration = 0.5f;
    private Button button;


    private void OnEnable()
    {

        player.UiEvents.OnSendShownSidebarAmmos += Initialize;
        player.UiEvents.OnSwitchDisplayedWeapon += SwitchAmmo;
        player.UiEvents.OnResetSidebarAmmos += SetPositions;

    }
    private void OnDisable()
    {
        player.UiEvents.OnSendShownSidebarAmmos -= Initialize;
        player.UiEvents.OnSwitchDisplayedWeapon -= SwitchAmmo;
        player.UiEvents.OnResetSidebarAmmos -= SetPositions;
        if (showSeq != null && showSeq.IsPlaying()) showSeq.Kill();

    }
    public void Initialize(int[] eggTypes, int startEgg)
    {
        Debug.Log("Initializing SidebarParent with egg types: " + string.Join(", ", eggTypes));
        mainRect = GetComponent<RectTransform>();
        mainImage = GetComponent<Image>();
        button = GetComponent<Button>();
        mainRectStartPos = mainRect.localPosition;

        // Initialize arrays
        eggOrderByType = new int[eggTypes.Length];
        ammoRects = new RectTransform[eggTypes.Length];
        positions = new Vector2[eggTypes.Length];

        // Calculate the full size and button shown size of the sidebar
        fullSize = new Vector2(baseWidth + (eggTypes.Length * addedWidthPerAmmo), mainRect.sizeDelta.y);
        buttonShownSize = new Vector2(baseWidth + ((eggTypes.Length - 1) * addedWidthPerAmmo), mainRect.sizeDelta.y);

        // Set the size and position of the main sidebar rect
        mainRect.sizeDelta = buttonShownSize;
        // mainRect.localPosition = fullSizePos;
        // mainRect.localScale = BoundariesManager.vectorThree1 * startEndScale.y;

        // Initialize egg order, positions, and ammoRects
        for (int i = 0; i < eggTypes.Length; i++)
        {
            int type = eggTypes[i];
            eggOrderByType[i] = type;


            // Calculate position for each ammo rect
            positions[i] = new Vector2(basePosition - ((eggTypes.Length - 1 - i) * addedWidthPerAmmo), baseYPosition);

            // Debug.LogError("Positions of: " + i + " is: " + positions[i]);

            // Instantiate the sidebar ammo prefab and initialize it
            GameObject ammoObject = Instantiate(sidebarAmmoPrefab, parentMask);
            ammoObject.GetComponent<SidebarAmmo>().Initialize(ammoImages[type], type);
            if (type == 1 && inCustomizeWindow)
            {
                ammoObject.GetComponent<SidebarAmmo>().SendText();
            }


            // Store the RectTransform for future use
            ammoRects[i] = ammoObject.GetComponent<RectTransform>();
        }

        endRightPosition = new Vector2(basePosition + addedWidthPerAmmo, baseYPosition);

        // Set positions for the first time
        SetPositions(startEgg);
    }

    public void OnPress()
    {
        // player.UiEvents.OnSwitchWeapon?.Invoke(0, -1);
        // player.UiEvents.OnPressAmmoSideButton?.Invoke(0);
        ShowAmmo();
        HapticFeedbackManager.instance.SoftImpactButton();

    }

    public void SetPressable(bool enable)
    {
        mainImage.raycastTarget = enable;
    }

    public void CollectCage(bool collected, bool fromHidden, Sequence seq, float dur)
    {
        if (collected)
        {
            if (button != null)
                button.enabled = false;
            // seq.Join(mainRect.DOLocalMove(cageEquippedPosition, dur));
            // seq.Join(mainRect.DOScale(1, dur));
        }
        else
        {
            if (button != null)
                button.enabled = true;

            ShowOrHide(fromHidden, seq);
        }

    }

    public void EquipAmmo(int i)
    {

    }
    public void UnEquipAmmo(int i)
    {

    }

    private void SwitchAmmo(int nextAmmoType, int amountOfAmmo, int direction)
    {
        if (direction == -2)
        {
            SetPositions(nextAmmoType);
            return;
        }

        if (tweeningEggs)
        {
            // SetPositions(currentEquippedEgg);
            seq.Complete();

        }
        TweenPositions(nextAmmoType, direction);


    }
    private bool isShown = true;
    private Sequence showSeq;
    private void ShowAmmo()
    {
        if (showSeq != null && showSeq.IsPlaying()) showSeq.Complete();

        showSeq = DOTween.Sequence();

        if (isShown)
        {
            isShown = false;
            showSeq.Append(mainRect.DOSizeDelta(hiddenSizeDelta, hideButtonDuration));
            showSeq.Play().SetUpdate(true).OnComplete(() => parentMask.gameObject.SetActive(false));

        }
        else
        {
            isShown = true;
            parentMask.gameObject.SetActive(true);
            showSeq.Append(mainRect.DOSizeDelta(buttonShownSize, showButtonDuration));
            showSeq.Play().SetUpdate(true);

        }

    }

    public void ShowOrHide(bool hide, Sequence seq)
    {
        // return;
        // int nonEquippedEgg = 0;

        // if (currentEquippedAmmoType == 0)
        //     nonEquippedEgg = 1;




        if (!hide)
        {


            // EquipAmmo(currentEquippedAmmoType);
            // UnEquipAmmo(nonEquippedEgg);
            seq.Join(mainRect.DOSizeDelta(buttonShownSize, showButtonDuration).SetEase(Ease.OutBack));



            seq.Join(mainRect.DOScale(startEndScale.x, showButtonDuration));
            // seq.Join(mainRect.DOLocalMove(buttonShownPos, showButtonDuration).SetEase(Ease.OutBack));

        }
        else
        {


            // seq.Join(eggs[currentEquippedAmmoType].DOLocalMove(eggButtonHiddenPosition1.localPosition, hideButtonDuration));


            // seq.Join(eggs[nonEquippedEgg].DOLocalMove(eggButtonHiddenPosition2.localPosition, hideButtonDuration));
            seq.Join(mainRect.DOSizeDelta(fullSize, hideButtonDuration).SetEase(Ease.OutBack));


            seq.Join(mainRect.DOScale(startEndScale.y, hideButtonDuration));
            // seq.Join(mainRect.DOLocalMove(fullSizePos, hideButtonDuration).SetEase(Ease.OutBack));


        }

    }


    private void SetPositions(int equippedAmmoType)
    {
        // Find the index of the equipped ammo type
        if (seq != null && seq.IsPlaying())
            seq.Kill();

        int equippedIndex = System.Array.IndexOf(eggOrderByType, equippedAmmoType);
        currentEquippedEgg = equippedIndex;

        if (equippedIndex == -1)
        {
            Debug.LogError($"Equipped ammo type {equippedAmmoType} not found in eggOrderByType!");
            return;
        }

        // Set the equipped ammo's position to the first position
        ammoRects[equippedIndex].anchoredPosition = positions[0];

        // Keep track of the current position index for unequipped items
        int positionIndex = 1;

        // Loop through all ammo types and position them
        for (int i = 0; i < eggOrderByType.Length; i++)
        {
            // Skip the equipped ammo
            if (i == equippedIndex)
                continue;

            // Set the position of unequipped ammo
            ammoRects[i].anchoredPosition = positions[positionIndex];
            positionIndex++;
        }
    }

    private void TweenPositions(int equippedAmmoType, int direction)
    {
        SwitchAmmoTween();
        tweeningEggs = true;
        int lastEgg = currentEquippedEgg;
        int equippedIndex = System.Array.IndexOf(eggOrderByType, equippedAmmoType);
        currentEquippedEgg = equippedIndex;
        int size = ammoRects.Length;
        int[] currentOrder = new int[size];
        bool wrapped = false;
        currentOrder[0] = lastEgg;

        for (int i = 1; i < size; i++)
        {
            int next = lastEgg + i;
            if (next >= size)
            {
                next -= size;
            }

            currentOrder[i] = next;
        }



        if (equippedIndex == -1)
        {
            Debug.LogError($"Equipped ammo type {equippedAmmoType} not found in eggOrderByType!");
            return;
        }

        // Create a DOTween sequence for the tweening animations
        seq = DOTween.Sequence();


        if (direction == 1)
        {

            seq.Append(ammoRects[currentOrder[0]].DOAnchorPosX(positions[size - 1].x, duration).From(endRightPosition));

            for (int n = 1; n < size; n++)
            {
                seq.Join(ammoRects[currentOrder[n]].DOAnchorPosX(positions[n - 1].x, duration));

            }


        }
        else if (direction == -1)
        {
            seq.Append(ammoRects[currentOrder[size - 1]].DOAnchorPosX(endRightPosition.x, duration).OnComplete(() => ammoRects[currentOrder[size - 1]].anchoredPosition = positions[0]));

            for (int n = 0; n < size - 1; n++)
            {
                seq.Join(ammoRects[currentOrder[n]].DOAnchorPosX(positions[n + 1].x, duration));

            }

        }
        seq.Play().SetUpdate(true).SetEase(equipEase).OnComplete(() => tweeningEggs = false);


    }

    private void SwitchAmmoTween()
    {
        return;
        if (rotateSidebarSeq != null && rotateSidebarSeq.IsPlaying())
            rotateSidebarSeq.Kill();

        button.enabled = false;

        rotateSidebarSeq = DOTween.Sequence();
        rotateSidebarSeq.Append(mainRect.DOLocalMove(UnequippedParentPostionOnAmmoSwitch.localPosition, startAmmoSwitchTweenDuration));
        rotateSidebarSeq.Join(mainRect.DOLocalRotate(UnequippedParentPostionOnAmmoSwitch.eulerAngles, startAmmoSwitchTweenDuration));
        rotateSidebarSeq.Append(mainRect.DOLocalMove(mainRectStartPos, endAmmoSwitchTweenDuration));
        rotateSidebarSeq.Join(mainRect.DOLocalRotate(Vector3.zero, endAmmoSwitchTweenDuration));
        rotateSidebarSeq.Play().SetUpdate(true).OnComplete(() => button.enabled = true);
    }



}