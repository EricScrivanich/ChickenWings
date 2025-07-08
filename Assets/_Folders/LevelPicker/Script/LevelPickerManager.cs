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

    [SerializeField] private CanvasGroup[] levelPopups;

    [SerializeField] private float tweenScaleBack;
    [SerializeField] private float tweenDurBack;
    [SerializeField] private float tweenScaleFront;
    [SerializeField] private float tweenDurFront;
    [SerializeField] private RectTransform displayPopupPos;




    private Sequence popupSequence;

    private int currentPopupIndex;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new InputController();
        foreach (var popup in levelPopups)
        {
            popup.alpha = 0;
            popup.interactable = false;


            popup.gameObject.SetActive(false);
        }

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
            DoLevelPopupSeq(true);











        }
        else
        {
            Debug.Log("No object found at the clicked position.");

        }
    }

    public void BackOut()
    {
        DoLevelPopupSeq(false, true);
    }

    private void DoLevelPopupSeq(bool normalOrder, bool goBack = false)
    {
        if (popupSequence != null && popupSequence.IsActive())
        {
            popupSequence.Complete();
        }
        popupSequence = DOTween.Sequence();
        int index = ReturnCurrentPopupIndex();
        if (goBack)
        {
            index = prevPopupIndex;
        }

        levelPopups[index].gameObject.SetActive(true);
        var r1 = levelPopups[index].GetComponent<RectTransform>();

        if (goBack)
        {
            levelPopups[index].interactable = false;
            popupSequence.Append(r1.DOScale(tweenScaleBack, tweenDurBack).From(1));
            popupSequence.Join(r1.DOMove(Vector3.zero, tweenDurBack).From(displayPopupPos.position));
            popupSequence.Join(levelPopups[index].DOFade(0, tweenDurBack));
            popupSequence.Play();
            return;



        }

        if (normalOrder)
        {

            popupSequence.Append(r1.DOScale(1, tweenDurBack).SetEase(Ease.OutBack).From(tweenScaleBack));
            popupSequence.Join(r1.DOMove(displayPopupPos.position, tweenDurBack).SetEase(Ease.OutBack).From(Vector3.zero));
            popupSequence.Join(levelPopups[index].DOFade(1, tweenDurBack).From(.3f).SetEase(Ease.OutSine));

            if (levelPopups[prevPopupIndex].gameObject.activeInHierarchy)
            {
                var r2 = levelPopups[prevPopupIndex].GetComponent<RectTransform>();
                levelPopups[prevPopupIndex].interactable = false;
                popupSequence.Join(r2.DOScale(tweenScaleFront, tweenDurFront));
                popupSequence.Join(r2.DOMove(Vector3.zero, tweenDurFront));
                popupSequence.Join(levelPopups[prevPopupIndex].DOFade(0, tweenDurFront));

            }

            popupSequence.Play().OnComplete(() =>
            {
                levelPopups[index].interactable = true;
                levelPopups[index].alpha = 1;

                levelPopups[prevPopupIndex].gameObject.SetActive(false);
                currentPopupIndex++;

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


    private int prevPopupIndex = 1;
    // Update is called once per frame
    private int ReturnCurrentPopupIndex()
    {
        if (currentPopupIndex < 0) currentPopupIndex = 1;
        else if (currentPopupIndex >= levelPopups.Length) currentPopupIndex = 0;


        if (currentPopupIndex == 0) prevPopupIndex = 1;
        else prevPopupIndex = 0;

        return currentPopupIndex;
    }
}
