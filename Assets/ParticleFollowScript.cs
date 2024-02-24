using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowScript : MonoBehaviour
{
    public RingMovement ringTransform;
    public RingID ID;
    private bool following;
    private ParticleSystem ps;
    public bool canPlay;
    private bool hasPlayed;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ringTransform!=null)
        {
             if (ringTransform.correctCollision && canPlay)
        {
            ps.Play();
            canPlay = false;
            hasPlayed = true;
        }
        // Debug.Log(ps.isStopped);

        if ((ps.isStopped && hasPlayed))
        {
            ringTransform.correctCollision = false;
            canPlay = true;
            hasPlayed = false;
            Debug.Log("retruen");
            ID.ReturnEffect(this);
            // ResetRing();
            // ID.particleSystemsQueue.Enqueue(this.gameObject);
            // // gameObject.SetActive(false);
            // ID.GetEffect(ID.ringList[ID.nextIndex]);

            // Return this gameobject to pool to then be attachted to the next ring based on its order in list 
        }
        gameObject.transform.position = ringTransform.transform.position;

        }
       

       




    }

   

    // private void ResetRing()
    // {
    //     if (ID.ringList != null && ID.ringList.Count > 0)
    //     {
    //         ringTransform = ID.GetNewRing(); // Ensure GetNewRing() handles null safely
    //     }
    //     else
    //     {
    //         Debug.LogWarning("ringList is null or empty.");
    //     }
    // }

    private void OnDisable()
    {
        Debug.Log("DIsssssss");

    }
    private void OnEnable()
    {
        // ID.ringEvent.ResetQuueue += 
        following = true;
        canPlay = true;
        hasPlayed = false;
        if (ringTransform != null)
        {
            gameObject.transform.rotation = ringTransform.transform.rotation;
        }
    }
}