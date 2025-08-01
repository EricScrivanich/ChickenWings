using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;
using DG.Tweening;
using UnityEngine.UI;

public class LevelPickerManager : MonoBehaviour
{
    private InputController controls;
    [SerializeField] private PathCreator[] paths;
    [SerializeField] private PlayerLevelPickerPathFollwer playerPathFollower;
    [SerializeField] private GameObject levelUiPopupPrefab;
    private CanvasGroup[] levelPopups;
    [SerializeField] private Transform levelPopupParent;

    [SerializeField] private float tweenScaleBack;
    [SerializeField] private float tweenDurBack;
    [SerializeField] private float tweenScaleFront;
    [SerializeField] private float tweenDurFront;
    [SerializeField] private RectTransform displayPopupPos;
    private CanvasGroup currentPopup;
    private CanvasGroup nextPopup;

    [Header("Sign Sequence Position Settings")]
    [SerializeField] private float topHiddenY;
    [SerializeField] private float normalY;
    [SerializeField] private float overShootY;
    [SerializeField] private float hiddenX;

    [SerializeField] private float rotationAngle;

    [Header("Sign Sequence Duration Settings")]
    [SerializeField] private float moveDownDuration;
    [SerializeField] private float moveUpDuration;
    [SerializeField] private float overshootDownDuration;
    [SerializeField] private float overshootUpDuration;
    [SerializeField] private float moveSideDuration;
    [SerializeField] private float swingDuration;




    private readonly Vector3[] rotations = new Vector3[]
      {
        new Vector3(0, 0, 8f),
        new Vector3(0, 0, -6.5f),
        new Vector3(0, 0, 3f),
        new Vector3(0, 0, -1.7f),
        new Vector3(0, 0, .7f),

        Vector3.zero
      };



    private Sequence popupSequence;
    private Sequence signYSeq;
    private Sequence signXSeq;

    private int currentPopupIndex;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new InputController();

        levelUiPopupPrefab.gameObject.SetActive(false);
        nextPopup = null;
        currentPopup = null;




        controls.LevelCreator.CursorClick.performed += ctx =>
        {


            HandleClickObject(Mouse.current.position.ReadValue());

        };

        controls.LevelCreator.Finger1Press.performed += ctx =>
        {
            HandleClickObject(Pointer.current.position.ReadValue());

        };
        //     controls.LevelCreator.Finger1Press.canceled += ctx =>
        //    {

        //        HandleReleaseClick();
        //    };

        //     controls.LevelCreator.CursorClick.canceled += ctx =>
        //     {

        //         HandleReleaseClick();
        //     };

        //     controls.LevelCreator.Finger1Pos.performed += ctx =>
        //   {
        //       Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        //       HandleMoveObject(pos);

        //   };

