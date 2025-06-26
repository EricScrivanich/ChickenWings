using UnityEngine;
using PathCreation;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerLevelPickerPathFollwer : MonoBehaviour
{

    [SerializeField] private PathCreator[] paths;

    [SerializeField] private float speed;

    public bool doPath;

    [SerializeField] private Vector2 minMaxZ;
    [SerializeField] private Vector2 minMaxScale;
    private SortingGroup sortingGroup;

    private bool inForeGround = true;
    [SerializeField] private float foregroundZ;
    [SerializeField] private int pathIndex;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        sortingGroup = GetComponent<SortingGroup>();


    }

    private void OnValidate()
    {
        if (doPath)
        {
            doPath = false;
            StartCoroutine(FollowPath(pathIndex));



        }
    }

    private IEnumerator FollowPath(int i)
    {
        if (paths[i] != null)
        {
            float distanceTravelled = 0;
            transform.position = paths[i].path.GetPointAtDistance(0);
            while (distanceTravelled < paths[i].path.length)
            {
                distanceTravelled += Time.deltaTime * speed; // Adjust speed as needed
                Vector3 point = paths[i].path.GetPointAtDistance(distanceTravelled);
                transform.position = (Vector2)point;
                float zPercent = Mathf.InverseLerp(minMaxZ.x, minMaxZ.y, point.z);
                float scale = Mathf.Lerp(minMaxScale.x, minMaxScale.y, zPercent);
                transform.localScale = Vector3.one * scale;

                if (inForeGround && point.z < foregroundZ)
                {
                    inForeGround = false;
                    sortingGroup.sortingOrder = -1;
                }
                else if (!inForeGround && point.z >= foregroundZ)
                {
                    inForeGround = true;
                    sortingGroup.sortingOrder = 1;
                }

                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
