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

    public RecordableObjectPlacer[] recordableObjectsByID;

    [SerializeField] private TextMeshProUGUI posXText;
    [SerializeField] private TextMeshProUGUI posYText;


    [field: SerializeField] public Slider valueSliderVertical { get; private set; }

    private SpriteRenderer arrowSprite;

    private int arrowPressedType = 0;


    public LevelType levelType;
    public string LevelName;
    public string levelWorldAndNumber;

    private InputController controls;
    public static readonly float TimePerStep = .18f;
    public static ushort CurrentTimeStep { get; private set; }

    public LevelData levelData;

    public List<RecordableObjectPlacer> RecordedObjects;

    [SerializeField] private RectTransform minuteHand;
    [SerializeField] private RectTransform hourHand;

    [SerializeField] private Slider timeSlider;
    // [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    public static Action<ushort, float> SetGlobalTime;
    public static Action<GameObject> AddNewObject;


    public static Action PressTimerBar;
    private GameObject objectToAdd;




    private bool objectClicked = false;
    private bool screenClicked = false;

    public RecordableObjectPlacer currentSelectedObject { get; private set; }

    private Vector2 lastObjPos;
    private Vector2 lastCamPos;
    private Vector2 lastClickedPos;

    private Coroutine NormalTimeCor;

    public LevelDataEditors[] floatSliders;
    private float cameraY;


    private bool moveCamera = false;
    private bool goingToAddObject = false;

    private Vector2 posOnDragNew = new Vector2(0, .5f);



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerDown(PointerEventData eventData)
    {
        lastCamPos = Camera.main.transform.position;
        moveCamera = true;

    }

    public void DuplicateObject()
    {

        if (currentSelectedObject != null) currentSelectedObject.SetIsSelected(false);
        var obj = Instantiate(recordableObjectsByID[currentSelectedObject.ID]);
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

        currentSelectedObject = null;
        HandleReleaseClick();

        CurrentTimeStep = 0;
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

    }

    private void HandleClickObject(Vector2 pos)
    {
        lastClickedPos = Camera.main.ScreenToWorldPoint(pos);

        var obj = GetObjectFromTouchPosition(lastClickedPos);

        screenClicked = true;


        if (obj != null)
        {
            if (currentSelectedObject != null) currentSelectedObject.SetIsSelected(false);
            currentSelectedObject = obj;

            obj.SetSelectedObject();
            obj.HandleClick(true, arrowPressedType);
            ReturnRoundedPosition(obj.transform.position);
            lastObjPos = obj.transform.position;

            objectClicked = true;

        }

    }

    private void HandleReleaseClick()
    {
        objectClicked = false;
        screenClicked = false;
        moveCamera = false;

        arrowPressedType = 0;
        if (currentSelectedObject != null)
            currentSelectedObject.HandleClick(false, 0);

    }

    private void HandleMoveObject(Vector2 pos)
    {
        if (screenClicked && goingToAddObject && pos.y > -5.3f)
        {
            Debug.Log("Adding object");
            if (currentSelectedObject != null) currentSelectedObject.SetIsSelected(false);
            currentSelectedObject = Instantiate(objectToAdd, pos, Quaternion.identity).GetComponent<RecordableObjectPlacer>();
            currentSelectedObject.CreateNew();
            AddNewObjectToList(currentSelectedObject);
            currentSelectedObject.SetSelectedObject();
            objectClicked = true;
            lastObjPos = Vector2.zero;
            goingToAddObject = false;
            lastClickedPos = posOnDragNew;
        }

        if (objectClicked)
        {
            Vector2 moveAmount = lastClickedPos - pos;

            Vector2 newPos = lastObjPos - moveAmount;

            if (arrowPressedType == 1)
                newPos = new Vector2(newPos.x, lastObjPos.y);
            else if (arrowPressedType == 2)
                newPos = new Vector2(lastObjPos.x, newPos.y);
            // float x = Mathf.Round(newPos.x * 100) / 100;
            // posXText.text = x.ToString();
            // float y = Mathf.Round(newPos.y * 100) / 100;
            // posYText.text = y.ToString();
            // newPos = new Vector2(x, y);
            currentSelectedObject.UpdateBasePosition(ReturnRoundedPosition(newPos));


        }
        else if (moveCamera)
        {
            Debug.Log("Moving Camera");
            Vector2 moveAmount = lastClickedPos - pos;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + moveAmount.x, cameraY, Camera.main.transform.position.z);
        }
    }
    private RecordableObjectPlacer GetObjectFromTouchPosition(Vector2 worldPoint)
    {

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
        float x = Mathf.Round(pos.x / 0.05f) * 0.05f;
        posXText.text = x.ToString("0.00"); // Keep two decimal places

        float y = Mathf.Round(pos.y / 0.05f) * 0.05f;
        posYText.text = y.ToString("0.00");
        return new Vector2(x, y);
    }

    private void SetObjectToBeAdded(GameObject obj)
    {
        objectToAdd = obj;
        goingToAddObject = true;
        // Debug.Log("Going to add object");
    }
    void Start()
    {
        Time.timeScale = 0;
        cameraY = Camera.main.transform.position.y;

        timeSlider.onValueChanged.AddListener(value => UpdateTime((ushort)value));
        timeSlider.value = 0;
        float totalSeconds = currentTime / FrameRateManager.BaseTimeScale;

        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        int decimals = Mathf.FloorToInt((totalSeconds - Mathf.Floor(totalSeconds)) * 100); // Get 2 decimal places

        // Format time as MM:SS.DD
        timeText.text = $"{minutes:00}:{seconds:00}.{decimals:00}";
        LoadAssets();
        CheckAllObjectsForNewTimeStep();
        StartCoroutine(InvokeTimeAfterDelay());


    }

    private IEnumerator InvokeTimeAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        LevelRecordManager.SetGlobalTime?.Invoke(CurrentTimeStep, 0);

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

    public void SaveAsset()
    {
        // LevelData savedData = ScriptableObject.CreateInstance<LevelData>();
        // savedData.levelName = LevelName;
        // savedData.levelWorldAndNumber = levelWorldAndNumber;

        // LevelDataConverter.instance.ConvertDataToArrays(RecordedObjects, savedData);


        if (levelType == LevelType.Main)
        {
            // AssetDatabase.CreateAsset(savedData, "Assets/Levels/" + levelType.ToString() + "/" + levelWorldAndNumber + LevelName + ".asset");


        }
        else
        {

            LevelDataConverter.instance.ConvertDataToJson(RecordedObjects, LevelName);

        }





        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

    }
    public static string LevelPath = "NAMEBU";
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

        for (int i = 0; i < data.Count; i++)
        {
            RecordableObjectPlacer obj = Instantiate(recordableObjectsByID[data[i].ID]);
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
    }

    private void OnEnable()
    {
        controls.LevelCreator.Enable();
        LevelRecordManager.AddNewObject += SetObjectToBeAdded;
        LevelRecordManager.PressTimerBar += PressTimeBarWhilePlaying;
    }

    private void OnDisable()
    {
        controls.LevelCreator.Disable();
        timeSlider.onValueChanged.RemoveAllListeners();
        LevelRecordManager.AddNewObject -= SetObjectToBeAdded;
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
            NormalTimeCor = StartCoroutine(PlayAtNormalTime());

        }
        else
        {
            Debug.Log("Stopping normal speed");

            if (NormalTimeCor != null)
            {
                isPlayingAtNormalTime = false;

                StopCoroutine(NormalTimeCor);
                NormalTimeCor = null;

                // Round the current time to the nearest time step
                int roundedTimeStep = Mathf.RoundToInt(currentTime / TimePerStep);

                CurrentTimeStep = (ushort)roundedTimeStep;

                // UpdateTime(CurrentTimeStep);

                // Update UI elements
                timeSlider.value = CurrentTimeStep;
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
            timeSlider.value = CurrentTimeStep;
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
                timeSlider.value = CurrentTimeStep;
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

    void UpdateTime(ushort val)
    {
        if (isPlayingAtNormalTime) return;
        CurrentTimeStep = val;
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

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks

        timeSlider.onValueChanged.RemoveAllListeners();
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


}
