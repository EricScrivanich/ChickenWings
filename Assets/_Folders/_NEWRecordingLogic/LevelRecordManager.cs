using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelRecordManager : MonoBehaviour, IPointerDownHandler
{
    public static LevelRecordManager instance;


    [SerializeField] private bool testNonEditorMode = false;
    [SerializeField] private GameObject DeleteButtonForMultSelectTime;

    [SerializeField] private GameObject PlayTimeObject;
    [SerializeField] private GameObject ViewObject;

    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private Image PlayOrPauseImage;
    [SerializeField] private Sprite PlayImage;
    [SerializeField] private Sprite PauseImage;

    public LevelCreatorColors colorSO;

    // public RecordableObjectPlacer[] recordableObjectsByIDfd;
    public GameObject checkPointFlagPrefab;
    public RecordableObjectPlacer[] pigsByID;
    public RecordableObjectPlacer[] aiByID;
    public RecordableObjectPlacer[] buildingsByID;
    public RecordableObjectPlacer[] collectablesByID;
    public RecordableObjectPlacer[] positionersByID;
    public RecordableObjectPlacer[] ringsByID;
    public RecordableObjectPlacer[] ByID;




    public RecordableObjectPlacer[] recordableObjectsWithNegtiveID;
    public GameObject[] specialAddedObjects;

    [SerializeField] private TextMeshProUGUI posXText;
    [SerializeField] private TextMeshProUGUI posYText;






    private int arrowPressedType = 0;





    private InputController controls;
    public static readonly float TimePerStep = .18f;
    public static ushort CurrentTimeStep { get; private set; }
    public static ushort CurrentPlayTimeStep { get; private set; }
    public static ushort PreloadObjectsTimeStep { get; private set; }
    public static int currentSavedMinIndex { get; private set; }
    public static int currentSavedMaxIndex { get; private set; }

    public LevelData levelData;

    public List<RecordableObjectPlacer> RecordedObjects;

    private List<CheckPointFlag> checkPointFlags = new List<CheckPointFlag>();

    [SerializeField] private RectTransform minuteHand;
    [SerializeField] private RectTransform hourHand;


    // [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI stepText;


    public static Action<ushort, float> SetGlobalTime;

    public static Action<int> OnShowObjectTime;
    public static Action<int, bool> OnSendSpecialDataToActiveObjects;


    public static Action PressTimerBar;
    private GameObject objectToAdd;

    private CanvasGroup editorCanvasGroup;
    [SerializeField] private CanvasGroup timebarCanvasGroup;
    public static bool PreloadedSceneReady = false;




    private bool objectClicked = false;
    private bool screenClicked = false;

    public RecordableObjectPlacer currentSelectedObject { get; private set; }

    public bool multipleObjectsSelected { get; private set; } = false;
    public bool multipleSelectedIDs { get; private set; } = false;

    public List<RecordableObjectPlacer> MultipleSelectedObjects = new List<RecordableObjectPlacer>();

    private Vector2 lastObjPos;
    private Vector2 lastCamPos;
    private Vector2 lastClickedPos;

    private Coroutine NormalTimeCor;


    private float cameraY;


    private bool moveCamera = false;
    private bool goingToAddObject = false;

    private Vector2 posOnDragNew = new Vector2(0, .5f);

    [Header("UI Elements")]


    [SerializeField] private GameObject parameterUI;
    [SerializeField] private GameObject folderUI;

    public bool editingPostionerObject = false;

    public PositionerObject currentSelectedPostionerObject;
    [HideInInspector] public int currentSelectedPostionerObjectIndex;

    public static bool ShowLines { get; private set; }
    public static bool ShowSpeeds { get; private set; }
    public static bool ShowFolders { get; private set; }
    public static bool ShowTime { get; private set; }
    public static bool ShowParameters { get; private set; }

    private bool showLinesTemp;
    private bool showParamsTemp;
    private ushort currentTimeStepTemp;
    private bool isPlayModeView = false;

    public bool MultiTouch { get; private set; } = false;

    public ushort finalSpawnStep { get; private set; } = 150;

    [SerializeField] private TextMeshProUGUI LevelTitleText;


    [SerializeField] private GameObject levelPicker;

    [SerializeField] private GameObject[] editorOnlyButtons;








    public static void ResetStaticParameters()
    {
        ShowLines = true;
        ShowSpeeds = false;
        ShowFolders = true;
        ShowParameters = true;

        ShowTime = false;
        CurrentTimeStep = 0;
        CurrentPlayTimeStep = 0;
        currentSavedMinIndex = 0;
        currentSavedMaxIndex = 225;


    }


    public void SetMinAndMax(int min, int max)
    {
        currentSavedMinIndex = min;
        currentSavedMaxIndex = max;
    }

    public static Action CheckViewParameters;
    public void InitializeViewParameters()
    {
        CustomTimeSlider.instance.SetPixelDragFactor();
        if (ShowTime)
        {
            CustomTimeSlider.instance.TimeEditorView();
            folderUI.SetActive(false);
        }
        else if (ShowFolders)
        {
            CustomTimeSlider.instance.NormalView();

        }

        CheckViewParameters?.Invoke();

    }

    public void RestoreStaticParameters()
    {
        ShowLines = showLinesTemp;
        ShowParameters = showParamsTemp;
        ShowTime = false;
        CurrentTimeStep = currentTimeStepTemp;
        if (currentSelectedObject != null)
        {
            currentSelectedObject.SetIsSelected(false);
            currentSelectedObject = null;
        }


    }
    public void PreloadPlayScene()
    {
        PreloadedSceneReady = false;
        SetPreloadTimeStep();
        // if (preloadSceneCoroutine != null)
        // {
        //     StopCoroutine(preloadSceneCoroutine);
        //     Scene scene = SceneManager.GetSceneByName("LevelPlayer");


        // }
        // preloadSceneCoroutine = StartCoroutine(PreloadScene());



    }

    private IEnumerator PreloadScene()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Scene scene = SceneManager.GetSceneByName("LevelPlayer");

        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene);
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadSceneAsync("LevelPlayer", LoadSceneMode.Additive);


    }
    public void EnterPlayTime(bool enter)
    {
        if (enter && isPlayModeView)
            enter = false;
        if (enter)
        {

            isPlayModeView = true;
            SetUsingMultipleSelect(false);
            PressTimeBarWhilePlaying();
            PlayTimeObject.SetActive(true);
            CustomTimeSlider.instance.PlayView();
            folderUI.SetActive(false);
            parameterUI.SetActive(false);
            ViewObject.SetActive(false);
            currentSelectedObject = null;
            showLinesTemp = ShowLines;
            showParamsTemp = ShowParameters;
            currentTimeStepTemp = CurrentTimeStep;
            CurrentPlayTimeStep = CurrentTimeStep;

            ShowLines = false;
            ShowParameters = false;
            UpdateTime(CurrentPlayTimeStep, true);
            CheckViewParameters?.Invoke();



        }
        else
        {
            // if (preloadSceneCoroutine != null)
            // {
            //     StopCoroutine(preloadSceneCoroutine);
            //     Scene scene = SceneManager.GetSceneByName("LevelPlayer");

            //     if (scene.isLoaded)
            //         SceneManager.UnloadSceneAsync(scene);
            // }
            isPlayModeView = false;

            PlayTimeObject.SetActive(false);

            ShowLines = showLinesTemp;
            ShowParameters = showParamsTemp;
            CurrentTimeStep = currentTimeStepTemp;
            folderUI.SetActive(true);

            ViewObject.SetActive(true);


            CustomTimeSlider.instance.ExitPlayView();
            CheckViewParameters?.Invoke();
            UpdateTime(CurrentTimeStep, false);

        }
    }
    public void SetStaticViewParameter(HideItemUI.Type type, bool show)
    {
        Debug.Log("Setting static bool of " + type + " to " + show);
        switch (type)
        {
            case HideItemUI.Type.Lines:
                ShowLines = show;
                break;
            case HideItemUI.Type.Speeds:
                ShowSpeeds = show;
                break;
            case HideItemUI.Type.Parameters:
                ShowParameters = show;
                if (currentSelectedObject != null)
                    parameterUI.SetActive(show);
                break;
            case HideItemUI.Type.Time:

                if (show)
                {
                    CustomTimeSlider.instance.TimeEditorView();
                    if (ShowFolders)
                    {
                        ShowFolders = false;
                        folderUI.SetActive(false);

                    }

                }
                else if (ShowTime)
                {

                    CustomTimeSlider.instance.NormalView();
                }

                ShowTime = show;

                break;
            case HideItemUI.Type.Folders:
                ShowFolders = show;
                if (show)
                {

                    folderUI.SetActive(true);
                    if (ShowTime)
                    {
                        ShowTime = false;
                        CustomTimeSlider.instance.NormalView();
                    }
                }
                else
                {
                    folderUI.SetActive(false);
                }

                break;
            case HideItemUI.Type.MultiTouch:
                MultiTouch = show;
                break;
        }
        CheckViewParameters?.Invoke();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerDown(PointerEventData eventData)
    {
        lastCamPos = Camera.main.transform.position;
        moveCamera = true;

    }

    public void DuplicateObject()
    {


        if (multipleObjectsSelected && MultipleSelectedObjects.Count > 0)
        {
            for (int i = 0; i < MultipleSelectedObjects.Count; i++)
            {
                var o = ReturnDuplicateObject(MultipleSelectedObjects[i]);
                if (o != null)
                {
                    MultipleSelectedObjects[i].SetIsSelected(false);
                    o.SetSelectedObject();
                    MultipleSelectedObjects[i] = o;
                }
            }



        }
        else
        {
            var obj = ReturnDuplicateObject(currentSelectedObject);
            if (obj == null) return;
            if (currentSelectedObject != null)
            {
                currentSelectedObject.SetIsSelected(false);
            }
            currentSelectedObject = obj;
            obj.SetSelectedObject();
        }


    }

    private RecordableObjectPlacer ReturnDuplicateObject(RecordableObjectPlacer obj)
    {
        if (obj == null) return null;
        RecordableObjectPlacer newObj = null;
        if (obj.Data.ID >= 0)
        {
            newObj = Instantiate(ReturnObjectByPoolType(obj.Data.ObjectType, obj.Data.ID));
        }
        // else
        // {
        //     newObj = Instantiate(recordableObjectsWithNegtiveID[Mathf.Abs(obj.ID) - 1]);
        // }

        RecordedDataStructDynamic copy = obj.Data;
        newObj.LoadAssetFromSave(copy);
        AddNewObjectToList(newObj);
        newObj.UpdateBasePosition(new Vector2(obj.transform.position.x + obj.speedChanger, obj.transform.position.y));

        return newObj;
    }

    public void DeleteObject()
    {
        if (multipleObjectsSelected && MultipleSelectedObjects.Count > 0)
        {
            for (int i = 0; i < MultipleSelectedObjects.Count; i++)
            {
                if (MultipleSelectedObjects[i] != null)
                {
                    RecordedObjects.Remove(MultipleSelectedObjects[i]);
                    MultipleSelectedObjects[i].DestroySelf();
                }
            }
            MultipleSelectedObjects.Clear();
            if (!CustomTimeSlider.instance.isPlayView)
                SetUsingMultipleSelect(false);
            currentSelectedObject = null;
            parameterUI.SetActive(false);
            SortRecordedObjectsBySpawnTime();

            return;
        }
        RecordedObjects.Remove(currentSelectedObject);
        currentSelectedObject.DestroySelf();
        currentSelectedObject = null;
        parameterUI.SetActive(false);
        SortRecordedObjectsBySpawnTime();
        // for (int i = 0; i < RecordedObjects.Count; i ++)
        // {
        //     if (RecordedObjects[i] == currentSelectedObject)
        //     {
        //         RecordedObjects.Remove(i);
        //     }
        // }
    }
    private bool ignoreAllClicks = false;
    public void DeleteAll()
    {
        for (int i = 0; i < RecordedObjects.Count; i++)
        {
            RecordedObjects[i].DestroySelf();
        }
        RecordedObjects.Clear();
    }


    private bool isEditor = false;
    void Awake()
    {


        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        Time.timeScale = 0;
        if (currentSelectedObject != null)
        {
            currentSelectedObject.SetIsSelected(false);
            currentSelectedObject = null;
        }


        ShowFolders = true;
        ShowTime = false;


        CreatePools();
        controls = new InputController();
        isEditor = false;

#if UNITY_EDITOR
        isEditor = true;

        if (testNonEditorMode) isEditor = false;
#endif
        if (!isEditor)
        {
            Destroy(stepText);
            for (int i = editorOnlyButtons.Length - 1; i >= 0; i--)
            {
                Destroy(editorOnlyButtons[i]);
            }
            Destroy(levelPicker);
        }



        controls.LevelCreator.CursorClick.performed += ctx =>
        {


            HandleClickObject(Mouse.current.position.ReadValue());

        };

        controls.LevelCreator.Finger1Press.performed += ctx =>
        {
            HandleClickObject(Pointer.current.position.ReadValue());

        };
        controls.LevelCreator.Finger1Press.canceled += ctx =>
       {

           HandleReleaseClick();
       };

        controls.LevelCreator.CursorClick.canceled += ctx =>
        {

            HandleReleaseClick();
        };

        controls.LevelCreator.Finger1Pos.performed += ctx =>
      {
          Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
          HandleMoveObject(pos);

      };

        controls.LevelCreator.CursorPos.performed += ctx =>
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            HandleMoveObject(pos);
        };

        Debug.LogError("starting stats isL " + levelData.startingStats.name);
        ViewObject.SetActive(true);
        editorCanvasGroup = parameterUI.GetComponent<CanvasGroup>();
        // parameterUI.SetActive(false);


    }
    private int pressedStep;
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Block"))
            {
                return true;
            }
        }
        return false;
    }
    private void HandleClickObject(Vector2 pos)
    {
        if (ignoreAllClicks) return;
        if (IsPointerOverUI(pos))
        {
            Debug.LogError("Pointer is over UI");
            return;
        }
        else
        {
            Debug.LogError("Pointer is not over UI");
        }
        lastClickedPos = Camera.main.ScreenToWorldPoint(pos);

        if (editingPostionerObject)
        {
            var pObj = GetPositionerObjectFromTouchPosition(lastClickedPos);

            if (pObj != null || (currentSelectedPostionerObject != null && arrowPressedType == 3) || (currentSelectedPostionerObject != null && arrowPressedType == 4))
            {
                currentSelectedObject.SetIsSelected(false);
                if (pObj != null)
                    currentSelectedPostionerObject = pObj;
                currentSelectedPostionerObject.Press(arrowPressedType);

                lastObjPos = currentSelectedPostionerObject.transform.position;

                screenClicked = true;
                objectClicked = true;
                return;

            }

        }

        var obj = GetObjectFromTouchPosition(lastClickedPos);

        screenClicked = true;


        if (obj != null)
        {
            if (readyToPressForCage)
            {
                SetUsingMultipleSelect(false);
                if (obj.canAttachCage)
                {
                    obj.SetCageAttachment(true);
                    SetCageReady(false);
                    UpdateTime(CurrentTimeStep, false);

                }

                return;
            }
            currentSelectedPostionerObject = null;
            if (!parameterUI.activeInHierarchy && ShowParameters)
            {
                parameterUI.SetActive(true);

            }
            if (multipleObjectsSelected)
            {
                if (!obj.isSelected) // If the object is already selected, do nothing
                    MultipleSelectedObjects.Add(obj);


                short id = MultipleSelectedObjects[0].ID;
                bool sameID = true;
                foreach (var o in MultipleSelectedObjects)
                {
                    if (o.ID != id)
                    {
                        sameID = false;
                        break;
                    }
                }
                multipleSelectedIDs = !sameID;
            }
            else if (currentSelectedObject != null && currentSelectedObject != obj) currentSelectedObject.SetIsSelected(false);

            currentSelectedObject = obj;


            obj.SetSelectedObject();
            obj.HandleClick(true, arrowPressedType);
            ReturnRoundedPosition(obj.transform.position);
            lastObjPos = obj.transform.position;
            Debug.Log("Clicked on object: " + obj.name + " at position: " + lastObjPos);

            objectClicked = true;
            OnShowObjectTime?.Invoke(currentSelectedObject.spawnedTimeStep);

        }
        // else if (currentSelectedObject != null)
        // {
        //     currentSelectedObject.SetIsSelected(false);
        //     currentSelectedObject = null;
        // }

    }



    private void HandleReleaseClick()
    {
        objectClicked = false;
        screenClicked = false;
        moveCamera = false;
        goingToAddObject = false;
        objectToAdd = null;

        arrowPressedType = 0;
        if (currentSelectedObject != null)
            currentSelectedObject.HandleClick(false, 0);
        if (currentSelectedPostionerObject != null)
            currentSelectedPostionerObject.ReleaseClick();

        if (holdingObject)
        {
            holdingObject = false;
            FadeEditorCanvasGroup(false);
        }

    }
    private bool holdingHandle = false;

    private bool holdingObject = false;

    public void SetHoldingHandle(bool b)
    {
        holdingHandle = b;

        if (isPlayingAtNormalTime && b)
            PlayNormalSpeed();
    }

    private void FadeEditorCanvasGroup(bool fade)
    {
        if (fade)
        {
            editorCanvasGroup.alpha = .3f;
            editorCanvasGroup.blocksRaycasts = false;
            timebarCanvasGroup.alpha = .2f;
            timebarCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            editorCanvasGroup.alpha = 1;
            editorCanvasGroup.blocksRaycasts = true;
            timebarCanvasGroup.alpha = 1;
            timebarCanvasGroup.blocksRaycasts = true;
        }

    }



    public void UnactivateSelectedObject()
    {
        if (currentSelectedObject == null) return;

        currentSelectedObject.SetIsSelected(false);
        if (currentSelectedPostionerObject != null)
        {
            ValueEditorManager.instance.ShowMainPanel();
            currentSelectedPostionerObject = null;
        }
        CustomTimeSlider.instance.UnselectObject();
        parameterUI.SetActive(false);
        currentSelectedObject = null;
    }

    private void HandleMoveObject(Vector2 pos)
    {
        if (screenClicked && goingToAddObject && pos.y > -5.3f)
        {
            if (!parameterUI.activeInHierarchy && ShowParameters)
            {
                parameterUI.SetActive(true);

            }
            if (multipleObjectsSelected)
            {
                SetUsingMultipleSelect(false);
            }
            if (currentSelectedObject != null) currentSelectedObject.SetIsSelected(false);

            currentSelectedObject = Instantiate(objectToAdd, pos, Quaternion.identity).GetComponent<RecordableObjectPlacer>();
            currentSelectedObject.CreateNew(typeOvveride);
            AddNewObjectToList(currentSelectedObject);

            currentSelectedObject.SetSelectedObject();
            objectClicked = true;
            lastObjPos = Vector2.zero;
            goingToAddObject = false;
            lastClickedPos = posOnDragNew;
            OnShowObjectTime?.Invoke(currentSelectedObject.spawnedTimeStep);

        }

        if (objectClicked && !holdingHandle)
        {
            if (!holdingObject)
            {
                holdingObject = true;
                FadeEditorCanvasGroup(true);
            }
            if (resetMovePostion)
            {
                lastClickedPos = pos;
                resetMovePostion = false;
            }
            Vector2 moveAmount = lastClickedPos - pos;

            if (multipleObjectsSelected)
            {

                foreach (var o in MultipleSelectedObjects)
                {

                    if (arrowPressedType == 1 || arrowPressedType == 3)
                        moveAmount = new Vector2(moveAmount.x, 0);
                    else if (arrowPressedType == 2 || arrowPressedType == 4)
                        moveAmount = new Vector2(0, moveAmount.y);

                    // float x = Mathf.Round(newPos.x * 100) / 100;
                    // posXText.text = x.ToString();
                    // float y = Mathf.Round(newPos.y * 100) / 100;
                    // posYText.text = y.ToString();
                    // newPos = new Vector2(x, y);
                    if (o != null)
                        o.UpdateBasePosition(moveAmount, true);


                }
                lastClickedPos = pos;
                return;
            }

            Vector2 newPos = lastObjPos - moveAmount;

            if (arrowPressedType == 1 || arrowPressedType == 3)
                newPos = new Vector2(newPos.x, lastObjPos.y);
            else if (arrowPressedType == 2 || arrowPressedType == 4)
                newPos = new Vector2(lastObjPos.x, newPos.y);
            // float x = Mathf.Round(newPos.x * 100) / 100;
            // posXText.text = x.ToString();
            // float y = Mathf.Round(newPos.y * 100) / 100;
            // posYText.text = y.ToString();
            // newPos = new Vector2(x, y);

            if (currentSelectedPostionerObject != null)
            {
                DynamicValueAdder.instance.EditPositionerObject(ReturnRoundedPosition(newPos));
            }
            else if (currentSelectedObject != null)
                currentSelectedObject.UpdateBasePosition(ReturnRoundedPosition(newPos));


        }
        else if (moveCamera)
        {

            Vector2 moveAmount = lastClickedPos - pos;
            float newX = Camera.main.transform.position.x + moveAmount.x;
            newX = Mathf.Clamp(newX, -30, 30);
            Camera.main.transform.position = new Vector3(newX, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
    }
    private bool resetMovePostion = false;
    public void ResetMovePosition(Vector2 newPosition)
    {
        lastObjPos = newPosition;
        resetMovePostion = true;
    }

    public PositionerObject GetPositionerObjectFromTouchPosition(Vector2 worldPoint)
    {
        if (isPlayModeView) return null;


        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        PositionerObject obj = null;


        bool arrowFound = false;

        if (hits.Length == 0)
        {
            Debug.Log("NO OBJECTS FOUND");
            return null;
        }


        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Arrow"))
            {
                Debug.Log("Touched Arrow");


                if (hit.collider.name == "X_P")
                {
                    arrowPressedType = 3;
                    arrowFound = true;
                    return null;

                }

                else if (hit.collider.name == "Y_P")
                {
                    arrowFound = true;
                    arrowPressedType = 4;
                    return null;
                }



                break; // Prioritize Arrow and exit loop
            }
        }

        if (!arrowFound)
        {

            foreach (var hit in hits)
            {
                Debug.Log("Touched: " + hit.collider.name);
                if (hit.collider.CompareTag("RecordableObject"))
                {

                    if (hit.collider.GetComponent<PositionerObject>() != null)
                    {
                        var p = hit.collider.GetComponent<PositionerObject>();
                        int pressIndex = p.ReturnIndex();
                        if (pressIndex != currentSelectedPostionerObjectIndex)
                        {
                            obj = p;
                            // currentSelectedPostionerObjectIndex = pressIndex;
                            // DynamicValueAdder.instance.SetCurrentSelectedValue(pressIndex);
                        }
                        else
                        {
                            return p;
                        }



                    }

                }

            }
            if (obj != null)
            {
                currentSelectedPostionerObjectIndex = obj.ReturnIndex();
                DynamicValueAdder.instance.SetCurrentSelectedValue(obj.ReturnIndex());
                return obj;
            }

        }

        return null;

    }
    private RecordableObjectPlacer GetObjectFromTouchPosition(Vector2 worldPoint)
    {
        Debug.Log("Is play mode is: " + isPlayModeView);
        if (isPlayModeView && !isEditor) return null;


        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        RecordableObjectPlacer obj = null;
        bool arrowFound = false;

        if (hits.Length == 0)
        {
            Debug.Log("NO OBJECTS FOUND");
            return null;
        }

        if (isPlayModeView && isEditor)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<CheckPointFlag>() != null)
                {
                    Debug.Log("Touched Checkpoint");
                    CheckPointFlag flag = hit.collider.gameObject.GetComponent<CheckPointFlag>();
                    checkPointFlags.Remove(flag);
                    Destroy(flag.gameObject);

                }

            }
            return null;
        }


        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Cage"))
            {
                Debug.Log("Touched Cage");
                currentCageObject = hit.collider.gameObject.GetComponent<ChicAndCageRecordable>();
                SetCageReady(true);
                return null; // Skip objects on the Ignore Raycast layer
            }

            if (hit.collider.CompareTag("Arrow"))
            {

                obj = FindRecordableParent(hit.collider.transform);

                if (hit.collider.name == "X")
                    arrowPressedType = 1;
                else
                    arrowPressedType = 2;

                arrowFound = true;
                break; // Prioritize Arrow and exit loop
            }
        }

        if (!arrowFound)
        {

            foreach (var hit in hits)
            {
                Debug.Log("Touched: " + hit.collider.name);
                if (hit.collider.CompareTag("RecordableObject"))
                {
                    Debug.Log("Touched Gizmo");
                    obj = hit.collider.GetComponent<RecordableObjectPlacer>();
                    break;
                }
                else if (hit.collider.CompareTag("Plane"))
                {
                    Debug.Log("Touched Pig");
                    obj = FindRecordableParent(hit.collider.transform);
                    break;
                }

            }
        }

        return obj;
    }
    #region CageLogic

    private ChicAndCageRecordable currentCageObject;


    private bool readyToPressForCage = false;

    public void DeleteCageObject()
    {
        if (currentCageObject != null)
        {
            currentCageObject.RemoveSelf();
            currentCageObject = null;
            UpdateTime(CurrentTimeStep, false);
        }
        SetCageReady(false);
    }
    public void SetCageReady(bool ready)
    {

        if (ready)
        {
            if (currentCageObject != null)
            {
                currentCageObject.SetSelected(true);
            }
            readyToPressForCage = true;
            if (currentSelectedObject != null)
            {
                currentSelectedObject.SetIsSelected(false);
            }
            OnSendSpecialDataToActiveObjects?.Invoke(0, true);
        }
        else
        {
            if (currentCageObject != null)
            {
                currentCageObject.SetSelected(false);
                currentCageObject = null;
            }
            readyToPressForCage = false;

            OnSendSpecialDataToActiveObjects?.Invoke(0, false);
        }
        ValueEditorManager.instance.ShowCagePanel(ready);

    }

    public GameObject ReturnSpecialObject(int t)
    {
        return Instantiate(specialAddedObjects[t]);
    }

    #endregion
    public Vector2 ReturnRoundedPosition(Vector2 pos, bool unkown = false, bool grounded = false)
    {
        float x = Mathf.Round(pos.x / 0.2f) * 0.2f;
        // Keep two decimal places

        float y = Mathf.Round(pos.y / 0.2f) * 0.2f;

        if (grounded) y = pos.y;
        if (unkown)
        {
            posXText.text = "?";
            posYText.text = "?";
        }
        else
        {
            posXText.text = x.ToString("0.0");
            posYText.text = y.ToString("0.0");
        }

        return new Vector2(x, y);
    }

    // private void SetPositionText(Vector2 pos, bool unkown = false)
    // {
    //     if (unkown)
    //     {
    //         posXText.text = "?";
    //         posYText.text = "?";
    //     }
    //     else
    //     {
    //         posXText.text = pos.x.ToString("0.0");
    //         posYText.text = pos.y.ToString("0.0");
    //     }

    // }
    private int typeOvveride = -1;
    public void SetObjectToBeAdded(GameObject obj, int overrideType)
    {
        if (readyToPressForCage) SetCageReady(false);
        objectToAdd = obj;
        goingToAddObject = true;
        typeOvveride = overrideType;
        // Debug.Log("Going to add object");
    }
    void Start()
    {
        Debug.Log("Left View and Right view boundaries: " + BoundariesManager.leftViewBoundary + " " + BoundariesManager.rightViewBoundary);
        AudioManager.instance.LoadVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f), 0);

        HandleReleaseClick();



        Time.timeScale = 0;
        cameraY = Camera.main.transform.position.y;
        currentTime = CurrentTimeStep * TimePerStep;

        float totalSeconds = currentTime / FrameRateManager.BaseTimeScale;

        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

        // Format time as MM:SS.DD
        timeText.text = $"{minutes:00}:{seconds:00}.{decimals:00}";
        if (isEditor)
            stepText.text = "Step: " + CurrentTimeStep;
        LoadAssets();
        CheckAllObjectsForNewTimeStep();
        StartCoroutine(InvokeTimeAfterDelay());
        parameterUI.SetActive(false);
        UpdateTime(CurrentTimeStep, false);



    }

    private IEnumerator InvokeTimeAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        InitializeViewParameters();
        LevelRecordManager.SetGlobalTime?.Invoke(CurrentTimeStep, 0);
        SortRecordedObjectsBySpawnTime();

    }

    private void CheckAllObjectsForNewTimeStep()
    {

        for (int i = 0; i < RecordedObjects.Count; i++)
        {

            // Debug.Log("Checking Objects in list, current pbject: " + RecordedObjects[i].spawnedTimeStep + " unspawnTimestep: " + RecordedObjects[i].unspawnedTimeStep + "Current time step: " + CurrentTimeStep);
            if (RecordedObjects[i].spawnedTimeStep <= CurrentTimeStep && RecordedObjects[i].unspawnedTimeStep > CurrentTimeStep)
            {

                if (!RecordedObjects[i].gameObject.activeInHierarchy)
                    RecordedObjects[i].SetActiveFromList();
            }


        }
    }

    private void SetPreloadTimeStep()
    {
        int start = CurrentTimeStep;
        for (int i = 0; i < RecordedObjects.Count; i++)
        {
            if (RecordedObjects[i].gameObject.activeInHierarchy && RecordedObjects[i].spawnedTimeStep < start)
            {
                start = RecordedObjects[i].spawnedTimeStep;
            }


        }
        PreloadObjectsTimeStep = (ushort)start;
        Debug.Log("Preload time step set to: " + PreloadObjectsTimeStep + " with current time step: " + CurrentTimeStep);

    }
    public Sprite GetIconImage()
    {
        return currentSelectedObject.GetIcon();
    }


    #region Loading and Saving
    private bool dataLoaded = false;


