using UnityEngine;
using PathCreation;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerLevelPickerPathFollwer : MonoBehaviour
{

    [SerializeField] private PathCreator[] paths;

    private float speed;



    public bool doPath;

    [SerializeField] private float baseSpeed = 1;

    [SerializeField] private Vector2 minMaxSpeed;
    [SerializeField] private float distanceSpeedModifier;


    [SerializeField] private Vector2 minMaxScale;
    private SortingGroup sortingGroup;

    [SerializeField] private ParticleSystem ps;

    private bool inForeGround = true;
    [SerializeField] private float foregroundZ;
    [SerializeField] private int pathIndex;

    private LevelPickerPathData pathData;
    private Animator anim;
    private Coroutine followPathCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();

        transform.position = paths[pathIndex].path.GetPoint(2);


    }

    private void OnValidate()
    {
        if (doPath)
        {
            doPath = false;
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }
            followPathCoroutine = StartCoroutine(FollowPath(pathIndex));



        }
    }

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


    private IEnumerator FollowPath(int i)
    {
        if (paths[i] != null)
        {
            float distanceTravelled = 0;

            float time = 0;

            anim.SetBool("Run", true);



            lastDistance = paths[i].GetCustomPointDistanceAtTime(distanceTravelled / paths[i].path.length);
            float s = Mathf.Lerp(minMaxScale.y, minMaxScale.x, lastDistance / 100);
            speed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, lastDistance / 100);
            transform.localScale = Vector3.one * s;
            pathData = paths[i].gameObject.GetComponent<LevelPickerPathData>();
            transform.position = paths[i].path.GetPointAtDistance(0);
            while (distanceTravelled < paths[i].path.length)
            {
                distanceTravelled += Time.deltaTime * speed; // Adjust speed as needed
                // Vector3 point = 
                transform.position = (Vector2)paths[i].path.GetPointAtDistance(distanceTravelled);

                float distance = paths[i].GetCustomPointDistanceAtTime(distanceTravelled / paths[i].path.length);
                // Debug.Log($"Distance: {distance} at {distanceTravelled}");

                float scale = Mathf.Lerp(minMaxScale.y, minMaxScale.x, distance / 100);
                float distanceModifier = distanceSpeedModifier * Mathf.Abs(distance - lastDistance);
                // Debug.Log($"Distance Modifier: {distanceModifier})");
                speed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, distance / 100) - distanceModifier;
                lastDistance = distance;
                if (speed < 1) speed = 1;

                speed *= baseSpeed;
                transform.localScale = Vector3.one * scale;
                int l = paths[i].ReturnLayer();
                if (l != currentLayer)
                {
                    if (currentLayer < l) ps.Clear();
                    currentLayer = l;
                    sortingGroup.sortingOrder = currentLayer;
                }


                // if (inForeGround && point.z < foregroundZ)
                // {
                //     inForeGround = false;
                //     sortingGroup.sortingOrder = -1;
                // }
                // else if (!inForeGround && point.z >= foregroundZ)
                // {
                //     inForeGround = true;
                //     sortingGroup.sortingOrder = 1;
                // }

                yield return null;
            }
            anim.SetBool("Run", false);
            HideParticles();


            // while (time < pathData.totalTime)
            // {
            //     time += Time.unscaledDeltaTime;
            //     Vector2 data = pathData.ReturnScaleAndDistance(time / pathData.totalTime);
            //     Vector3 point = paths[i].path.GetPointAtTime(data.y);
            //     transform.position = (Vector2)point;
            //     // float zPercent = Mathf.InverseLerp(minMaxZ.x, minMaxZ.y, point.z);
            //     // float scale = Mathf.Lerp(minMaxScale.x, minMaxScale.y, zPercent);
            //     transform.localScale = Vector3.one * data.x;

            //     // if (inForeGround && point.z < foregroundZ)
            //     // {
            //     //     inForeGround = false;
            //     //     sortingGroup.sortingOrder = -1;
            //     // }
            //     // else if (!inForeGround && point.z >= foregroundZ)
            //     // {
            //     //     inForeGround = true;
            //     //     sortingGroup.sortingOrder = 1;
            //     // }

            //     yield return null;
            // }

            // Vector2 d = pathData.ReturnScaleAndDistance(1);
            // Vector3 p = paths[i].path.GetPointAtTime(d.y);
            // transform.position = (Vector2)p;
            // // float zPercent = Mathf.InverseLerp(minMaxZ.x, minMaxZ.y, point.z);
            // // float scale = Mathf.Lerp(minMaxScale.x, minMaxScale.y, zPercent);
            // transform.localScale = Vector3.one * d.x;
        }
    }
    private float currentDistance;

    public void DoPathToPoint(PathCreator path, float distance)
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        followPathCoroutine = StartCoroutine(DoPath(path, distance));


    }
    private IEnumerator DoPath(PathCreator path, float distanceToTravel)
    {
        if (path == null) yield break;

        anim.SetBool("Run", true);

        pathData = path.GetComponent<LevelPickerPathData>();
        transform.position = path.path.GetPointAtDistance(0);

        lastDistance = path.GetCustomPointDistanceAtTime(currentDistance / path.path.length);
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


            currentDistance += Time.deltaTime * speed * direction * easeFactor;

            // Clamp in case we overshoot
            if ((direction == 1 && currentDistance > distanceToTravel) ||
                (direction == -1 && currentDistance < distanceToTravel))
            {
                currentDistance = distanceToTravel;
            }

            transform.position = (Vector2)path.path.GetPointAtDistance(currentDistance);

            float distSample = path.GetCustomPointDistanceAtTime(currentDistance / path.path.length, direction);
            float scale = Mathf.Lerp(minMaxScale.y, minMaxScale.x, distSample / 100);
            float distModifier = distanceSpeedModifier * Mathf.Abs(distSample - lastDistance);

            float rawSpeed = Mathf.Lerp(minMaxSpeed.y, minMaxSpeed.x, distSample / 100) - distModifier;
            if (rawSpeed < 1) rawSpeed = 1;
            speed = rawSpeed * baseSpeed;

            transform.localScale = Vector3.one * scale;
            lastDistance = distSample;

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