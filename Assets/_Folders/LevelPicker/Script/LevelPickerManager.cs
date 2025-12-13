using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;



public class LevelPickerManager : MonoBehaviour, INavigationUI
{
    public static LevelPickerManager instance;
    [SerializeField] private bool allowClickAll;
    [SerializeField] private GameObject steroidButton;
    [SerializeField] private RectTransform handLevelPick;
    public float xHillMult;
    public float yHillMult;
    public float scaleHillMult;
    private InputController controls;
    [SerializeField] private GameObject uiButton;

    public static Action<ILevelPickerPathObject> OnLevelPickerObjectSelected;




    [SerializeField] private PathCreator[] paths;
    [SerializeField] private bool[] pathIntersectsCenter;
    [SerializeField] private PathIntersection[] pathIntersections;
    [SerializeField] private UnlockableMapItem[] unlockableMapItems;
    [SerializeField] private PlayerLevelPickerPathFollwer playerPathFollower;
    [SerializeField] private GameObject levelUiPopupPrefab;
    private CanvasGroup[] levelPopups;
    [SerializeField] private Transform levelPopupParent;

    [SerializeField] private GameObject[] levelPickerPathObjects;
    private ILevelPickerPathObject[] levelPickerObjs;
    [SerializeField] private ILevelPickerPathObject[] baseCamDatas;
    private Sequence adjustHillSeq;

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
    private float moveSideDuration = 1.3f;
    private float swingDuration = .4f;

    [Header("Hill Parents and Transform Settings")]
    [SerializeField] private Transform frontHillParent;
    [SerializeField] private Vector2 frontHillStartPos;
    [SerializeField] private Transform backHillParent;

    [SerializeField] private Vector2 backHillStartPos;
    [Header("Cam Movement Settings")]
    [SerializeField] private float delayToMoveCam;
    [SerializeField] private float moveCamDuration;

    [Header("Additonal Parralax Objects")]

    [SerializeField] private Transform[] additionalParralaxObjects;
    [SerializeField] private Vector2[] parralaxMovementMultipliers;
    public Vector2[] additionalParralaxObjectPositions;

    public Action OnUpdateUIPostions;

    private readonly Vector3[] rotations = new Vector3[]
      {
        new Vector3(0, 0, 7.5f),
        new Vector3(0, 0, -6f),
        new Vector3(0, 0, 2.5f),
        new Vector3(0, 0, -1.2f),
        new Vector3(0, 0, .3f),

        Vector3.zero
      };



    private Sequence popupSequence;
    private Sequence signYSeq;
    private Sequence signXSeq;

    private int currentPopupIndex;
    public Vector4 BaseCameraData { get; private set; } = new Vector4(0, 0, -10, 5.5f);
    [SerializeField] private float frontHillMultiplier;
    private int currentPathIndex;
    private int currentPathRoot = -2;
    private int nextPathRoot;
    private List<LevelButtonUI> levelButtons;


    private Camera cam;

    [Header("Camera settings")]
    private Vector2 camVelRef;
    private float camOrthoRef;
    [SerializeField] private float camSmoothTime;
    [SerializeField] private float camMaxSpeed;
    [SerializeField] private float updatePathDistanceTime;
    [SerializeField] private float updatePathDistanceThreshold;

