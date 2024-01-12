
using UnityEngine;

[CreateAssetMenu]
public class RingID : ScriptableObject

{
    public bool Testing;
    public ParticleSystem ringParticles;
    public int CorrectRing;
    public Material defaultMaterial;
    public Material highlightedMaterial; 
    public Material passedMaterial;
    public float ballMaterialSpeed;
    public RingEvents ringEvent;

    public int placeholderCount;

    public GameObject BucketPrefab;
    
    private BucketScript bucketScript;

    public void InstantiateBucket()
    {
        GameObject bucket = Instantiate(BucketPrefab);
        
        bucketScript = bucket.GetComponent<BucketScript>();
        bucketScript.gameObject.SetActive(false);

    }

    public BucketScript GetBucket(Transform setTransform, int bucketOrder, float setSpeed)
    {
        bucketScript.gameObject.SetActive(false);
        bucketScript.transform.position = setTransform.position;
        bucketScript.transform.rotation = setTransform.rotation;
        bucketScript.transform.localScale = setTransform.localScale;
        bucketScript.order = bucketOrder;
        bucketScript.speed = setSpeed;

        bucketScript.gameObject.SetActive(true);
        return bucketScript;

    }

   

    // public Action<int> OnPassRing;



}
