
using UnityEngine;

[CreateAssetMenu]
public class RingID : ScriptableObject

{
    [SerializeField] private int currentTriggerEdited;
    private int lastCurrentTrigger;
    [SerializeField] private bool showAllPlaceholders;
    private bool lastShowAllPlaceholders;

    // public int CurrentTriggerEdited
    // {
    //     get { return _currentTriggerEdited; } // Use the backing field
    //     set
    //     {

    //         Debug.Log($"CurrentTriggerEdited changed to {value}");
    //         _currentTriggerEdited = value; // Update the backing field
    //         ringEvent.OnCurrentTriggerEditedChanged?.Invoke(_currentTriggerEdited);

    //     }
    // }


    public bool Testing;
    public ParticleSystem ringParticles;
    public int CorrectRing;
    public Material defaultMaterial;
    public Material highlightedMaterial;
    public Material passedMaterial;
    public float ballMaterialSpeed;
    public RingEvents ringEvent;
    public int triggeredRingOrder = 0;

    public int placeholderCount;

    public GameObject BucketPrefab;

    private BucketScript bucketScript;


    public void InstantiateBucket()
    {
        GameObject bucket = Instantiate(BucketPrefab);

        bucketScript = bucket.GetComponent<BucketScript>();
        bucketScript.gameObject.SetActive(false);
        Debug.Log("Created Bucket");

    }
    private void OnEnable() {
        if (showAllPlaceholders)
        {
            ShowAllPlaceholders();
            lastCurrentTrigger = currentTriggerEdited;
        }
        else
        {
            ShowCorrectPlaceholders();

        }
        
        
    }
    private void OnValidate()
    {
        if (lastCurrentTrigger != currentTriggerEdited)
        {
            ShowCorrectPlaceholders();
            
        }

        else if (showAllPlaceholders != lastShowAllPlaceholders)
        {
            if (showAllPlaceholders)
            {
                ShowAllPlaceholders();

            }

            else
            {
                ShowCorrectPlaceholders();
            }
            

        }
      
    }
    private void ShowAllPlaceholders()
    {
        foreach (PlaceholderRing placeholder in FindObjectsOfType<PlaceholderRing>())
        {
            placeholder.gameObject.layer = LayerMask.NameToLayer("PlayerEnemy");
        }
        lastShowAllPlaceholders = showAllPlaceholders;

    }

    private void ShowCorrectPlaceholders()
    {
        foreach (PlaceholderRing placeholder in FindObjectsOfType<PlaceholderRing>())
        {
            // Check and update the layer as needed
            if (placeholder.getsTriggeredInt == currentTriggerEdited)
            {
                placeholder.gameObject.layer = LayerMask.NameToLayer("PlayerEnemy");
            }
            else
            {
                placeholder.gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }
        lastCurrentTrigger = currentTriggerEdited;
        showAllPlaceholders = false;
        lastShowAllPlaceholders = showAllPlaceholders;

    }

    public BucketScript GetBucket(Transform setTransform, int bucketOrder, float setSpeed)
    {
        if (bucketScript.gameObject.activeInHierarchy)
        {
            bucketScript.gameObject.SetActive(false);
        }

        bucketScript.transform.position = setTransform.position;
        bucketScript.transform.rotation = setTransform.rotation;
        // bucketScript.transform.localScale = setTransform.localScale;
        bucketScript.order = bucketOrder;
        bucketScript.speed = setSpeed;
        Debug.Log("Getting Bucket from ID");


        // bucketScript.gameObject.SetActive(true);

        return bucketScript;

    }



    // public Action<int> OnPassRing;

}