    void OnEnable()
    {
        controls.LevelCreator.Enable();
        StartCoroutine(HandleStarTweens());
        OnLevelPickerObjectSelected += HandleSelectObject;
    }
    void OnDisable()
    {
        controls.LevelCreator.Disable();
        OnLevelPickerObjectSelected -= HandleSelectObject;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
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
    public void SetAdditionalObjectPostions()
    {
        additionalParralaxObjectPositions = new Vector2[additionalParralaxObjects.Length];
        for (int i = 0; i < additionalParralaxObjects.Length; i++)
        {
            additionalParralaxObjectPositions[i] = additionalParralaxObjects[i].localPosition;
        }
    }

    public Transform RetrunPlayerParentByLayer(int layer)
    {
        if (layer == 3)
            return frontHillParent;
        else if (layer == -1)
            return backHillParent;

        else
            return backHillParent;
    }

    private GameObject firstSelectedUI;
    private bool ignoreAll;

    public void DoNextLevelAfterUnlock()
    {
        ignoreAll = false;
        StartCoroutine(WaitToDoNextLevel(-1));
    }

    private IEnumerator WaitToDoNextLevel(short NextUnlockedItem)
    {

        yield return new WaitForSecondsRealtime(.1f);

        if (NextUnlockedItem >= 0)
        {
            if (currentTarget != null)
                ZoomSeq(currentTarget.CameraPositionAndOrhtoSize, currentTarget.layersShownFrom);
            Debug.Log("Next unlocked item to unlock: " + NextUnlockedItem);
            for (short i = 0; i < unlockableMapItems.Length; i++)
            {

                if (unlockableMapItems[i].mapItemID == NextUnlockedItem)
                {
                    unlockableMapItems[i].UnlockItem(playerPathFollower.unlockableTargetTransform.position);
                    ignoreAll = true;

                    yield break;


                }
            }
        }



        else if (PlayerPrefs.GetString("NextLevel", "Menu") != "Menu")
        {
            Vector3Int nextLevel = ReturnLevelAsVector(PlayerPrefs.GetString("NextLevel", "Menu"));
            Debug.LogError("Loading next level: " + nextLevel);

            for (int i = 0; i < levelPickerObjs.Length; i++)
            {

                var obj = levelPickerObjs[i].GetComponent<ILevelPickerPathObject>();


                if (obj.WorldNumber == nextLevel)
                {
                    // currentTarget = obj;
                    // CreateLastSave(obj.WorldNumber);
                    firstSelectedUI = levelButtons[i].gameObject;


                    HandleSelectObject(obj);
                    // LevelDataConverter.currentChallengeType = currentTarget.challengeType;
                    // Vector3Int data = obj.RootIndex_PathIndex_Order;

                    // float d = paths[data.y].path.GetClosestDistanceAlongPath(obj.ReturnLinePostion());
                    // playerPathFollower.DoPathToPoint(paths[data.y], d, null, null, null, null);
                    // DoLevelPopupSeq(true, obj.WorldNumber);
                    InputSystemSelectionManager.instance.SetNewWindow(this, false);
                    PlayerPrefs.SetString("NextLevel", "Menu");
                    PlayerPrefs.Save();
                    yield break;
                }
            }
            if (InputSystemSelectionManager.instance != null && InputSystemSelectionManager.instance.tutorialType == 0)
                firstSelectedUI = steroidButton;

            InputSystemSelectionManager.instance.SetNewWindow(this, false);
            PlayerPrefs.SetString("NextLevel", "Menu");
            PlayerPrefs.Save();
        }
        else if (currentTarget != null)
        {
            // InputSystemSelectionManager.instance.SetNewWindow(this, false);

            ZoomSeq(currentTarget.CameraPositionAndOrhtoSize, currentTarget.layersShownFrom);
            InputSystemSelectionManager.instance.SetNewWindow(this, false);
            // yield return new WaitForSecondsRealtime(.1f);
            // currentTarget.SetSelected(false);
        }
        // yield return new WaitForSecondsRealtime(.1f);
        currentTarget = null;





    }

    private void Start()
    {
        playerPathFollower.SetPathManager(this);
        AudioManager.instance.LoadVolume(PlayerPrefs.GetFloat("MusicVolume", .5f), PlayerPrefs.GetFloat("SFXVolume", 1.0f));

        string s = PlayerPrefs.GetString("LastLevel", "1-1-0");
        Vector3Int lastLevel = ReturnLevelAsVector(s);
        LevelDataConverter.instance.LoadWorldData(lastLevel.x);
        // LevelDataConverter.instance.ReturnAndLoadWorldLevelData(null, lastLevel.x); // loading current world

        short NextUnlockedItem = LevelDataConverter.instance.GetNextUnlockedItem((ushort)lastLevel.x);
        Vector3Int numberToCheck = LevelDataConverter.instance.CurrentFurthestLevel();
        frontHillParent.localScale = Vector3.one;

        frontHillParent.localPosition = frontHillStartPos;
        backHillParent.localPosition = backHillStartPos;
        // Camera.main.transform.position = new Vector3(0, 0, -10);



        List<ILevelPickerPathObject> pathObjects = new List<ILevelPickerPathObject>();

        List<short> unlockedItems = LevelDataConverter.instance.ReturnUnlockedItems();

        if (unlockedItems != null && unlockedItems.Count > 0)
        {
            for (int i = 0; i < unlockableMapItems.Length; i++)
            {
                unlockableMapItems[i].Initializeitem(this, unlockedItems.Contains(unlockableMapItems[i].mapItemID) && unlockableMapItems[i].mapItemID != NextUnlockedItem);
            }
        }
        else
        {
            for (int i = 0; i < unlockableMapItems.Length; i++)
            {

                unlockableMapItems[i]?.Initializeitem(this, false);
            }
        }


        levelButtons = new List<LevelButtonUI>();

        for (int i = 0; i < levelPickerPathObjects.Length; i++)
        {
            var l = levelPickerPathObjects[i].GetComponent<ILevelPickerPathObject>();
            l.SetOrder(i);
            var ui = Instantiate(uiButton, levelPopupParent).GetComponent<RectTransform>();
            levelButtons.Add(ui.GetComponent<LevelButtonUI>());
            l.SetButtonRect(ui);

            if (i == 0)
            {
                handLevelPick.transform.SetParent(ui);
                handLevelPick.anchoredPosition = new Vector2(200, 40);
            }

            int beatenType = 0;

            if (LevelDataConverter.instance.CheckIfLevelCompleted(i, new Vector2Int(l.WorldNumber.y, l.WorldNumber.z)))
            {
                beatenType = 2;
            }
            else if (i > 0)
            {
                if (l.WorldNumber.z > 0)
                {
                    if (LevelDataConverter.instance.CheckIfLevelCompleted(i - 1, new Vector2Int(l.WorldNumber.y, l.WorldNumber.z - 1)))
                    {
                        beatenType = 1;
                    }
                }

                else
                {
                    if (LevelDataConverter.instance.CheckIfLevelCompleted(i - 1, new Vector2Int(l.WorldNumber.y - 1, l.WorldNumber.z)))
                    {
                        beatenType = 1;
                    }

                }

            }
            else
            {
                beatenType = 1;
            }
            l.SetBeatenType(beatenType);
            l.SetLastSelectable(numberToCheck, beatenType);
            if (l.WorldNumber == lastLevel)
            {
                Debug.Log("Last level found: " + lastLevel);
                Vector3Int data = l.RootIndex_PathIndex_Order;

                float d = paths[data.y].path.GetClosestDistanceAlongPath(l.ReturnLinePostion());
                playerPathFollower.SetInitialPostionAndLayer(paths[data.y], d);
                currentPathIndex = data.y;
                currentPathRoot = data.x;

                if (InputSystemSelectionManager.instance != null && InputSystemSelectionManager.instance.tutorialType == -1)
                    firstSelectedUI = ui.gameObject;
                currentTarget = l;

            }

            var nums = l.WorldNumber;
            // if (nums.y < numberToCheck.y)
            // {

            // }
            pathObjects.Add(l);




        }



        levelPickerObjs = pathObjects.ToArray();
        Debug.LogError("Next Unlocked Item in LevelPickerManager: " + NextUnlockedItem);

        StartCoroutine(WaitToDoNextLevel(NextUnlockedItem));


    }

    public GameObject GetFirstSelected()
    {
        return firstSelectedUI;
    }

    public GameObject GetNextSelected()
    {
        return firstSelectedUI;
    }


    private int currentPlayerPath;
    private ILevelPickerPathObject currentTarget;

    public void SetCurrentPathIndexAndRoot(int index)
    {
        currentPathIndex = index;
        // currentPathRoot = pathRoot;
        Debug.LogError("Setting Current Path Index to: " + index);
    }
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

    private void HandleSelectObject(ILevelPickerPathObject obj)
    {
        HandleClickObject(Vector2.zero, obj);
    }

    public float GetPathDisntaceBasedOnIndexLogic(PathCreator path, int type)
    {
        switch (type)
        {
            case -1:
                return -1;
            case 0:
                return 0;
            case 1:
                return path.path.length;
            default:
                return 0;
        }
    }

    private void SendPathDataToPlayer(ILevelPickerPathObject obj)
    {



        if (currentTarget != null)
        {
            currentTarget.SetSelected(false);
        }
        currentTarget = obj;
        CreateLastSave(obj.WorldNumber);


        currentTarget.SetSelected(true);
        LevelDataConverter.currentChallengeType = currentTarget.challengeType;
        Vector3Int data = obj.RootIndex_PathIndex_Order;

        float d = paths[data.y].path.GetClosestDistanceAlongPath(obj.ReturnLinePostion());


        if (data.y != currentPathIndex)
        {
            var (pathRoute, intersectionDistances) = GetPathDataFromIntersections(obj);

            List<PathCreator> addedPaths = new List<PathCreator>();
            List<float> currentPlayerDistances = new List<float>();
            List<float> dists = new List<float>();
            List<int> pathIndices = new List<int>();
            List<int> pathRoots = new List<int>();



            float inbetweenDistance = 0;
            PathCreator currentPath = paths[pathRoute[0]];
            float initialDistance = intersectionDistances[0];

            bool isPlayerDistance = true;

            for (int i = 1; i < pathRoute.Count; i++)
            {
                addedPaths.Add(paths[pathRoute[i]]);
                pathIndices.Add(pathRoute[i]);


            }

            for (int i = 1; i < intersectionDistances.Count; i++)
            {
                if (isPlayerDistance)
                {
                    currentPlayerDistances.Add(intersectionDistances[i]);


                }
                else
                {
                    dists.Add(intersectionDistances[i]);

                }
                isPlayerDistance = !isPlayerDistance;

            }
            dists.Add(d);


            playerPathFollower.DoPathToPoint(currentPath, initialDistance, addedPaths, currentPlayerDistances, dists, pathIndices, pathRoots, currentTarget.GetPathTransform());






            // if (currentPathRoot == data.x)
            // {

            //     inbetweenDistance = 0;
            //     float distanceFirst = paths[currentPathRoot].path.GetClosestDistanceAlongPath(paths[data.y].path.GetPointAtDistance(0));
            //     dists.Add(distanceFirst);
            //     dists.Add(d);
            //     pathIndices.Add(currentPathRoot);
            //     pathIndices.Add(data.y);
            //     addedPaths.Add(this.paths[currentPathRoot]);
            //     addedPaths.Add(this.paths[data.y]);
            //     pathRoots.Add(currentPathRoot - 1);
            //     pathRoots.Add(currentPathRoot);

            // }
            // else
            // {
            //     dists.Add(d);
            //     pathIndices.Add(data.y);
            //     addedPaths.Add(paths[data.y]);
            //     pathRoots.Add(data.x);
            //     if (currentPathIndex < data.y)
            //     {
            //         Debug.LogError("Current Path Index: " + currentPathIndex + " is less than target path index: " + data.y);
            //         if (pathIntersectsCenter[data.y])

            //             inbetweenDistance = paths[currentPathIndex].path.GetClosestDistanceAlongPath(paths[data.y].path.GetPointAtDistance(0));
            //         else
            //             inbetweenDistance = paths[currentPathIndex].path.length;


            //     }
            //     else
            //     {
            //         inbetweenDistance = 0;
            //     }
            // }


        }
        else
        {
            playerPathFollower.DoPathToPoint(paths[currentPathIndex], d, null, null, null, null, null, currentTarget.GetPathTransform());

        }

        Debug.LogError("Moving to level: " + obj.WorldNumber);
        DoLevelPopupSeq(true, obj.WorldNumber);

        ZoomSeq(currentTarget.CameraPositionAndOrhtoSize, currentTarget.layersShownFrom);

    }

    private (List<int> pathRoute, List<float> intersectionDistances) GetPathDataFromIntersections(ILevelPickerPathObject obj)
    {
        int targetPathIndex = obj.RootIndex_PathIndex_Order.y;

        // If we're already on the target path, no intersections needed
        if (currentPathIndex == targetPathIndex)
        {
            return (new List<int> { currentPathIndex }, new List<float>());
        }

        // BFS to find the shortest path through intersections
        // Each queue entry is a tuple: (path route, intersection distances in traversal order)
        Queue<(List<int> paths, List<float> distances)> pathQueue = new Queue<(List<int>, List<float>)>();
        HashSet<int> visitedPaths = new HashSet<int>();

        // Start with current path
        pathQueue.Enqueue((new List<int> { currentPathIndex }, new List<float>()));
        visitedPaths.Add(currentPathIndex);

        while (pathQueue.Count > 0)
        {
            var (currentPath, currentDistances) = pathQueue.Dequeue();
            int lastPathInRoute = currentPath[currentPath.Count - 1];

            // Search through all intersections to find connections from this path
            for (int intersectionIdx = 0; intersectionIdx < pathIntersections.Length; intersectionIdx++)
            {
                var intersection = pathIntersections[intersectionIdx];
                if (intersection == null || intersection.connectedPathsIndices == null)
                    continue;

                // Check if this intersection connects to our current path in the route
                int fromIndex = -1;
                for (int i = 0; i < intersection.connectedPathsIndices.Length; i++)
                {
                    if (intersection.connectedPathsIndices[i] == lastPathInRoute)
                    {
                        fromIndex = i;
                        break;
                    }
                }

                // If this intersection connects to our path, check all other connected paths
                if (fromIndex >= 0)
                {
                    for (int toIndex = 0; toIndex < intersection.connectedPathsIndices.Length; toIndex++)
                    {
                        int connectedPathIndex = intersection.connectedPathsIndices[toIndex];

                        // Skip the path we came from or already visited paths
                        if (toIndex == fromIndex || visitedPaths.Contains(connectedPathIndex))
                            continue;

                        // Create new route with this path added
                        List<int> newRoute = new List<int>(currentPath);
                        newRoute.Add(connectedPathIndex);

                        // Calculate actual distances based on connectedPathsDistances type
                        // 0 = start (distance 0), 1 = end (path.length), -1 = intersection position
                        List<float> newDistances = new List<float>(currentDistances);

                        int fromType = intersection.connectedPathsDistances[fromIndex];
                        int toType = intersection.connectedPathsDistances[toIndex];

                        float fromDistance = fromType switch
                        {
                            0 => 0f,
                            1 => paths[lastPathInRoute].path.length,
                            -1 => paths[lastPathInRoute].path.GetClosestDistanceAlongPath(intersection.transform.position),
                            _ => 0f
                        };

                        float toDistance = toType switch
                        {
                            0 => 0f,
                            1 => paths[connectedPathIndex].path.length,
                            -1 => paths[connectedPathIndex].path.GetClosestDistanceAlongPath(intersection.transform.position),
                            _ => 0f
                        };

                        newDistances.Add(fromDistance);
                        newDistances.Add(toDistance);

                        // Found the target!
                        if (connectedPathIndex == targetPathIndex)
                        {
                            for (int n = 0; n < newRoute.Count; n++)
                            {
                                Debug.LogWarning($"Path in route: {newRoute[n]}");
                            }
                            for (int n = 0; n < newDistances.Count; n += 2)
                            {
                                Debug.LogWarning($"Intersection distances: from={newDistances[n]}, to={newDistances[n + 1]}");
                            }
                            return (newRoute, newDistances);
                        }

                        // Otherwise, add to queue to continue searching
                        visitedPaths.Add(connectedPathIndex);
                        pathQueue.Enqueue((newRoute, newDistances));
                    }
                }
            }
        }

        // No path found - return empty lists as fallback
        Debug.LogWarning($"No path found from path {currentPathIndex} to path {targetPathIndex} using intersections");
        return (new List<int>(), new List<float>());
    }

    private void HandleClickObject(Vector2 screenPosition, ILevelPickerPathObject manualSelect = null)
    {
        if (ignoreAll || (InputSystemSelectionManager.instance.tutorialType >= 0 && InputSystemSelectionManager.instance.tutorialType < 3))
            return;

        if (InputSystemSelectionManager.instance.tutorialType == 4)
        {
            InputSystemSelectionManager.instance.tutorialType = 5;
            SteroidTutorial.instance.ShowNextHand(5);
        }
        ILevelPickerPathObject obj;
        // Debug.LogError("Screen Position: " + screenPosition);
        if (manualSelect == null)
        {
            if (IsPointerOverUI(screenPosition))
            {
                Debug.Log("Clicked on UI element, ignoring level picker click.");
                return;
            }

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPosition);

            obj = GetObjectFromTouchPosition(worldPoint);
        }
        else
        {
            obj = manualSelect;
        }

        if (obj.beatenType == 0 && !allowClickAll)
        {
            HapticFeedbackManager.instance.PlayerButtonFailure();
            return;
        }

        if (obj != null && (obj != currentTarget || manualSelect != null))
        {
            if (manualSelect == null)
                HapticFeedbackManager.instance.PressUIButton();
            SendPathDataToPlayer(obj);
            firstSelectedUI = levelButtons[obj.order].gameObject;

        }
        else
        {
            Debug.Log("No object found at the clicked position.");

        }



    }

