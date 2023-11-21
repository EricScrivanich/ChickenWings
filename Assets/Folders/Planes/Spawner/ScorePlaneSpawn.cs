using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePlaneSpawn : MonoBehaviour
{
    public GameObject childPrefab;
    private List<GameObject> newChildren;

    private int planeAmount;

    private GameObject child;
    private GameObject newChild;

    private float minY;
    private float maxY;
    private float yPos;

    private StatsManager statsMan; // reference to the ScoreManager
   
    [SerializeField] private int[] spawnScores;

    private Transform topBoundryTransform;
    private Transform bottomBoundryTransform;

    void Awake()
    {
        planeAmount = 0;
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        statsMan.OnScoreChanged += CheckScore; // Subscribe to the OnScoreChanged event
    }

    void Start()
    {
        topBoundryTransform = GetComponentInChildren<Transform>().Find("topBoundry");
        bottomBoundryTransform = GetComponentInChildren<Transform>().Find("bottomBoundry");
        maxY = topBoundryTransform.position.y;
        minY = bottomBoundryTransform.position.y;

        yPos = Random.Range(minY, maxY);
        newChildren = new List<GameObject>();
    }

    void CheckScore(int newScore) // This method will be called every time the score changes
    {
        // Check if we've reached the score for the next plane
        if (planeAmount < spawnScores.Length && newScore >= spawnScores[planeAmount])
        {
            // Generate a new y position for the child object
            yPos = Random.Range(minY, maxY);

            // Instantiate the new child object
            GameObject newChild = Instantiate(childPrefab, new Vector3(BoundariesManager.rightBoundary, yPos, transform.position.z), Quaternion.identity);

            // Add the new child to the list of new children
            newChildren.Add(newChild);
            planeAmount++;
        }
    }

    private void OnDestroy() // When this object is destroyed
    {
        if (statsMan != null)
        {
            statsMan.OnScoreChanged -= CheckScore; // Unsubscribe from the OnScoreChanged event to avoid null reference errors
        }
    }
}

