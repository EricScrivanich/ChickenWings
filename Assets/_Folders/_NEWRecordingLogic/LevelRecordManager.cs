using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine.EventSystems;

public class LevelRecordManager : MonoBehaviour, IPointerDownHandler
{
    public static LevelRecordManager instance;



    public enum LevelType
    {
        Main,
        DownloadedCustom,
        OwnerCustom
    }
    [SerializeField] private GameObject PlayTimeObject;
    [SerializeField] private GameObject ViewObject;

    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private Image PlayOrPauseImage;
    [SerializeField] private Sprite PlayImage;
    [SerializeField] private Sprite PauseImage;

    public LevelCreatorColors colorSO;

    public RecordableObjectPlacer[] recordableObjectsByID;
    public RecordableObjectPlacer[] recordableObjectsWithNegtiveID;

    [SerializeField] private TextMeshProUGUI posXText;
    [SerializeField] private TextMeshProUGUI posYText;






    private int arrowPressedType = 0;


    public LevelType levelType;
    public string LevelName;
    public string levelWorldAndNumber;
    public ushort levelStepLength = 400;

    private InputController controls;
    public static readonly float TimePerStep = .18f;
    public static ushort CurrentTimeStep { get; private set; }
    public static ushort CurrentPlayTimeStep { get; private set; }
    public static int currentSavedMinIndex { get; private set; }
    public static int currentSavedMaxIndex { get; private set; }

    public LevelData levelData;

    public List<RecordableObjectPlacer> RecordedObjects;

    [SerializeField] private RectTransform minuteHand;
    [SerializeField] private RectTransform hourHand;


    // [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI timeText;


    public static Action<ushort, float> SetGlobalTime;

    public static Action<int> OnShowObjectTime;


    public static Action PressTimerBar;
    private GameObject objectToAdd;

    private CanvasGroup editorCanvasGroup;




    private bool objectClicked = false;
    private bool screenClicked = false;

    public RecordableObjectPlacer currentSelectedObject { get; private set; }

    public List<RecordableObjectPlacer> MultipleSelectedObjects = new List<RecordableObjectPlacer>();

    private Vector2 lastObjPos;
    private Vector2 lastCamPos;
    private Vector2 lastClickedPos;

    private Coroutine NormalTimeCor;

    public LevelDataEditors[] floatSliders;
    private float cameraY;


    private bool moveCamera = false;
    private bool goingToAddObject = false;

    private Vector2 posOnDragNew = new Vector2(0, .5f);

    [Header("UI Elements")]


    [SerializeField] private GameObject parameterUI;
    [SerializeField] private GameObject folderUI;

    public bool editingPostionerObject = false;

    public PositionerObject currentSelectedPostionerObject;
    public int currentSelectedPostionerObjectIndex;

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



    public static void ResetStaticParameters()
    {
        ShowLines = true;
        ShowSpeeds = true;
        ShowFolders = true;
        ShowParameters = true;

        ShowTime = false;
        CurrentTimeStep = 0;
        CurrentPlayTimeStep = 0;
        currentSavedMinIndex = 0;
        currentSavedMaxIndex = 100;


    }

    public void SetMinAndMax(int min, int max)
    {
        currentSavedMinIndex = min;
        currentSavedMaxIndex = max;
    }

    public static Action CheckViewParameters;
    public void InitializeViewParameters()
    {
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



    }

