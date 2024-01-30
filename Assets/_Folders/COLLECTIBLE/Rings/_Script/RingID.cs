
using UnityEngine;
using System.Collections.Generic;

public class RingID : ScriptableObject

{
    [SerializeField] private RingPool Pool;
    public Color CenterColor;
    public ParticleSystem ringParticles;
    [SerializeField] private int currentTriggerEdited;
    private int lastCurrentTrigger;
    [SerializeField] private bool showAllPlaceholders;
    private bool lastShowAllPlaceholders;
    public int IDIndex;

    private List<GameObject> activeRings;

    public bool Testing;
    
    public int CorrectRing;
    public Material defaultMaterial;
    public Material highlightedMaterial;
    public Material passedMaterial;
    public float ballMaterialSpeed;
    public RingEvents ringEvent;
    public int triggeredRingOrder = 0;

    public int placeholderCount;

  

    private void OnEnable() {
        activeRings = new List<GameObject>();
        
    }
    public void GetBucket(Transform setTransform, int bucketOrder, float setSpeed)
    {
        BucketScript bucketScript = Pool.GetBucket(this);

        bucketScript.transform.position = setTransform.position;
        bucketScript.transform.rotation = setTransform.rotation;
        // bucketScript.transform.localScale = setTransform.localScale;
        bucketScript.order = bucketOrder;
        bucketScript.speed = setSpeed;
        Debug.Log("Getting Bucket from ID");


        // bucketScript.gameObject.SetActive(true);
 
        bucketScript.gameObject.SetActive(true);
    }

    public void GetRing(Transform setTransform, int ringOrder, float setSpeed, int doesTriggerInt, float xCordinateTrigger)
    {
        RingMovement ringScript = Pool.GetRing(this);

        ringScript.transform.position = setTransform.position;
        ringScript.transform.rotation = setTransform.rotation;
        ringScript.transform.localScale = setTransform.localScale;
        ringScript.order = ringOrder;
        ringScript.speed = setSpeed;
        ringScript.doesTriggerInt = doesTriggerInt;
        ringScript.xCordinateTrigger = xCordinateTrigger;
        


        // ringScript.gameObject.SetActive(true);

        ringScript.gameObject.SetActive(true);
        activeRings.Add(ringScript.gameObject);

    }

    public void DisableRings()
    {
        foreach(GameObject ring in activeRings)
        {
            if (ring != null)
            {
                ring.SetActive(false);
            }
            else
            {
                return;
            }
        }
        activeRings.Clear();
    }
}