    public void SetAsCurrentNavigationWindow()
    {
        InputSystemSelectionManager.instance.SetNewWindow(this, false);
    }
    private Sequence zoomSeq;
    [SerializeField] private Ease easeType;
    [SerializeField] private bool useOldZoomSeq;
    private void ZoomSeqOld(Vector4 data, int layerShownFrom = 0)
    {
        if (zoomSeq != null && zoomSeq.IsActive())
        {
            zoomSeq.Kill();
        }
        zoomSeq = DOTween.Sequence();
        if (data == Vector4.zero)
            data = BaseCameraData;
        float mult = BaseCameraData.w / data.w;
        Vector2 move = new Vector2(data.x * xHillMult, data.y * yHillMult) * -mult;
        move += frontHillStartPos;
        float dur = moveCamDuration;
        DoLayerStuff(layerShownFrom, delayToMoveCam + dur, easeType);
        float scale = mult * scaleHillMult;

        if (UpdateUIPositionsCoroutine != null)
        {
            StopCoroutine(UpdateUIPositionsCoroutine);
        }

        UpdateUIPositionsCoroutine = StartCoroutine(UpdateUIPositions(delayToMoveCam + dur));



        zoomSeq.AppendInterval(delayToMoveCam);
        zoomSeq.Append(Camera.main.DOOrthoSize(data.w, dur));

        if ((Vector2)frontHillParent.localPosition != move)
            zoomSeq.Join(frontHillParent.DOLocalMove(move, dur));
        if (frontHillParent.localScale.x != scale)
            zoomSeq.Join(frontHillParent.DOScale(scale, dur));

        zoomSeq.Join(Camera.main.transform.DOMove(new Vector3(data.x, data.y, data.z), dur));
        for (int i = 0; i < additionalParralaxObjects.Length; i++)
        {
            zoomSeq.Join(additionalParralaxObjects[i].DOLocalMove(additionalParralaxObjectPositions[i] - new Vector2(data.x * parralaxMovementMultipliers[i].x, data.y * parralaxMovementMultipliers[i].y), dur));
        }
        zoomSeq.Play().SetEase(easeType).SetUpdate(true);


    }
    private void ZoomSeq(Vector4 data, int layerShownFrom = 0)
    {
        if (useOldZoomSeq)
        {
            ZoomSeqOld(data, layerShownFrom);
            return;
        }

        Ease ease = easeType;

        if (doingZoomSeq)
            ease = Ease.OutSine;
        if (data == Vector4.zero)
            data = BaseCameraData;

        float mult = BaseCameraData.w / data.w;
        float dur = moveCamDuration;

        // === Hill ===
        Vector2 hillPos = new Vector2(data.x * xHillMult, data.y * yHillMult) * -mult;
        hillPos += frontHillStartPos;

        float hillScale = mult * scaleHillMult;

        // === Stop UI Reposition coroutine ===
        if (UpdateUIPositionsCoroutine != null)
            StopCoroutine(UpdateUIPositionsCoroutine);

        UpdateUIPositionsCoroutine = StartCoroutine(UpdateUIPositions(delayToMoveCam + dur));

        // === Camera & Hills tween — INTERRUPTIBLE AND SMOOTH === //

        // Camera zoom
        Camera.main.DOOrthoSize(data.w, dur)
                   .SetEase(ease)
                   .SetUpdate(true);

        // Camera move
        Camera.main.transform.DOMove(
            new Vector3(data.x, data.y, data.z),
            dur
        )
        .SetEase(ease)
        .SetUpdate(true);

        // Hill move
        frontHillParent.DOLocalMove(hillPos, dur * .9f)
                       .SetEase(ease)
                       .SetUpdate(true);

        // Hill scale
        frontHillParent.DOScale(hillScale, dur * .9f)
                       .SetEase(ease)
                       .SetUpdate(true);

        // Additional parallax objects
        for (int i = 0; i < additionalParralaxObjects.Length; i++)
        {
            Vector3 target = additionalParralaxObjectPositions[i] -
                             new Vector2(data.x * parralaxMovementMultipliers[i].x,
                                         data.y * parralaxMovementMultipliers[i].y);

            additionalParralaxObjects[i].DOLocalMove(target, dur)
                       .SetEase(ease)
                       .SetUpdate(true);
        }

        // Layers fade
        DoLayerStuff(layerShownFrom, delayToMoveCam + dur, ease);
    }

