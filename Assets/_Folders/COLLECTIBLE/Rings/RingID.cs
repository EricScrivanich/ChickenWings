
using UnityEngine;


[CreateAssetMenu]
public class RingID : ScriptableObject

{
    [SerializeField] private RingPool Pool;
    public Color CenterColor;
    public ParticleSystem ringParticles; 
    public int IDIndex;
    private int correctRing;
    
    public Material defaultMaterial;
    public Material highlightedMaterial;
    public Material passedMaterial;
    public float ballMaterialSpeed;
    public RingEvents ringEvent;
    public int triggeredRingOrder = 0;

    public int placeholderCount;

    public int CorrectRing
    {
        get
        {
            return correctRing;
        }
        set
        {
            correctRing = value;

            if (correctRing > 1)
            {
                ringEvent.OnPassedCorrectRing?.Invoke();
            }
        }
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
       

    }

    public void GetBall(Vector2 startPosition, GameObject obj = null, Vector2? targetPos = null)
    {
        BallMaterialMovement ballScript = Pool.GetBall(this);
        ballScript.transform.position = startPosition;

        if (targetPos.HasValue)
        {
            ballScript.targetPosition = targetPos.Value;
        }
        else
        {
            Debug.Log("Grabbiung");
            ballScript.targetObject = obj;
            Debug.Log("Got");

        }
        ballScript.gameObject.SetActive(true);
    }

    
}
