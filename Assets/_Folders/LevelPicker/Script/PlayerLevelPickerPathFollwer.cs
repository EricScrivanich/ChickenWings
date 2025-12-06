using UnityEngine;
using PathCreation;
using System.Collections;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerLevelPickerPathFollwer : MonoBehaviour
{



    private float speed;
    [SerializeField] private Transform linePosition;



    public bool doPath;

    [SerializeField] private float baseSpeed = 1;

    [SerializeField] private Vector2 minMaxSpeed;
    [SerializeField] private float distanceSpeedModifier;
    [SerializeField] private Vector2 minMaxFootstepVolumeMultiplier;
    [SerializeField] private Vector2 minMaxFootstepTime;
    private float currentFootstepTime = 0;


    [SerializeField] private Vector2 minMaxScale;
    [SerializeField] private Vector2 minMaxZoom;

    private SortingGroup sortingGroup;

    [SerializeField] private ParticleSystem ps;

    private bool inForeGround = true;
    [SerializeField] private float foregroundZ;
    [SerializeField] private int pathIndex;

    private LevelPickerPathData pathData;
    private LevelPickerManager pathManager;
    private Animator anim;
    private Coroutine followPathCoroutine;



    void Awake()
    {
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
    }
    public void SetPathManager(LevelPickerManager p)
    {
        pathManager = p;
    }
    [SerializeField] private UnlockableMapItem mapItem;
    [field: SerializeField] public Transform unlockableTargetTransform { get; private set; }
    [SerializeField] private bool doAnim;
    [SerializeField] private bool resetAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



        // transform.position = paths[pathIndex].path.GetPoint(2);


    }
    private void OnValidate()
    {
        if (doAnim)
        {
            doAnim = false;
            mapItem.DoUnlockedAnimation(unlockableTargetTransform.position);
        }
        if (resetAnim)
        {
            resetAnim = false;
            mapItem.ResetUnlockAnimation();
        }
    }

    // private void OnValidate()
    // {
    //     if (doPath)
    //     {
    //         doPath = false;
    //         if (followPathCoroutine != null)
    //         {
    //             StopCoroutine(followPathCoroutine);
    //         }
    //         followPathCoroutine = StartCoroutine(FollowPath(pathIndex));



    //     }
    // }
    private bool stopParticles = false;
    public void ShowParticles()
    {

        if (isRunning && ps != null)
        {
            ps.Play();
        }
    }
    public void HideParticles()
    {
        if (ps != null)
        {
            ps.Stop();
        }
    }


    private int currentLayer = 0;
    private float lastDistance = 0;



    private float currentDistance;

    private void SetPositionUsingLinePoint(Vector2 pointToMatch)
    {
        if (linePosition == null) return;

        //transform.position = new Vector2(pointToMatch.x, pointToMatch.y - linePosition.localPosition.y);
        transform.position = pointToMatch;
    }

    public void SetInitialPostionAndLayer(PathCreator path, float distance)
    {
        if (path == null) return;

        currentDistance = distance;

        SetPositionUsingLinePoint(path.path.GetPointAtDistance(currentDistance));
        lastDistance = path.GetCustomPointDistanceAtPercent(currentDistance / path.path.length, -1);

        float s = Mathf.Lerp(minMaxScale.y, minMaxScale.x, lastDistance / 100);
        speed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, currentDistance / 100);
        transform.localScale = Vector3.one * s;
        Debug.Log("Set initial position: " + transform.position + " at distance: " + currentDistance + " with scale: " + s);

        int l = path.ReturnLayerForStart(currentDistance / path.path.length);
        if (l != currentLayer)
        {
            currentLayer = l;
            sortingGroup.sortingOrder = currentLayer;
            transform.SetParent(pathManager.RetrunPlayerParentByLayer(currentLayer));

        }
        Debug.Log("Set initial position and layer: " + currentLayer + " at distance: " + currentDistance);

    }
    private int currentPathIndex;
    private int futurePathIndex;
    public void DoPathToPoint(PathCreator path, float distance, List<PathCreator> addedPaths, List<float> addedDistancesToTravel, List<int> addedPathIndices, List<int> addedPathRoots, Transform finalPathTarget)
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        followPathCoroutine = StartCoroutine(DoPath(path, distance, addedPaths, addedDistancesToTravel, addedPathIndices, addedPathRoots, finalPathTarget));
        hitZoom = false;



    }

    [SerializeField] private float zoomSpeed;
    // void LateUpdate()
    // {
    //     if (!hitZoom)
    //         Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);

    // }

    private Sequence zoomSequence;
    private bool recheckPathDistance = false;

    public void SetRecheckPathDistance(bool r)
    {
        recheckPathDistance = r;
    }
    private float targetZoom;
    private bool hitZoom = true;
    private bool isRunning = false;

    private IEnumerator DoPath(PathCreator path, float distanceToTravel, List<PathCreator> addedPaths, List<float> addedDistancesToTravel, List<int> addedPathIndices, List<int> addedPathRoots, Transform finalPathTarget)
    {
        if (path == null) yield break;

        bool addtionalPaths = addedPaths != null;

        anim.SetBool("Run", true);
        isRunning = true;

        Debug.LogError("Doing path on path: " + path.name + " to distance: " + distanceToTravel / path.path.length);

        pathData = path.GetComponent<LevelPickerPathData>();
        // transform.position = path.path.GetPointAtDistance(0);

        lastDistance = path.GetCustomPointDistanceAtPercent(currentDistance / path.path.length);
        // ZoomCamera(path.GetCustomPointDistanceAtTime(distanceToTravel / path.path.length));
        float s = Mathf.Lerp(minMaxScale.y, minMaxScale.x, lastDistance / 100);
        speed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, lastDistance / 100);
        transform.localScale = Vector3.one * s;

        int direction = currentDistance <= distanceToTravel ? 1 : -1;

        while ((direction == 1 && currentDistance < distanceToTravel) ||
               (direction == -1 && currentDistance > distanceToTravel))
        {

            if (!addtionalPaths && recheckPathDistance)
            {
                distanceToTravel = path.path.GetClosestDistanceAlongPath(finalPathTarget.position);
            }

            float remainingDist = Mathf.Abs(distanceToTravel - currentDistance);
            float t = Mathf.Clamp01(remainingDist / 1.3f); // <= Easing radius 
            float easeFactor = 1;

            if (addedPaths != null) easeFactor = Mathf.SmoothStep(0.8f, 1f, t);
            else easeFactor = Mathf.SmoothStep(0.2f, 1f, t);

            if (currentFootstepTime <= 0)
            {
                currentFootstepTime = Mathf.Lerp(minMaxFootstepTime.x, minMaxFootstepTime.y, t);
            }
            currentFootstepTime -= Time.deltaTime;


            currentDistance += Time.deltaTime * speed * direction * easeFactor;


            // Clamp in case we overshoot
            if ((direction == 1 && currentDistance > distanceToTravel) ||
                (direction == -1 && currentDistance < distanceToTravel))
            {
                currentDistance = distanceToTravel;
            }

            SetPositionUsingLinePoint(path.path.GetPointAtDistance(currentDistance));

            float distSample = path.GetCustomPointDistanceAtPercent(currentDistance / path.path.length, direction);
            float scale = Mathf.Lerp(minMaxScale.y, minMaxScale.x, distSample / 100);
            targetZoom = Mathf.Lerp(minMaxZoom.x, minMaxZoom.y, (distSample / 100));
            float distModifier = distanceSpeedModifier * Mathf.Abs(distSample - lastDistance);

            float rawSpeed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, distSample / 100) - distModifier;
            if (rawSpeed < 1) rawSpeed = 1;
            speed = rawSpeed * baseSpeed;

            transform.localScale = Vector3.one * scale;
            lastDistance = distSample;
            if (currentFootstepTime <= 0)
            {
                AudioManager.instance.PlayFootStepSound(Mathf.Lerp(minMaxFootstepVolumeMultiplier.x, minMaxFootstepVolumeMultiplier.y, distSample / 100));
            }

            int l = path.ReturnLayer();
            if (l != currentLayer)
            {
                if (currentLayer < l) ps.Clear();
                currentLayer = l;
                sortingGroup.sortingOrder = currentLayer;
                transform.SetParent(pathManager.RetrunPlayerParentByLayer(currentLayer));
            }

            yield return null;
        }

        if (addedPaths != null)
        {
            PathCreator nextPath = addedPaths[0];
            float distanceToTravel2 = addedDistancesToTravel[0];
            int nextIndex = addedPathIndices[0];
            int nextRoot = addedPathRoots[0];
            currentDistance = nextPath.path.GetClosestDistanceAlongPath(transform.position);
            Debug.LogError("Switching to next path: " + nextPath.name + " to distance: " + distanceToTravel2 + " from position: " + transform.position + " at distance: " + currentDistance);
            if (currentDistance < 5)
                currentDistance = 0;
            else if (currentDistance > nextPath.path.length - 5)
                currentDistance = nextPath.path.length;
            nextPath.ReturnLayerForStart(currentDistance / nextPath.path.length);
            pathManager.SetCurrentPathIndexAndRoot(nextIndex, nextRoot);
            addedPaths.RemoveAt(0);
            if (addedPaths.Count <= 0) addedPaths = null;

            addedDistancesToTravel.RemoveAt(0);
            if (addedDistancesToTravel.Count <= 0) addedDistancesToTravel = null;
            addedPathIndices.RemoveAt(0);
            if (addedPathIndices.Count <= 0) addedPathIndices = null;
            addedPathRoots.RemoveAt(0);
            if (addedPathRoots.Count <= 0) addedPathRoots = null;
            followPathCoroutine = StartCoroutine(DoPath(nextPath, distanceToTravel2, addedPaths, addedDistancesToTravel, addedPathIndices, addedPathRoots, finalPathTarget));

        }
        else
        {
            anim.SetBool("Run", false);
            isRunning = false;

            HideParticles();
        }


    }
}