    // private Coroutine camMoveCoroutine;
    // private Vector2 cameraTargetPos;
    // private Vector2 hillFrontTargetPos;
    // private Vector2 hillBackTargetPos;
    // private float hillFrontTargetScale;
    // private float cameraTargetOrtho;
    // private bool movingCamera = false;
    // private Vector2 hillFrontVelRef;
    // private Vector2 hillBackVelRef;
    // private float hillFrontScaleRef;

    // private IEnumerator MoveCamaeraToTarget()
    // {
    //     movingCamera = true;
    //     while (Vector2.Distance((Vector2)cam.transform.position, cameraTargetPos) > 0.1f)
    //     {
    //         cam.transform.position = Vector2.SmoothDamp((Vector2)cam.transform.position, cameraTargetPos, ref camVelRef, camSmoothTime, camMaxSpeed, Time.unscaledDeltaTime);
    //         cameraTargetOrtho = Mathf.SmoothDamp(Camera.main.orthographicSize, cameraTargetOrtho, ref camOrthoRef, camSmoothTime, camMaxSpeed, Time.unscaledDeltaTime);

    //         yield return null;
    //     }
    //     cam.transform.position = new Vector3(cameraTargetPos.x, cameraTargetPos.y, cam.transform.position.z);
    //     Camera.main.orthographicSize = cameraTargetOrtho;
    //     camMoveCoroutine = null;
    //     movingCamera = false;
    // }
    private Coroutine UpdateUIPositionsCoroutine;