#if UNITY_EDITOR
    private void CreateLastSave(Vector3Int worldNum)
    {
        string l = $"{worldNum.x}-{worldNum.y}-{worldNum.z}";
        PlayerPrefs.SetString("LastEditorLevel", l);
        PlayerPrefs.Save();
    }
    private Vector3Int ReturnLastEditorLevel()
    {
        string l = PlayerPrefs.GetString("LastEditorLevel", "0-0-0");

        string[] parts = l.Split('-');
        return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));

    }
    public void OpenLevelPicker(bool open)
    {
        ignoreAllClicks = open;
        if (open)
        {
            levelPicker.SetActive(true);

        }
        else if (dataLoaded)
        {
            levelPicker.SetActive(false);

        }


    }

    public void LoadNewLevel(LevelData data)
    {
        OpenLevelPicker(false);
        if (data != null)
        {
            CreateLastSave(data.levelWorldAndNumber);
            LevelDataConverter.instance.SetCurrentLevelInstance(data.levelWorldAndNumber);
            // reload the scene
            ResetStaticParameters();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }


    }
#endif

    public void LoadAssets()
    {
        RecordedObjects = new List<RecordableObjectPlacer>();
        string addedTitleText = "";
        LevelDataArrays dataArrays = null;
        if (isEditor)
        {
#if UNITY_EDITOR

            LevelDataConverter.instance.SetCurrentLevelInstance(ReturnLastEditorLevel());
            if (LevelDataConverter.instance.ReturnLevelData() != null && LevelDataConverter.currentLevelInstance > 0)
            {
                levelData = LevelDataConverter.instance.ReturnLevelData();
                dataArrays = levelData.ReturnDataArrays();
                dataLoaded = true;

                OpenLevelPicker(false);
                if (levelData.levelWorldAndNumber.z <= 0)
                    addedTitleText = $"{levelData.levelWorldAndNumber.x:00}-{levelData.levelWorldAndNumber.y:00} ";
                else
                    addedTitleText = $"{levelData.levelWorldAndNumber.x:00}-{levelData.levelWorldAndNumber.y:00}-{levelData.levelWorldAndNumber.z:00} ";

                if (levelData.checkPointSteps != null && levelData.checkPointSteps.Length > 0)
                    for (int i = 0; i < levelData.checkPointSteps.Length; i++)
                    {
                        var o = Instantiate(checkPointFlagPrefab, new Vector2(BoundariesManager.rightBoundary, -.49f), Quaternion.Euler(new Vector3(0, 0, 90))).GetComponent<CheckPointFlag>();
                        o.SetSpawnedTimeStep(levelData.checkPointSteps[i]);

                        checkPointFlags.Add(o);
                        o.gameObject.SetActive(false);
                    }
            }
            else
            {
                dataLoaded = false;
                OpenLevelPicker(true);
            }
#endif
        }
        // levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Levels/" + levelType.ToString() + "/" + levelWorldAndNumber + LevelName + ".asset");
        else
        {
            levelData = LevelDataConverter.instance.ReturnLevelData(0);

            dataArrays = levelData.ReturnDataArrays(LevelDataConverter.instance.ConvertDataFromJson(), true);

        }



        if (levelData == null)
        {
            Debug.Log("No level data found");
            return;
        }

        LevelDataConverter.instance.SetCurrentLevelInstance(levelData.levelWorldAndNumber);

        LevelTitleText.text = addedTitleText + levelData.LevelName;
        var data = LevelDataConverter.instance.ReturnDynamicData(levelData, dataArrays);

        // logic to set min and max view
        finalSpawnStep = levelData.finalSpawnStep;
        if (finalSpawnStep < 50) finalSpawnStep = 50;
        // if (currentSavedMaxIndex > finalSpawnStep)
        // {
        //     currentSavedMaxIndex = finalSpawnStep;
        // }
        currentSavedMaxIndex = finalSpawnStep;

        CustomTimeSlider.instance.SetVariables(CurrentTimeStep, currentSavedMinIndex, currentSavedMaxIndex);
        CustomTimeSlider.instance.SetMaxTime(finalSpawnStep);

        for (int i = 0; i < data.Count; i++)
        {
            // RecordableObjectPlacer obj;
            // if (data[i].ID < 0)
            // {
            //     obj = Instantiate(recordableObjectsWithNegtiveID[Mathf.Abs(data[i].ID) - 1]);
            // }
            // else
            //     obj = Instantiate(recordableObjectsByID[data[i].ID]);

            RecordableObjectPlacer obj = Instantiate(ReturnObjectByPoolType(data[i].ObjectType, data[i].ID));

            if (obj == null)
            {
                Debug.Log("No object found");
                return;
            }
            obj.LoadAssetFromSave(data[i]);
            obj.SetIsSelected(false);
            obj.gameObject.SetActive(false);
            RecordedObjects.Add(obj);
        }


    }
    public void SaveAsset()
    {
        currentSelectedObject = null;
        showLinesTemp = ShowLines;
        showParamsTemp = ShowParameters;
        currentTimeStepTemp = CurrentTimeStep;
        CurrentPlayTimeStep = CurrentTimeStep;

        SortRecordedObjectsBySpawnTime();
        PreloadPlayScene();

        ushort lastTimeStep = 0;
        int lastSpawnedIndex = 0;


        // int[] poolSizes = new int[recordableObjectsByID.Length];
        ushort[] pigPoolSizes = new ushort[pigsByID.Length];
        ushort[] aiPoolSizes = new ushort[aiByID.Length];
        ushort[] builderPoolSizes = new ushort[buildingsByID.Length];
        ushort[] collectablePoolSizes = new ushort[collectablesByID.Length];
        ushort[] positionerPoolSizes = new ushort[positionersByID.Length];
        ushort[] ringPoolSizes = new ushort[ringsByID.Length];

        List<ushort[]> poolSizesList = new List<ushort[]>();
        poolSizesList.Add(pigPoolSizes);
        poolSizesList.Add(aiPoolSizes);
        poolSizesList.Add(builderPoolSizes);
        poolSizesList.Add(collectablePoolSizes);
        poolSizesList.Add(positionerPoolSizes);
        poolSizesList.Add(ringPoolSizes);









        List<RecordableObjectPlacer> randomObjects = new List<RecordableObjectPlacer>();

        //remove random spawn objects from RecordedObjects and add to randomObjects list
        for (int i = RecordedObjects.Count - 1; i >= 0; i--)
        {
            if (RecordedObjects[i].Data.randomSpawnData != Vector2Int.zero)
            {
                randomObjects.Add(RecordedObjects[i]);
                RecordedObjects.RemoveAt(i);
            }
        }

        // reorganize RandomObjects by randomSPawnData.x then .y
        randomObjects = randomObjects.OrderBy(o => o.Data.randomSpawnData.x).ThenBy(o => o.Data.randomSpawnData.y).ToList();


        List<RecordableObjectPlacer> activeObjects = new List<RecordableObjectPlacer>();
        for (ushort s = 0; s < finalSpawnStep; s++)
        {
            if (lastSpawnedIndex >= RecordedObjects.Count) break;
            bool addedMore = false;
            for (int i = lastSpawnedIndex; i < RecordedObjects.Count; i++)
            {
                if (RecordedObjects[i].spawnedTimeStep == s)
                {
                    if (RecordedObjects[i].ID >= 0 && (!RecordedObjects[i].typeIsVarient || (RecordedObjects[i].typeIsVarient && RecordedObjects[i].Data.type == 0)))
                    {
                        activeObjects.Add(RecordedObjects[i]);
                        addedMore = true;
                    }


                    lastSpawnedIndex++;
                    if (lastSpawnedIndex >= RecordedObjects.Count) break;



                }
                else break;

            }
            if (!addedMore) continue;
            // int[] poolSizeCompare = new int[recordableObjectsByID.Length];
            int[] pigPoolSizeCompare = new int[pigsByID.Length];
            int[] aiPoolSizeCompare = new int[aiByID.Length];
            int[] builderPoolSizeCompare = new int[buildingsByID.Length];
            int[] collectablePoolSizeCompare = new int[collectablesByID.Length];
            int[] positionerPoolSizeCompare = new int[positionersByID.Length];
            int[] ringPoolSizeCompare = new int[ringsByID.Length];
            List<int[]> poolSizeCompareList = new List<int[]>();
            poolSizeCompareList.Add(pigPoolSizeCompare);
            poolSizeCompareList.Add(aiPoolSizeCompare);
            poolSizeCompareList.Add(builderPoolSizeCompare);
            poolSizeCompareList.Add(collectablePoolSizeCompare);
            poolSizeCompareList.Add(positionerPoolSizeCompare);
            poolSizeCompareList.Add(ringPoolSizeCompare);


            for (int o = 0; o < activeObjects.Count; o++)
            {
                // Debug.Log("Checking object: " + activeObjects[o].name + " with ID: " + activeObjects[o].ID + " and pool size: " + poolSizes.Length);
                if (!activeObjects[o].CheckForPoolSizes(s) || activeObjects[o].Data.randomSpawnData != Vector2Int.zero) activeObjects[o] = null;

                // else poolSizeCompare[activeObjects[o].ID]++;

                else poolSizeCompareList[activeObjects[o].ObjectType][activeObjects[o].ID]++;

            }

            // for (int i = 0; i < poolSizeCompare.Length; i++)
            // {
            //     if (poolSizeCompare[i] > poolSizes[i]) poolSizes[i] = poolSizeCompare[i];
            // }

            for (int o = 0; o < poolSizeCompareList.Count; o++)
            {
                for (int i = 0; i < poolSizeCompareList[o].Length; i++)
                {
                    if (poolSizeCompareList[o][i] > poolSizesList[o][i]) poolSizesList[o][i] = (ushort)poolSizeCompareList[o][i];
                }
            }


            activeObjects.RemoveAll(x => x == null);


        }

        // for (int i = 0; i < poolSizes.Length; i++)
        // {
        //     Debug.Log("Pool size for index: " + i + " is: " + poolSizes[i]);
        // }






        if (isEditor)
        {
            List<ushort> flags = new List<ushort>();

            if (checkPointFlags.Count > 0)
            {
                for (int i = 0; i < checkPointFlags.Count; i++)
                {
                    flags.Add(checkPointFlags[i].spawnedTimeStep);
                }

                flags.Sort();
                levelData.checkPointSteps = flags.ToArray();

            }
            else
            {
                levelData.checkPointSteps = null;
            }



            LevelDataConverter.instance.SaveDataToDevice(RecordedObjects, randomObjects, "", poolSizesList, finalSpawnStep, levelData.StartingAmmos, levelData.StartingLives, levelData);
            // Level data added at end to save it directly to scriptable object

        }
        else
        {
            string path = PlayerPrefs.GetString("LevelCreatorPath");
            if (path == null || path == "")
            {
                Debug.Log("No path found");
                return;
            }


            LevelDataConverter.instance.SaveDataToDevice(RecordedObjects, randomObjects, path, poolSizesList, finalSpawnStep, levelData.StartingAmmos, levelData.StartingLives);
            Debug.Log("Saved with final spawn step: " + finalSpawnStep);

        }





        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

    }

    #endregion
    public static string LevelPath = "NAMEBU";



    public void EditMaxTime(int dif)
    {
        dif = Mathf.RoundToInt(dif / (TimePerStep / FrameRateManager.BaseTimeScale));
        int newMax = finalSpawnStep + dif;
        int min = 49;
        if (RecordedObjects.Count > 0)
        {
            min = RecordedObjects[RecordedObjects.Count - 1].spawnedTimeStep;

            if (min < 49) min = 49;
        }


        if (newMax <= min)
        {
            newMax = min + 1;

        }
        finalSpawnStep = (ushort)newMax;
        CustomTimeSlider.instance.SetMaxTime(finalSpawnStep);
    }



    // public LevelData GetLevelData()
    // {
    //     if (levelType == LevelType.Main)
    //         levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Levels/" + levelType.ToString() + "/" + levelWorldAndNumber + LevelName + ".asset");
    //     else
    //         levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Levels/" + levelType.ToString() + "/" + LevelName);

    //     if (levelData == null)
    //     {
    //         Debug.Log("No level data found");
    //         return null;
    //     }
    //     else return levelData;

    // }
    private void AddNewObjectToList(RecordableObjectPlacer newObj)
    {
        RecordedObjects.Add(newObj);
        SortRecordedObjectsBySpawnTime();
    }
    public void SortRecordedObjectsBySpawnTime()
    {

        RecordedObjects.Sort((a, b) =>
        {
            // 1. First, sort by spawnedTimeStep
            int timeStepComparison = a.spawnedTimeStep.CompareTo(b.spawnedTimeStep);
            if (timeStepComparison != 0)
                return timeStepComparison;

            // 2. If time steps are the same, sort by ID
            int idComparison = a.ID.CompareTo(b.ID);
            if (idComparison != 0)
                return idComparison;

            // 3. If ID is also the same, sort by absolute startPos.x (lower absolute values first)
            return Mathf.Abs(a.Data.startPos.x).CompareTo(Mathf.Abs(b.Data.startPos.x));
        });

        CustomTimeSlider.instance.SetSpawnedTimeSteps(ReturnSpawnedSteps());
    }

    public List<int> ReturnSpawnedSteps()
    {
        List<int> steps = new List<int>();
        int lastSpawnStep = -1;
        for (int i = 0; i < RecordedObjects.Count; i++)
        {
            if (lastSpawnStep != RecordedObjects[i].spawnedTimeStep)
            {
                int s = RecordedObjects[i].spawnedTimeStep;
                steps.Add(s);
                lastSpawnStep = s;
            }

        }
        return steps;
    }

    private void OnEnable()
    {
        controls.LevelCreator.Enable();
        // LevelRecordManager.AddNewObject += SetObjectToBeAdded;
        LevelRecordManager.PressTimerBar += PressTimeBarWhilePlaying;
        if (isEditor) SetGlobalTime += CheckGlobalTime;
    }

    private void OnDisable()
    {
        controls.LevelCreator.Disable();

        // timeSlider.onValueChanged.RemoveAllListeners();
        // LevelRecordManager.AddNewObject -= SetObjectToBeAdded;
        LevelRecordManager.PressTimerBar -= PressTimeBarWhilePlaying;
        if (isEditor) SetGlobalTime -= CheckGlobalTime;



    }

    private float currentTime = 0;
    private bool isPlayingAtNormalTime = false;
    public void PlayNormalSpeed()
    {

        if (!isPlayingAtNormalTime)
        {


            Debug.Log("Starting normal speed");
            isPlayingAtNormalTime = true;
            currentTime = CurrentTimeStep * TimePerStep;
            PlayOrPauseImage.sprite = PauseImage;
            NormalTimeCor = StartCoroutine(PlayAtNormalTime());

        }
        else
        {
            Debug.Log("Stopping normal speed");

            if (NormalTimeCor != null)
            {
                PlayOrPauseImage.sprite = PlayImage;
                isPlayingAtNormalTime = false;

                StopCoroutine(NormalTimeCor);
                NormalTimeCor = null;

                // Round the current time to the nearest time step
                int roundedTimeStep = Mathf.RoundToInt(currentTime / TimePerStep);

                CurrentTimeStep = (ushort)roundedTimeStep;

                // UpdateTime(CurrentTimeStep);

                // Update UI elements

                Debug.Log("Updating Time In Normal Speed");

                UpdateTime(CurrentTimeStep);

            }
        }
    }



    public void PressTimeBarWhilePlaying()
    {
        if (NormalTimeCor != null && isPlayingAtNormalTime)
        {
            isPlayingAtNormalTime = false;

            StopCoroutine(NormalTimeCor);
            NormalTimeCor = null;

            // Round the current time to the nearest time step
            int roundedTimeStep = Mathf.RoundToInt(currentTime / TimePerStep);

            CurrentTimeStep = (ushort)roundedTimeStep;

            // UpdateTime(CurrentTimeStep);

            // Update UI elements

            Debug.Log("Updating Time In Press Time Bar");

            UpdateTime(CurrentTimeStep);

        }

    }





    private IEnumerator PlayAtNormalTime()
    {
        while (true)
        {
            currentTime += Time.unscaledDeltaTime * FrameRateManager.BaseTimeScale;
            LevelRecordManager.SetGlobalTime?.Invoke(0, currentTime);
            float totalSeconds = currentTime / FrameRateManager.BaseTimeScale;

            int minutes = Mathf.FloorToInt(totalSeconds / 60);
            int seconds = Mathf.FloorToInt(totalSeconds % 60);
            int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

            // Format time as MM:SS.DD
            timeText.text = $"{minutes:00}:{seconds:00}.{decimals:00}";
            int roundedTimeStep = Mathf.RoundToInt(currentTime / TimePerStep);

            if (CurrentTimeStep != (ushort)roundedTimeStep)
            {
                CurrentTimeStep = (ushort)roundedTimeStep;
                if (isEditor)
                    stepText.text = "Step: " + CurrentTimeStep;
                CustomTimeSlider.instance.UpdateMainHandlePosition(CurrentTimeStep);
                CustomTimeSlider.instance.SetMainHandlePosition(CurrentTimeStep);

                minuteHand.eulerAngles = new Vector3(0, 0, -CurrentTimeStep);
                hourHand.eulerAngles = new Vector3(0, 0, -CurrentTimeStep / 12);
                CheckAllObjectsForNewTimeStep();

            }


            // Update UI elements

            yield return null;

        }


    }

    // Update is called once per frame


    public void UpdateObjectTime(int val)
    {
        if (currentSelectedObject != null)
            currentSelectedObject.UpdateTimeStep(val);
    }


    private List<int> spawnStepsPerObject;
    public void SetChangeMultipleObjectSpawnStep(bool pressed)
    {
        if (spawnStepsPerObject == null) spawnStepsPerObject = new List<int>();
        if (pressed)
        {

            foreach (var o in MultipleSelectedObjects)
            {
                if (o != null)
                {
                    spawnStepsPerObject.Add(o.spawnedTimeStep);
                }
            }

        }
        else
        {
            spawnStepsPerObject.Clear();
            spawnStepsPerObject = new List<int>();
        }
    }

    public void DoingTimeEdit(bool holding)
    {
        if (holding)
        {

        }
        else
        {
            SortRecordedObjectsBySpawnTime();
        }

    }


    public void UpdateTime(ushort val, bool playView = false)
    {

        if (isPlayingAtNormalTime) return;
        if (playView)
        {
            CurrentPlayTimeStep = val;
            CurrentTimeStep = val;
            LevelRecordManager.SetGlobalTime?.Invoke(val, 0);
            playTimeText.text = "Start Time: " + FormatTimerText(CurrentPlayTimeStep);
            CustomTimeSlider.instance.SetMainHandlePosition(CurrentTimeStep);
            CheckAllObjectsForNewTimeStep();
            return;
        }
        else
        {
            CurrentTimeStep = val;
            CustomTimeSlider.instance.SetMainHandlePosition(CurrentTimeStep);
        }

        // Debug.Log("Updated time step to: " + val);
        LevelRecordManager.SetGlobalTime?.Invoke(val, 0);

        // Convert time steps to real-world time (factoring in custom time scale)
        float totalSeconds = (val * TimePerStep) / FrameRateManager.BaseTimeScale;

        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

        // Format time as MM:SS.DD
        timeText.text = $"{minutes:00}:{seconds:00}.{decimals:00}";
        if (isEditor)
            stepText.text = "Step: " + CurrentTimeStep;
        // Rotate clock hands
        minuteHand.eulerAngles = new Vector3(0, 0, -val);
        hourHand.eulerAngles = new Vector3(0, 0, -val / 12);
        CheckAllObjectsForNewTimeStep();
    }





    private RecordableObjectPlacer FindRecordableParent(Transform child)
    {
        int i = 0;
        while (child != null || i < 10)
        {
            if (child.CompareTag("RecordableObject"))
            {
                return child.GetComponent<RecordableObjectPlacer>(); // Found the correct parent
            }
            child = child.parent; // Move up to the next parent
            i++;
        }
        return null; // No valid parent found
    }
    [field: SerializeField] public GameObject projectileLinePrefab;
    [field: SerializeField] public GameObject positionAtTimePrefab;
    public Queue<GameObject> ProjectileLines { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> PositionAtTimes { get; private set; } = new Queue<GameObject>();


    private void CreatePools()
    {
        projectileLinePrefab.GetComponent<SpriteRenderer>().color = colorSO.SelectedLineColor;
        for (int i = 0; i < 80; i++)
        {
            if (i == 0)
            {
                ProjectileLines.Enqueue(projectileLinePrefab);
                projectileLinePrefab.SetActive(false);
            }

            else
            {
                var o = Instantiate(projectileLinePrefab);
                ProjectileLines.Enqueue(o);
                o.SetActive(false);
            }

        }

        // for (int i = 0; i < 40; i++)
        // {
        //     if (i == 0)
        //     {
        //         PositionAtTimes.Enqueue(positionAtTimePrefab);
        //         positionAtTimePrefab.SetActive(false);
        //     }

        //     else
        //     {
        //         var o = Instantiate(positionAtTimePrefab);
        //         PositionAtTimes.Enqueue(o);
        //         o.SetActive(false);
        //     }

        // }

    }

    public GameObject ReturnPooledRecordingObject(int type)
    {
        if (type == 0)
        {
            if (PositionAtTimes.Count <= 0)
            {
                var o = Instantiate(positionAtTimePrefab);

                return o;
            }
            else return PositionAtTimes.Dequeue();
        }
        else if (type == 1)
        {
            if (ProjectileLines.Count <= 0)
            {
                var o = Instantiate(projectileLinePrefab);

                return o;
            }
            else return ProjectileLines.Dequeue();
        }

        else return null;

    }
    private RecordableObjectPlacer ReturnObjectByPoolType(short objectType, short id)
    {
        //Pools, Pigs 0, AI 1, Buildings 2, Collectables 3, Positioners 4, Rings 5
        switch (objectType)
        {
            case 0: // PositionerObject
                return pigsByID[id];

                break;
            case 1: // ProjectileObject
                return aiByID[id];

                break;
            case 2: // BuildingObject
                return buildingsByID[id];

                break;
            case 3: // CollectableObject
                return collectablesByID[id];

                break;
            case 4: // PositionerObject
                return positionersByID[id];

                break;
            case 5: // RingObject
                return ringsByID[id];

                break;

            default:
                Debug.LogError("Invalid object type: " + objectType);
                return null;


        }

    }
    public void ReturnPooledObjectToQ(GameObject obj, int type)
    {

        obj.SetActive(false);
        if (type == 0)
        {

            PositionAtTimes.Enqueue(obj);
        }
        else if (type == 1)
        {

            ProjectileLines.Enqueue(obj);
        }

    }

    public void FadeTitleText(bool fade)
    {
        if (fade)
        {
            LevelTitleText.color = Color.white * .25f;
        }
        else
        {
            LevelTitleText.color = Color.white;
        }
    }

    #region MultipleObjectSelection

    public void UpdateMultipleObjectsTime(int change)
    {
        for (int i = 0; i < MultipleSelectedObjects.Count; i++)
        {
            Debug.Log("Chaning object time step");
            if (MultipleSelectedObjects[i] != null)
            {
                MultipleSelectedObjects[i].UpdateTimeStep(change + spawnStepsPerObject[i], true);
            }
        }

    }
    public void SetUsingMultipleSelect(bool isOn)
    {
        multipleObjectsSelected = isOn;

        if (isOn)
        {
            if (currentSelectedObject != null)
            {
                MultipleSelectedObjects.Add(currentSelectedObject);
            }

        }
        else
        {
            DeleteButtonForMultSelectTime.SetActive(false);
            for (int i = 0; i < MultipleSelectedObjects.Count; i++)
            {
                if (MultipleSelectedObjects[i] != null)
                {
                    MultipleSelectedObjects[i].SetIsSelected(false);
                }
            }
            MultipleSelectedObjects.Clear();
            MultipleSelectedObjects = new List<RecordableObjectPlacer>();
            if (currentSelectedObject != null)
            {
                currentSelectedObject.SetIsSelected(false);
                currentSelectedObject = null;
            }


            multipleSelectedIDs = false;
            parameterUI.SetActive(false);

            CustomTimeSlider.instance.ExitMultipleObjectSelection();
            if (spawnStepsPerObject != null)
            {
                spawnStepsPerObject.Clear();
                spawnStepsPerObject = null;
            }

        }
        CheckViewParameters?.Invoke();

    }

    public void SetMultipleSelected(int min, int max)
    {
        foreach (var o in RecordedObjects)
        {
            if (o.spawnedTimeStep >= min && o.spawnedTimeStep <= max)
            {
                if (!MultipleSelectedObjects.Contains(o))
                {
                    MultipleSelectedObjects.Add(o);
                    o.SetIsSelected(true);
                }
            }
            else
            {
                if (MultipleSelectedObjects.Contains(o))
                {
                    o.SetIsSelected(false);
                    MultipleSelectedObjects.Remove(o);
                }
            }
        }

        if (MultipleSelectedObjects.Count > 0 && !DeleteButtonForMultSelectTime.activeInHierarchy)
            DeleteButtonForMultSelectTime.SetActive(true);
        else if (MultipleSelectedObjects.Count <= 0 && DeleteButtonForMultSelectTime.activeInHierarchy)
            DeleteButtonForMultSelectTime.SetActive(false);

    }

    #endregion


    public void AddFlag()
    {

        var o = Instantiate(checkPointFlagPrefab, new Vector2(BoundariesManager.rightBoundary, -.49f), Quaternion.Euler(new Vector3(0, 0, 90))).GetComponent<CheckPointFlag>();
        o.SetSpawnedTimeStep(CurrentTimeStep);
        checkPointFlags.Add(o);


        // sort the flags in ascending order

    }

    public void CheckGlobalTime(ushort step, float realTime)
    {
        float time = step * TimePerStep;
        if (realTime > 0)
        {
            time = realTime;
        }
        foreach (var o in checkPointFlags)
        {
            Vector2 pos = o.GetPositionAtStep(time);
            if (pos == Vector2.zero && o.gameObject.activeInHierarchy)
            {
                o.gameObject.SetActive(false);
            }
            else if (pos != Vector2.zero)
            {
                if (!o.gameObject.activeInHierarchy)
                {
                    o.gameObject.SetActive(true);
                }
                o.transform.position = pos;
            }
        }

    }

    public string FormatTimerText(int step)
    {
        float totalSeconds = (step * LevelRecordManager.TimePerStep) / FrameRateManager.BaseTimeScale;

        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

        // Format time as MM:SS.DD
        return $"{minutes:00}:{seconds:00}.{decimals:00}";
    }

    public string FormatVectorText(Vector2 vec)
    {
        float x = Mathf.Round(vec.x * 100) / 100;
        float y = Mathf.Round(vec.y * 100) / 100;
        return "X: " + x.ToString("0.0") + " Y: " + y.ToString("0.0");
    }

    public float ConvertLevelTimeToRealTime(float levelTime)
    {
        return levelTime / FrameRateManager.BaseTimeScale;
    }





}
