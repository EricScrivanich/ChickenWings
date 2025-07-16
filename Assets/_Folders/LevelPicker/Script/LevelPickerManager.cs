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




    private Sequence popupSequence;

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

    public void BackOut()
    {
        DoLevelPopupSeq(false, Vector3Int.zero, true);
    }

    private void DoLevelPopupSeq(bool normalOrder, Vector3Int worldNum, bool goBack = false)
    {
        if (popupSequence != null && popupSequence.IsActive())
        {
            popupSequence.Complete();
        }
        popupSequence = DOTween.Sequence();





        if (goBack)
        {
            var r1 = currentPopup.GetComponent<RectTransform>();
            currentPopup.interactable = false;
            popupSequence.Append(r1.DOScale(tweenScaleBack, tweenDurBack).From(1));
            popupSequence.Join(r1.DOMove(Vector3.zero, tweenDurBack).From(displayPopupPos.position));
            popupSequence.Join(currentPopup.DOFade(0, tweenDurBack));
            popupSequence.Play().OnComplete(() =>
            {

                if (currentPopup.gameObject != null)
                    Destroy(currentPopup.gameObject);

                currentPopup = null;



            });
            return;



        }
        LevelDataConverter.instance.SetCurrentLevelInstance(worldNum);
        nextPopup = Instantiate(levelUiPopupPrefab, levelPopupParent).GetComponent<CanvasGroup>();
        nextPopup.alpha = 0;
        nextPopup.GetComponent<LevelPickerUIPopup>().ShowData(LevelDataConverter.instance.ReturnLevelData(), this);
        nextPopup.gameObject.SetActive(true);

        if (normalOrder)
        {
            var r1 = nextPopup.GetComponent<RectTransform>();
            popupSequence.Append(r1.DOScale(1, tweenDurBack).SetEase(Ease.OutBack).From(tweenScaleBack));
            popupSequence.Join(r1.DOMove(displayPopupPos.position, tweenDurBack).SetEase(Ease.OutBack).From(Vector3.zero));
            popupSequence.Join(nextPopup.DOFade(1, tweenDurBack).From(.3f).SetEase(Ease.OutSine));

            if (currentPopup != null)
            {
                var r2 = currentPopup.GetComponent<RectTransform>();
                currentPopup.interactable = false;
                popupSequence.Join(r2.DOScale(tweenScaleFront, tweenDurFront));
                popupSequence.Join(r2.DOMove(Vector3.zero, tweenDurFront));
                popupSequence.Join(currentPopup.DOFade(0, tweenDurFront));

            }

            popupSequence.Play().OnComplete(() =>
            {
                nextPopup.interactable = true;
                nextPopup.alpha = 1;
                if (currentPopup != null)
                    Destroy(currentPopup.gameObject);

                currentPopup = nextPopup;
                nextPopup = null;



            });


        }






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
