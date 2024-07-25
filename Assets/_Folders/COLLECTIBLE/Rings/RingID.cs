
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class RingID : ScriptableObject

{
    [SerializeField] private RingPool Pool;
    public bool ReadyToSpawn;

    public Color CenterColor;
    public GameObject ringParticles;
    public int nextIndex = 0;
    private int particlesInUse;
    public int IDIndex;

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
    public Queue<ParticleFollowScript> particleSystemsQueue;
    private GameObject currentBucket;
    private List<GameObject> objectsToDeactivate;
    private Transform parent;
    public bool newSetup;
    public int placeholderCount;

    public int CorrectRing;

    private int ringOrder;



    private const int poolSize = 5;
    public void ResetVariables()
    {

        CorrectRing = 1;
        ringOrder = 1;
        currentBucket = null;
        objectsToDeactivate.Clear();
        nextIndex = 0;
        triggeredRingOrder = 0;

        ReturnAllParticles();
        ringList.Clear();
        particlesInUse = 0;
        ReadyToSpawn = true;

    }

    public void InitializeVariables()
    {
        CorrectRing = 1;
        ringOrder = 1;
    }

    public void GetEffect(RingMovement ring)
    {
        // if (ring == null)
        // {
        //     return;
        // }

        Debug.Log("Getting Effect for: " + this);


        ParticleFollowScript script = particleSystemsQueue.Dequeue();
        script.ringTransform = ring;



        script.gameObject.SetActive(true);



        particleSystemsQueue.Enqueue(script);

        // particlesInUse++;

        nextIndex++;
        // grabbedPS.SetActive(true);


    }

    public void ReturnAllParticles()
    {
        if (particlesInUse > 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var effect = particleSystemsQueue.Dequeue();
                effect.gameObject.SetActive(false);
                particleSystemsQueue.Enqueue(effect);

            }

        }

    }

    public void BallParticles(Vector2 postion)
    {
        ballPS.transform.position = postion;
        ballPS.Play();
    }

    public void InitializeEffectsPool()
    {
        if (!parent)
        {
            parent = new GameObject(name).transform;
        }
        if (ballParticlePrefab != null)
        {
            ballParticle = Instantiate(ballParticlePrefab, parent);
            ballPS = ballParticle.GetComponent<ParticleSystem>();

        }
        particlesInUse = 0;

        ringList = new List<RingMovement>();
        particleSystemsQueue = new Queue<ParticleFollowScript>();
        objectsToDeactivate = new List<GameObject>();


        if (ringParticles != null)
        {


            for (int i = 0; i < poolSize; i++)
            {
                GameObject effectInstance = Instantiate(ringParticles, parent); // Instantiate from the prefab
                var script = effectInstance.GetComponent<ParticleFollowScript>();

                effectInstance.SetActive(false); // Start with the GameObject disabled
                particleSystemsQueue.Enqueue(script);
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

    public void GetSonicWave(Vector2 position)
    {
        Pool.GetSonicWave(position);

    }

    public void GetRing(Vector2 setPosition, Quaternion setRotation, Vector2 setScale, float setSpeed)
    {
        ReadyToSpawn = false;
        RingMovement ringScript = Pool.GetRing(this);

        ringScript.transform.position = setPosition;
        ringScript.transform.rotation = setRotation;
        ringScript.transform.localScale = setScale;
        ringScript.order = ringOrder;
        ringScript.speed = setSpeed;


        ringScript.gameObject.SetActive(true);


        ringList.Add(ringScript);
        ringOrder++;
        if (particlesInUse < poolSize)
        {
            Debug.Log("YES PARTILCE");
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


    public void GetBucket(Vector2 setPosition, Quaternion setRotation, Vector2 setScale, float setSpeed)
    {
        BucketScript bucketScript = Pool.GetBucket(this);


        bucketScript.transform.position = setPosition;
        bucketScript.transform.rotation = setRotation;
        bucketScript.transform.localScale = setScale;
        bucketScript.order = ringOrder;
        bucketScript.speed = setSpeed;
        currentBucket = bucketScript.gameObject;
        bucketScript.gameObject.SetActive(true);
        ringOrder++;

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
