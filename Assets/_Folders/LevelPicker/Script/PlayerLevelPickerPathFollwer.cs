using UnityEngine;
using PathCreation;
using System.Collections;
using UnityEngine.Rendering;
using DG.Tweening;

public class PlayerLevelPickerPathFollwer : MonoBehaviour
{

    [SerializeField] private PathCreator[] paths;

    private float speed;



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
    private Animator anim;
    private Coroutine followPathCoroutine;



    void Awake()
    {
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        // transform.position = paths[pathIndex].path.GetPoint(2);


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

    public void ShowParticles()
    {
        if (ps != null)
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

    public void SetInitialPostionAndLayer(PathCreator path, float distance)
    {
        if (path == null) return;

        currentDistance = distance;

        transform.position = (Vector2)path.path.GetPointAtDistance(currentDistance);
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

        }
        Debug.Log("Set initial position and layer: " + currentLayer + " at distance: " + currentDistance);

    }

    public void DoPathToPoint(PathCreator path, float distance)
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        followPathCoroutine = StartCoroutine(DoPath(path, distance));
        hitZoom = false;



    }
    [SerializeField] private float zoomSpeed;
    // void LateUpdate()
    // {
    //     if (!hitZoom)
    //         Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);

    // }

    private Sequence zoomSequence;
    private void ZoomCamera(float distancePercent, Vector2 pos, float time)
    {
        if (zoomSequence != null && zoomSequence.IsActive() && zoomSequence.IsPlaying())
        {
            zoomSequence.Kill();
        }

        zoomSequence = DOTween.Sequence();
        targetZoom = Mathf.Lerp(minMaxZoom.x, minMaxZoom.y, distancePercent * .01f);
        zoomSequence.Append(Camera.main.DOOrthoSize(targetZoom, 0.5f))
                    .OnComplete(() => Debug.Log("Camera zoomed to: " + targetZoom + " at distance percent: " + distancePercent))
                    .SetUpdate(true);


    }
    private float targetZoom;
    private bool hitZoom = true;

    private IEnumerator DoPath(PathCreator path, float distanceToTravel)
    {
        if (path == null) yield break;

        anim.SetBool("Run", true);

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



            float remainingDist = Mathf.Abs(distanceToTravel - currentDistance);
            float t = Mathf.Clamp01(remainingDist / 1.3f); // <= Easing radius (5 units)
            float easeFactor = Mathf.SmoothStep(0.2f, 1f, t); // Slows down near target

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

            transform.position = (Vector2)path.path.GetPointAtDistance(currentDistance);

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
            }

            yield return null;
        }

        anim.SetBool("Run", false);

        HideParticles();
    }
}