    private bool doingZoomSeq = false;
    private IEnumerator UpdateUIPositions(float dur)
    {
        float delay = .2f;
        doingZoomSeq = true;
        playerPathFollower.SetRecheckPathDistance(true);

        int steps = Mathf.CeilToInt(dur / delay);
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(delay);
            foreach (var b in levelButtons)
            {
                b.UpdateRectPosition();
            }

        }
        yield return new WaitForSeconds(delay);
        foreach (var b in levelButtons)
        {
            b.UpdateRectPosition();
        }
        doingZoomSeq = false;
        playerPathFollower.SetRecheckPathDistance(false);






    }
    public void DoLayerStuff(int layer, float dur, Ease ease)
    {
        foreach (var n in levelPickerObjs)
        {
            n.SetLayerFades(layer, dur, ease);
        }
    }
    public void SetHillPos(Vector4 data)
    {
        if (data == Vector4.zero)
            data = BaseCameraData;
        float mult = BaseCameraData.w / data.w;
        Vector2 move = new Vector2(data.x * xHillMult, data.y * yHillMult) * -mult;
        move += frontHillStartPos;
        frontHillParent.localPosition = move;
        frontHillParent.localScale = mult * scaleHillMult * Vector3.one;
        for (int i = 0; i < additionalParralaxObjects.Length; i++)
        {
            additionalParralaxObjects[i].localPosition = additionalParralaxObjectPositions[i] - (new Vector2(data.x * parralaxMovementMultipliers[i].x, data.y * parralaxMovementMultipliers[i].y) * mult);
        }

    }

    private void MoveHillSeq(bool revert, Vector2 frontPos, float frontScale, float zoom, Vector2 backPos, float dur)
    {
        return;
        dur = 1.5f;
        if (adjustHillSeq != null && adjustHillSeq.IsActive())
        {
            adjustHillSeq.Kill();
        }
        adjustHillSeq = DOTween.Sequence();
        if (revert)
        {
            frontPos = frontHillStartPos;
            // backPos = backHillStartPos;
            frontScale = 1;
            zoom = 5.2f;
        }
        else
        {
            frontPos = new Vector2(frontHillStartPos.x + frontPos.x, frontHillStartPos.y + frontPos.y);
            // backPos = new Vector2(backHillStartPos.x + backPos.x, backHillStartPos.y + backPos.y);
        }
        adjustHillSeq.AppendInterval(.3f);
        adjustHillSeq.Append(frontHillParent.DOLocalMove(frontPos, dur));
        adjustHillSeq.Join(frontHillParent.DOScale(frontScale, dur));
        adjustHillSeq.Join(Camera.main.DOOrthoSize(zoom, dur));
        // adjustHillSeq.Join(backHillParent.DOLocalMove(backPos, dur));
        adjustHillSeq.Play().SetEase(Ease.InOutSine).SetUpdate(true);
    }

    public void BackOutSpecial(float dur)
    {
        var target = currentPopup.GetComponent<RectTransform>();
        Sequence hideSeq = DOTween.Sequence();
        // hideSeq.Append(target.DOAnchorPosY(normalY - overShootY, overshootUpDuration));
        hideSeq.Append(target.DOAnchorPosY(topHiddenY, dur));
        hideSeq.Play();
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
        HapticFeedbackManager.instance.PressUIButton();

        Vector3Int worldNum = currentTarget.WorldNumber;
        Vector4 camData = Vector4.zero;

        for (int i = baseCamDatas.Length - 1; i >= 0; i--)
        {
            Vector3Int check = baseCamDatas[i].WorldNumber;
            Debug.LogError("Checking base cam data: " + check + " against world num: " + worldNum);
            if (check.x == worldNum.x && worldNum.y >= check.y)
            {

                camData = baseCamDatas[i].CameraPositionAndOrhtoSize;
                break;
            }


        }

        // MoveHillSeq(true, Vector2.zero, 0, 0, Vector2.zero, 1.3f);
        ZoomSeq(camData);
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

        if (currentPopup != null)
            currentPopup.GetComponent<LevelPickerUIPopup>().HideDifficultys(true);


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

        int dif = 1;
        if (currentTarget.difficultyType == 3) dif = 3;
        if (currentTarget.message != null && currentTarget.message != "")
            nextPopup.GetComponent<LevelPickerUIPopup>().ShowMessage(currentTarget.message);
        nextPopup.GetComponent<LevelPickerUIPopup>().ShowData(LevelDataConverter.instance.ReturnLevelData(), this, dif, false);
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
    private float tweenInInterval = .35f;
    private float tweenOutInterval = .3f;


    private IEnumerator HandleStarTweens()
    {

        yield return new WaitForSecondsRealtime(.2f);

        while (true)
        {
            for (int s = 0; s < 3; s++)
            {
                foreach (var l in levelPickerObjs)
                {
                    l.DoStarSeq(s, true);
                }

                yield return new WaitForSecondsRealtime(tweenInInterval);

            }
            for (int s = 0; s < 3; s++)
            {
                foreach (var l in levelPickerObjs)
                {
                    l.DoStarSeq(s, false);
                }

                yield return new WaitForSecondsRealtime(tweenOutInterval);

            }
            // foreach (var l in levelPickerObjs)
            // {
            //     l.DoStarSeq(i, true);
            // }
            // yield return new WaitForSecondsRealtime(tweenInInterval * .5f);

            // foreach (var l in levelPickerObjs)
            // {
            //     l.DoStarSeq(i, false);
            // }
            // yield return new WaitForSecondsRealtime(tweenOutInterval);
            // lastIndex = i;
            // i++;
            // if (i >= 3)
            // {
            //     i = 0;
            // }




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




    private void CreateLastSave(Vector3Int worldNum)
    {
        string l = $"{worldNum.x}-{worldNum.y}-{worldNum.z}";
        PlayerPrefs.SetString("LastLevel", l);
        PlayerPrefs.Save();
    }
    private Vector3Int ReturnLevelAsVector(string l)
    {
        Debug.Log("Returning level as vector for: " + l);

        string[] parts = l.Split('-');
        return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));

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