    public void EnterPlayTime(bool enter)
    {
        if (enter)
        {
            isPlayModeView = true;
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
        RecordableObjectPlacer obj = null;
        if (currentSelectedObject != null) currentSelectedObject.SetIsSelected(false);
        if (currentSelectedObject.Data.ID >= 0)
        {
            obj = Instantiate(recordableObjectsByID[currentSelectedObject.ID]);
        }
        else
        {
            obj = Instantiate(recordableObjectsWithNegtiveID[Mathf.Abs(currentSelectedObject.ID) - 1]);
        }



        RecordedDataStructDynamic copy = currentSelectedObject.Data;
        obj.LoadAssetFromSave(copy);

        AddNewObjectToList(obj);
        obj.UpdateBasePosition(new Vector2(currentSelectedObject.transform.position.x + currentSelectedObject.speedChanger, currentSelectedObject.transform.position.y));
        currentSelectedObject = obj;
        obj.SetSelectedObject();

    }

    public void DeleteObject()
    {
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

    public void DeleteAll()
    {
        for (int i = 0; i < RecordedObjects.Count; i++)
        {
            RecordedObjects[i].DestroySelf();
        }
        RecordedObjects.Clear();
    }


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

        currentSelectedObject = null;

        ShowFolders = true;
        ShowTime = false;

        controls = new InputController();
        CreatePools();



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

    private void HandleClickObject(Vector2 pos)
    {
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
            currentSelectedPostionerObject = null;
            if (!parameterUI.activeInHierarchy && ShowParameters)
            {
                parameterUI.SetActive(true);

            }
            if (currentSelectedObject != null && currentSelectedObject != obj) currentSelectedObject.SetIsSelected(false);
            currentSelectedObject = obj;

            obj.SetSelectedObject();
            obj.HandleClick(true, arrowPressedType);
            ReturnRoundedPosition(obj.transform.position);
            lastObjPos = obj.transform.position;

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
        }
        else
        {
            editorCanvasGroup.alpha = 1;
            editorCanvasGroup.blocksRaycasts = true;
        }

    }



    public void UnactivateSelectedObject()
    {
        if (currentSelectedObject == null) return;
        currentSelectedObject.SetIsSelected(false);
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
            Vector2 moveAmount = lastClickedPos - pos;

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
        if (isPlayModeView) return null;


        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        RecordableObjectPlacer obj = null;
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


    private Vector2 ReturnRoundedPosition(Vector2 pos)
    {
        float x = Mathf.Round(pos.x / 0.2f) * 0.2f;
        posXText.text = x.ToString("0.0"); // Keep two decimal places

        float y = Mathf.Round(pos.y / 0.2f) * 0.2f;
        posYText.text = y.ToString("0.0");
        return new Vector2(x, y);
    }
    private int typeOvveride = -1;
    public void SetObjectToBeAdded(GameObject obj, int overrideType)
    {
        objectToAdd = obj;
        goingToAddObject = true;
        typeOvveride = overrideType;
        // Debug.Log("Going to add object");
    }
    void Start()
    {
        AudioManager.instance.LoadVolume(1, 0);
        HandleReleaseClick();
        CustomTimeSlider.instance.SetVariables(CurrentTimeStep, currentSavedMinIndex, currentSavedMaxIndex);
        UpdateTime(CurrentTimeStep, false);


        Time.timeScale = 0;
        cameraY = Camera.main.transform.position.y;
        currentTime = CurrentTimeStep * TimePerStep;

        float totalSeconds = currentTime / FrameRateManager.BaseTimeScale;

        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

        // Format time as MM:SS.DD
        timeText.text = $"{minutes:00}:{seconds:00}.{decimals:00}";
        LoadAssets();
        CheckAllObjectsForNewTimeStep();
        StartCoroutine(InvokeTimeAfterDelay());
        parameterUI.SetActive(false);


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
    public Sprite GetIconImage()
    {
        return currentSelectedObject.GetIcon();
    }
    public void SaveAsset()
    {

        SortRecordedObjectsBySpawnTime();

        ushort lastTimeStep = 0;
        int lastSpawnedIndex = 0;

        int[] poolSizes = new int[recordableObjectsByID.Length];



        List<RecordableObjectPlacer> activeObjects = new List<RecordableObjectPlacer>();
        for (ushort s = 0; s < levelStepLength; s++)
        {
            if (lastSpawnedIndex >= RecordedObjects.Count) break;
            bool addedMore = false;
            for (int i = lastSpawnedIndex; i < RecordedObjects.Count; i++)
            {
                if (RecordedObjects[i].spawnedTimeStep == s)
                {
                    if (RecordedObjects[i].ID >= 0)
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
            int[] poolSizeCompare = new int[recordableObjectsByID.Length];

            for (int o = 0; o < activeObjects.Count; o++)
            {
                Debug.Log("Checking object: " + activeObjects[o].name + " with ID: " + activeObjects[o].ID + " and pool size: " + poolSizes.Length);
                if (!activeObjects[o].CheckForPoolSizes(s)) activeObjects[o] = null;

                else poolSizeCompare[activeObjects[o].ID]++;
            }

            for (int i = 0; i < poolSizeCompare.Length; i++)
            {
                if (poolSizeCompare[i] > poolSizes[i]) poolSizes[i] = poolSizeCompare[i];
            }
            activeObjects.RemoveAll(x => x == null);


        }

        for (int i = 0; i < poolSizes.Length; i++)
        {
            Debug.Log("Pool size for index: " + i + " is: " + poolSizes[i]);
        }






        if (levelType == LevelType.Main)
        {
            // AssetDatabase.CreateAsset(savedData, "Assets/Levels/" + levelType.ToString() + "/" + levelWorldAndNumber + LevelName + ".asset");


        }
        else
        {
            string path = PlayerPrefs.GetString("LevelCreatorPath");
            if (path == null || path == "")
            {
                Debug.Log("No path found");
                return;
            }


            LevelDataConverter.instance.ConvertDataToJson(RecordedObjects, path, poolSizes, finalSpawnStep, levelData.startingStats.startingAmmos, levelData.startingStats.StartingLives);
            Debug.Log("Saved with final spawn step: " + finalSpawnStep);

        }





        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

    }
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
    public void LoadAssets()
    {
        RecordedObjects = new List<RecordableObjectPlacer>();
        if (levelType == LevelType.Main) Debug.Log("Loading main level");
        // levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Levels/" + levelType.ToString() + "/" + levelWorldAndNumber + LevelName + ".asset");
        else
        {
            levelData.LoadData();
        }

        if (levelData == null)
        {
            Debug.Log("No level data found");
            return;
        }
        var data = LevelDataConverter.instance.ReturnDynamicData(levelData);
        finalSpawnStep = levelData.finalSpawnStep;
        if (finalSpawnStep < 50) finalSpawnStep = 50;


        CustomTimeSlider.instance.SetMaxTime(finalSpawnStep);

        for (int i = 0; i < data.Count; i++)
        {
            RecordableObjectPlacer obj;
            if (data[i].ID < 0)
            {
                obj = Instantiate(recordableObjectsWithNegtiveID[Mathf.Abs(data[i].ID) - 1]);
            }
            else
                obj = Instantiate(recordableObjectsByID[data[i].ID]);

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
    }

    private void OnDisable()
    {
        controls.LevelCreator.Disable();

        // timeSlider.onValueChanged.RemoveAllListeners();
        // LevelRecordManager.AddNewObject -= SetObjectToBeAdded;
        LevelRecordManager.PressTimerBar -= PressTimeBarWhilePlaying;




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
    void Update()
    {

    }

    public void UpdateObjectTime(int val)
    {
        if (currentSelectedObject != null)
            currentSelectedObject.UpdateTimeStep(val);
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