        //     controls.LevelCreator.CursorPos.performed += ctx =>
        //     {
        //         Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        //         HandleMoveObject(pos);
        //     };


    }
    private int currentPlayerPath;
    private ILevelPickerPathObject currentTarget;

    private void HandleClickObject(Vector2 screenPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPosition);

        var obj = GetObjectFromTouchPosition(worldPoint);

        if (obj != null && obj != currentTarget)
        {
            HapticFeedbackManager.instance.PlayerButtonPress();
            if (currentTarget != null)
            {
                currentTarget.SetSelected(false);
            }
            currentTarget = obj;


            currentTarget.SetSelected(true);
            Vector3Int data = obj.Return_Type_PathIndex_Order();

            float d = paths[data.y].path.GetClosestDistanceAlongPath(obj.ReturnLinePostion());
            playerPathFollower.DoPathToPoint(paths[data.y], d);
            DoLevelPopupSeq(true, obj.ReturnWorldNumber());











        }
        else
        {
            Debug.Log("No object found at the clicked position.");

        }
    }

    private void HandleSignY(RectTransform target, bool reverse)
    {



        if (reverse)
        {
            Sequence hideSeq = DOTween.Sequence();
            hideSeq.Append(target.DOAnchorPosY(normalY - overShootY, overshootUpDuration));
            hideSeq.Append(target.DOAnchorPosY(topHiddenY, moveUpDuration));
            hideSeq.Play().SetEase(Ease.InSine).SetUpdate(true).OnComplete(() =>
              {
                  Destroy(target.gameObject);
              });



        }
        else
        {
            signYSeq = DOTween.Sequence();
            signYSeq.Append(target.DOAnchorPosY(normalY - overShootY, moveDownDuration));
            signYSeq.Append(target.DOAnchorPosY(normalY, overshootDownDuration));
            signYSeq.Play().SetEase(Ease.InSine).SetUpdate(true);

        }



    }

    private void HandleSignX(RectTransform currentShown, RectTransform nextShown, int flip)
    {
        signXSeq = DOTween.Sequence();
        float halfPos = hiddenX * .5f;
        float halfDur = moveSideDuration * .5f;


        currentShown.DOAnchorPosX(hiddenX * -flip, halfDur).SetEase(Ease.InSine).SetUpdate(true);
        currentShown.DORotate(rotations[0] * flip, halfDur).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() =>
        {
            Destroy(currentShown.gameObject);
        });
        signXSeq.Append(nextShown.DOAnchorPosX(halfPos, halfDur).SetEase(Ease.InSine));
        signXSeq.Join(nextShown.DORotate(rotations[0] * flip, halfDur).SetEase(Ease.InSine));

        signXSeq.Append(nextShown.DOAnchorPosX(0, halfDur + .1f).SetEase(Ease.OutSine));
        signXSeq.Join(nextShown.DORotate(rotations[1] * flip, halfDur + .1f).SetEase(Ease.OutSine));

        for (int i = 2; i < rotations.Length; i++)
        {
            signXSeq.Append(nextShown.DORotate(rotations[i] * flip, swingDuration).SetEase(Ease.InOutSine));

        }

        signXSeq.Play().SetUpdate(true);




    }
    void ResetCurrentSign()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup.gameObject);
            currentPopup = null;
        }
        if (nextPopup != null)
        {
            currentPopup = nextPopup;
            nextPopup = null;
        }
    }

    public void BackOut()
    {
        HapticFeedbackManager.instance.PlayerButtonPress();
        currentTarget.SetSelected(false);
        currentTarget = null;
        DoLevelPopupSeq(false, Vector3Int.zero, true);
    }

    private void DoLevelPopupSeq(bool normalOrder, Vector3Int worldNum, bool goBack = false)
    {
        // if (popupSequence != null && popupSequence.IsActive())
        // {
        //     popupSequence.Complete();
        // }
        // popupSequence = DOTween.Sequence();

        if (signYSeq != null && signYSeq.IsActive())
        {
            signYSeq.Kill();
        }
        if (signXSeq != null && signXSeq.IsActive())
        {
            signXSeq.Kill();
        }





        if (goBack)
        {
            // var r1 = currentPopup.GetComponent<RectTransform>();
            // currentPopup.interactable = false;
            // // popupSequence.Append(r1.DOScale(tweenScaleBack, tweenDurBack).From(1));
            // // popupSequence.Join(r1.DOMove(Vector3.zero, tweenDurBack).From(displayPopupPos.position));
            // // popupSequence.Join(currentPopup.DOFade(0, tweenDurBack));


            // popupSequence.Play().OnComplete(() =>
            // {

            //     if (currentPopup.gameObject != null)
            //         Destroy(currentPopup.gameObject);

            //     currentPopup = null;



            // });

            HandleSignY(currentPopup.GetComponent<RectTransform>(), true);
            currentPopup = null;
            return;



        }
        LevelDataConverter.instance.SetCurrentLevelInstance(worldNum);
        int flip = 1;
        Vector2 startPos = Vector2.up * topHiddenY;
        if (!normalOrder)
        {
            flip = -1;
        }
        if (currentPopup != null)
        {
            startPos = new Vector2(hiddenX * flip, normalY);

        }
        nextPopup = Instantiate(levelUiPopupPrefab, levelPopupParent).GetComponent<CanvasGroup>();
        nextPopup.GetComponent<RectTransform>().anchoredPosition = startPos;

        nextPopup.GetComponent<LevelPickerUIPopup>().ShowData(LevelDataConverter.instance.ReturnLevelData(), this);
        nextPopup.gameObject.SetActive(true);


        if (currentPopup != null)
        {

            HandleSignX(currentPopup.GetComponent<RectTransform>(), nextPopup.GetComponent<RectTransform>(), flip);
            currentPopup = nextPopup;
            nextPopup = null;
        }
        else
        {
            HandleSignY(nextPopup.GetComponent<RectTransform>(), false);
            currentPopup = nextPopup;
            nextPopup = null;
        }



        // var r1 = nextPopup.GetComponent<RectTransform>();
        // popupSequence.Append(r1.DOScale(1, tweenDurBack).SetEase(Ease.OutBack).From(tweenScaleBack));
        // popupSequence.Join(r1.DOMove(displayPopupPos.position, tweenDurBack).SetEase(Ease.OutBack).From(Vector3.zero));
        // popupSequence.Join(nextPopup.DOFade(1, tweenDurBack).From(.3f).SetEase(Ease.OutSine));

        // if (currentPopup != null)
        // {
        //     var r2 = currentPopup.GetComponent<RectTransform>();
        //     currentPopup.interactable = false;
        //     popupSequence.Join(r2.DOScale(tweenScaleFront, tweenDurFront));
        //     popupSequence.Join(r2.DOMove(Vector3.zero, tweenDurFront));
        //     popupSequence.Join(currentPopup.DOFade(0, tweenDurFront));

        // }

        // popupSequence.Play().OnComplete(() =>
        // {
        //     nextPopup.interactable = true;
        //     nextPopup.alpha = 1;
        //     if (currentPopup != null)
        //         Destroy(currentPopup.gameObject);

        //     currentPopup = nextPopup;
        //     nextPopup = null;



        // });









    }



    private ILevelPickerPathObject GetObjectFromTouchPosition(Vector2 worldPoint)
    {



        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);



        if (hits.Length == 0)
        {
            Debug.Log("NO OBJECTS FOUND");
            return null;
        }


        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<ILevelPickerPathObject>() != null)
            {
                return hit.collider.gameObject.GetComponent<ILevelPickerPathObject>();
            }
        }

        return null;
    }

    private void OnEnable()
    {
        controls.LevelCreator.Enable();
        // LevelRecordManager.AddNewObject += SetObjectToBeAdded;

    }

    private void OnDisable()
    {
        controls.LevelCreator.Disable();

        // timeSlider.onValueChanged.RemoveAllListeners();
        // LevelRecordManager.AddNewObject -= SetObjectToBeAdded;





    }


    // private int prevPopupIndex = 1;
    // Update is called once per frame
    // private int ReturnCurrentPopupIndex()
    // {
    //     if (currentPopupIndex < 0) currentPopupIndex = 1;
    //     else if (currentPopupIndex >= levelPopups.Length) currentPopupIndex = 0;


    //     if (currentPopupIndex == 0) prevPopupIndex = 1;
    //     else prevPopupIndex = 0;

    //     return currentPopupIndex;
    // }
}
