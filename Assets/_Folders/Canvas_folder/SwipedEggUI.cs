using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SwipedEggUI : MonoBehaviour
{
    [Header("Egg Pressed")]
    private bool playingPressedRoutine = false;
    [SerializeField] private float shakeAmountTarget;
    [SerializeField] private float offsetAmountTargetY;
    [SerializeField] private float zoomAmountTarget;
    [SerializeField] private float blurAmountTarget;
    [SerializeField] private float pressedEggDurationIN;
    [SerializeField] private float pressedEggDurationOUT;

    private Sequence eggPressSeq;

    private Sequence rotationSeq;
    private float moveYAmount;
    private float pressTweenDuration;

    private float currentShake = 0;
    private float currentBlur = 0;
    private float currentOffsetY = 0;
    private float currentZoom = 1;

    private Coroutine eggPressedRoutine;


    private Material eggMaterial;
    private bool OnZero = false;

    private int currentAmmo = 0;

    private Sequence flashAmmoSequence;

    [SerializeField] private ButtonColorsSO colorSO;
    public bool initialEgg;
    [SerializeField] private PlayerID player;
    [SerializeField] private Image swipeButton;
    [SerializeField] private Vector3 startRotation = new Vector3(0, 0, 60);
    [SerializeField] private Vector3 middleRotation = new Vector3(0, 0, 5);

    private float originalEggPos;

    private bool isActive = false;

    private float lastPointerPosition;

    private int dragThreshold = 60;


    [SerializeField] private Image eggIMG;
    private EggAmmoDisplay parentScript;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform rect;

    [SerializeField] private int currentType;

    [SerializeField] private float rotateInDuration;



    // Start is called before the first frame update


    private void Awake()
    {

        parentScript = GetComponentInParent<EggAmmoDisplay>();
        rect = GetComponent<RectTransform>();
        // text.color = new Color(colorSO.normalButtonColor.r, colorSO.normalButtonColor.g, colorSO.normalButtonColor.b, 1);
        text.color = Color.white;
        originalEggPos = eggIMG.rectTransform.localPosition.y;


    }

    private void Start()
    {
        if (currentType == 1)
        {
            if (player.ShotgunAmmo == 0)
            {
                OnZero = true;
                FlashAmmoTween(false);
            }
        }

    }

    // Update is called once per frame
    void UpdateAmmo(int amount)
    {
        Debug.Log("Ammo update");
        text.text = amount.ToString();
        parentScript.SetAmmo(amount);

        currentAmmo = amount;
        if (currentType == 0 && eggIMG != null)
        {
            if (amount <= 0)
            {
                eggIMG.color = colorSO.DisabledEggTextColor;
                text.color = colorSO.disabledButtonColorFull;
                OnZero = true;
            }
            else if (OnZero)
            {
                OnZero = false;
                eggIMG.color = Color.white;
                text.color = colorSO.MainTextColor;

            }

        }
        else if (currentType == 1 && eggIMG != null)
        {

            if (amount <= 0 && !OnZero)
            {
                // FlashAmmoTween(true);
                FlashAmmoTween(false);  // commented out for no cahined ammo

                OnZero = true;
            }
            else if (OnZero && amount > 0)
            {
                OnZero = false;
                player.globalEvents.OnUseChainedAmmo?.Invoke(false);
                eggIMG.color = Color.white;
                text.color = colorSO.MainTextColor;

            }



        }


    }

    public void FlashAmmoTween(bool flash)
    {


        if (flashAmmoSequence != null && flashAmmoSequence.IsPlaying())
            flashAmmoSequence.Kill();

        flashAmmoSequence = DOTween.Sequence();

        if (flash)
        {
            text.color = colorSO.DashImageManaHighlight2;


            flashAmmoSequence.Append(text.DOColor(colorSO.DashImageManaHighlight, .2f));
            flashAmmoSequence.Append(text.DOColor(colorSO.DashImageManaHighlight2, .2f));
            flashAmmoSequence.Play().SetLoops(-1);


        }
        else
        {
            text.color = colorSO.disabledButtonColorFull;

        }
    }
    private void OnEnable()
    {


    }
    private void OnDisable()
    {
        DOTween.Kill(this);
        isActive = false;
        switch (currentType)
        {
            case (0):

                player.globalEvents.OnUpdateAmmo -= UpdateAmmo;



                break;
            case (1):
                player.globalEvents.OnUpdateShotgunAmmo -= UpdateAmmo;


                break;
        }
    }


    public void UnEquip(bool clockwise, int t)
    {
        if (currentType == t)
        {
            int mul = 1;
            if (!clockwise)
            {
                mul = -1;

            }
            rect.DORotate(-startRotation * mul, .1f).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
        }


    }
    public void Equip(bool clockwise, int type)
    {
        if (type != currentType) return;
        int mul = -1;
        // isRotating = true;
        // isPressed = false;

        if (clockwise)
        {
            mul = 1;

        }
        rect.eulerAngles = startRotation * mul;

        gameObject.SetActive(true);

        switch (currentType)
        {
            case (0):
                UpdateAmmo(player.Ammo);

                player.globalEvents.OnUpdateAmmo += UpdateAmmo;



                break;
            case (1):
                UpdateAmmo(player.ShotgunAmmo);
                player.globalEvents.OnUpdateShotgunAmmo += UpdateAmmo;


                break;
        }






        if (rotationSeq != null && rotationSeq.IsPlaying())
            rotationSeq.Kill();



        rotationSeq = DOTween.Sequence();
        // parentScript.SetJoystick(eggIMG.rectTransform, type);






        // gameObject.SetActive(true);
        // rotationSeq.Append(rect.DORotate(-middleRotation * mul, .2f));
        rotationSeq.Append(rect.DORotate(Vector3.zero, rotateInDuration).SetEase(Ease.OutBack));
        rotationSeq.Play().SetUpdate(true);

        player.events.OnSwitchAmmoType?.Invoke(type);

        // switch (type)
        // {
        //     case (0):

        //         UpdateAmmo(player.Ammo);
        //         player.globalEvents.OnUpdateAmmo += UpdateAmmo;
        //         break;
        //     case (1):

        //         UpdateAmmo(player.ShotgunAmmo);

        //         player.globalEvents.OnUpdateShotgunAmmo += UpdateAmmo;


        //         break;
        // }



    }

    public void SetTweenAmounts(float moveY, float duration)
    {
        moveYAmount = moveY;
        pressTweenDuration = duration;

    }

    private void EggPressedTween()
    {
        // if (eggPressSeq != null && eggPressSeq.IsPlaying())
        //     eggPressSeq.Kill();

        // eggPressSeq = DOTween.Sequence();

        // eggPressSeq.Append(eggIMG.rectTransform.DOLocalMoveY(originalEggPos + moveYAmount, pressTweenDuration));
        // eggPressSeq.Join(eggIMG.rectTransform.DOShakeRotation(10, pressTweenDuration));
        // eggPressSeq.Append(eggIMG.rectTransform.DOLocalMoveY(originalEggPos, pressTweenDuration));
        // eggPressSeq.Join(eggIMG.rectTransform.DOShakeRotation(0, pressTweenDuration));

        // eggPressSeq.Play();


    }



    private void EggPressed()
    {
        // Debug.Log("Egg pressed");


        // parentScript.NormalEggPressTween();


        // if (playingPressedRoutine)
        //     StopCoroutine(eggPressedRoutine);

        // eggPressedRoutine = StartCoroutine(EggPressedMaterialCourintine());



    }
    // private void MaterialOnStop()
    // {



    //     // eggIMG.materialForRendering.SetFloat("_ZoomUvAmount", 1);

    //     StartCoroutine(StopBlur());



    // }

    // private IEnumerator StopBlur()
    // {
    //     float time = 0;

    //     while (time < .4f)
    //     {
    //         time += Time.deltaTime;
    //         currentBlur = Mathf.Lerp(blurAmountTarget, 0, time / .4f);
    //         eggIMG.materialForRendering.SetFloat("_BlurIntensity", currentBlur);
    //         yield return null;
    //     }

    //     eggIMG.materialForRendering.SetFloat("_BlurIntensity", 0);
    // }

    // private void MaterialOnSwitch()
    // {

    //     // eggIMG.materialForRendering.SetFloat("_ZoomUvAmount", 1.15f);

    //     eggIMG.materialForRendering.SetFloat("_BlurIntensity", blurAmountTarget);





    // }


    // private IEnumerator EggPressedMaterialCourintine()
    // {
    //     Debug.Log("entered routine");
    //     playingPressedRoutine = true;
    //     float currentShakeVar = currentShake;
    //     float currentBlurVar = currentBlur;
    //     float currentOffsetYVar = currentOffsetY;
    //     float currentZoomVar = currentZoom;

    //     float time = 0;

    //     while (time < pressedEggDurationIN)
    //     {
    //         time += Time.deltaTime;

    //         currentShake = Mathf.Lerp(currentShakeVar, shakeAmountTarget, time / pressedEggDurationIN);
    //         // currentBlur = Mathf.Lerp(currentBlurVar, blurAmountTarget, time / pressedEggDurationIN);
    //         currentOffsetY = Mathf.Lerp(currentOffsetYVar, offsetAmountTargetY, time / pressedEggDurationIN);
    //         currentZoom = Mathf.Lerp(currentZoomVar, zoomAmountTarget, time / pressedEggDurationIN);


    //         eggIMG.materialForRendering.SetFloat("_ZoomUvAmount", currentZoom);
    //         eggIMG.materialForRendering.SetFloat("_ShakeUvSpeed", currentShake);
    //         eggIMG.materialForRendering.SetFloat("_OffsetUvY", currentOffsetY);
    //         // eggIMG.materialForRendering.SetFloat("_BlurIntensity", currentBlur);

    //         yield return null;

    //     }

    //     time = 0;

    //     while (time < pressedEggDurationOUT)
    //     {
    //         time += Time.deltaTime;

    //         currentShake = Mathf.Lerp(shakeAmountTarget, 0, time / pressedEggDurationOUT);
    //         // currentBlur = Mathf.Lerp(blurAmountTarget, 0, time / pressedEggDurationOUT);
    //         currentOffsetY = Mathf.Lerp(offsetAmountTargetY, 0, time / pressedEggDurationOUT);
    //         currentZoom = Mathf.Lerp(zoomAmountTarget, 1, time / pressedEggDurationOUT);


    //         eggIMG.materialForRendering.SetFloat("_ZoomUvAmount", currentZoom);
    //         eggIMG.materialForRendering.SetFloat("_ShakeUvSpeed", currentShake);
    //         eggIMG.materialForRendering.SetFloat("_OffsetUvY", currentOffsetY);
    //         // eggIMG.materialForRendering.SetFloat("_BlurIntensity", currentBlur);

    //         yield return null;

    //     }
    //     eggIMG.materialForRendering.SetFloat("_ZoomUvAmount", 1);
    //     eggIMG.materialForRendering.SetFloat("_ShakeUvSpeed", 0);
    //     eggIMG.materialForRendering.SetFloat("_OffsetUvY", 0);
    //     // eggIMG.materialForRendering.SetFloat("_BlurIntensity", 0);
    //     playingPressedRoutine = false;


    // }

    public void HighlightButton(bool isPressed)
    {
        // if (!gameObject.activeInHierarchy) return;
        if (isPressed) swipeButton.color = colorSO.highlightButtonColor;
        else swipeButton.color = colorSO.normalButtonColor;

    }








}
