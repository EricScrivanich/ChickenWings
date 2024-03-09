
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class RingID : ScriptableObject

{
    [SerializeField] private RingPool Pool;

    public Color CenterColor;
    public GameObject ringParticles;
    public int nextIndex = 0;
    private int particlesInUse;
    public int IDIndex;
    private bool ReadyToSpawn;
    public GameObject ballParticlePrefab;
    private GameObject ballParticle;
    private ParticleSystem ballPS;
    public float ballExplosionThreshold;


    public Material defaultMaterial;
    public Material highlightedMaterial;
    public Material passedMaterial;
    public float ballMaterialSpeed;
    public RingEvents ringEvent;
    public int triggeredRingOrder = 0;
    private List<RingMovement> ringList;
    public Queue<GameObject> particleSystemsQueue;
    public bool newSetup;
    public int placeholderCount;

    public int CorrectRing;



    private const int poolSize = 5;
    public void ResetVariables()
    {

        CorrectRing = 1;
        nextIndex = 0;
        triggeredRingOrder = 0;
        ReadyToSpawn = true;
        ringList.Clear();
        particlesInUse = 0;

        // if (!initialReset)
        // {
        //     Debug.Log("GOing buhhh: " + IDIndex);
        //     int initialCount = particleSystemsQueue.Count; // Store the initial count

        //     for (int i = 0; i < initialCount; i++)
        //     {
        //         GameObject ps = particleSystemsQueue.Dequeue(); // Remove the item from the front of the queue
        //         ps.SetActive(false); // Perform the reset action
        //         particleSystemsQueue.Enqueue(ps); // Place it back at the end of the queue
        //     }
        // }

    }
    public void GetEffect(RingMovement ring)
    {
        // if (ring == null)
        // {
        //     return;
        // }
        GameObject grabbedPS = particleSystemsQueue.Dequeue();

        ParticleFollowScript script = grabbedPS.GetComponent<ParticleFollowScript>();
        script.ringTransform = ring;

       
        
            grabbedPS.SetActive(true);

        

        particleSystemsQueue.Enqueue(grabbedPS);

        // particlesInUse++;

        nextIndex++;
        // grabbedPS.SetActive(true);


    }

    public void BallParticles(Vector2 postion)
    {
        ballPS.transform.position = postion;
        ballPS.Play();
    }

    public void InitializeEffectsPool()
    {
        if (ballParticlePrefab != null)
        {
            ballParticle = Instantiate(ballParticlePrefab);
            ballPS = ballParticle.GetComponent<ParticleSystem>();
        }

        ringList = new List<RingMovement>();
        particleSystemsQueue = new Queue<GameObject>();
        if (ringParticles != null)
        { 


            for (int i = 0; i < poolSize; i++)
            {
                GameObject effectInstance = Instantiate(ringParticles); // Instantiate from the prefab

                effectInstance.SetActive(false); // Start with the GameObject disabled
                particleSystemsQueue.Enqueue(effectInstance);
            }
        }

    }
    public RingMovement GetNewRing()
    {
        if (nextIndex >= ringList.Count)
        {
            return null;
        }
        else
        {
            return ringList[nextIndex];
        }
  
    }

    public void GetRing(Vector2 setPosition, Quaternion setRotation, Vector2 setScale, int ringOrder, float setSpeed, int doesTriggerInt, float xCordinateTrigger)
    {
        RingMovement ringScript = Pool.GetRing(this);

        ringScript.transform.position = setPosition;
        ringScript.transform.rotation = setRotation;
        ringScript.transform.localScale = setScale;
        ringScript.order = ringOrder;
        ringScript.speed = setSpeed;
        ringScript.doesTriggerInt = doesTriggerInt;
        ringScript.xCordinateTrigger = xCordinateTrigger;

        ringScript.gameObject.SetActive(true);


        ringList.Add(ringScript);
        if (particlesInUse < poolSize)
        {
            particlesInUse++;
            
            GetEffect(ringScript);
        }
    }



    // private void AddRingToList(RingMovement ring, int order)
    // {
    //     // Ensure the list is large enough to hold the ring at its order position
    //     while (ringList.Count < order)
    //     {
    //         ringList.Add(null);
    //     }

    //     // Insert the ring at its order position in the list
    //     ringList[order - 1] = ring;
    // }


    public void ReturnEffect(ParticleFollowScript effect)
    {
        if (effect != null) // Check if the GameObject is not null
        {

            // effect.gameObject.SetActive(false);

            if (nextIndex < ringList.Count)
            {
                effect.gameObject.SetActive(false);
                // particleSystemsQueue.Enqueue(effect.gameObject);
                GetEffect(GetNewRing());
            }
            else
            {
                effect.gameObject.SetActive(false);
                particlesInUse--;
            }
            // else{
            //     effect.gameObject.SetActive(false);
            //     particleSystemsQueue.Enqueue(effect.gameObject);

            // }

            // effect.SetActive(false); // Deactivate the GameObject
            // particleSystemsQueue.Enqueue(effect.gameObject); // Return it to the queue for reuse


        }
    }

    private void MoveParticleObject()
    {

    }




    public void GetBucket(Vector2 setPosition, Quaternion setRotation, Vector2 setScale, int bucketOrder, float setSpeed)
    {
        BucketScript bucketScript = Pool.GetBucket(this);


        bucketScript.transform.position = setPosition;
        bucketScript.transform.rotation = setRotation;
        bucketScript.transform.localScale = setScale;
        bucketScript.order = bucketOrder;
        bucketScript.speed = setSpeed;
       


        // bucketScript.gameObject.SetActive(true);

        bucketScript.gameObject.SetActive(true);
    }



    public void GetBall(Vector2 startPosition, GameObject obj = null, Vector2? targetPos = null)
    {
        BallMaterialMovement ballScript = Pool.GetBall(this);
        ballScript.transform.position = startPosition;

        if (targetPos.HasValue)
        {
            ballScript.targetPosition = targetPos.Value;
            ballScript.targetObject = null;
        }
        else
        {
           
            ballScript.targetObject = obj;
            

        }
        ballScript.ID = this;
        ballScript.gameObject.SetActive(true);
    }


}
