using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu]
public class RingPool : ScriptableObject
{
    public bool Testing;
    [SerializeField] private int currentTriggerEdited;
    private int lastCurrentTrigger;
    [SerializeField] private bool showAllPlaceholders;
    private bool lastShowAllPlaceholders;
    // Start is called before the first frame update
    public GameObject ringPrefab;
    public GameObject bucketPrefab;
    public GameObject ballPrefab;
 
    private int ringAmount = 25;
    private int bucketAmount = 4;

    public bool isTutorial;





    // private Queue<GameObject> ringPool;
    private List<BucketScript> bucketPool;
    private List<RingMovement> ringPool;
    private List<BallMaterialMovement> ballPool;

    private Transform parent;

    public List<RingID> RingType;

    public void SpawnRingPool()
    {
        ringPool = new List<RingMovement>();
        

        if (!parent)
        {
            parent = new GameObject(name).transform;
        }

        while (ringPool.Count < ringAmount)
        {
            // GameObject obj = Instantiate(prefab, parent);
            GameObject obj = Instantiate(ringPrefab, parent);
            obj.gameObject.SetActive(false);
            RingMovement ringScript = obj.GetComponent<RingMovement>();
            ringPool.Add(ringScript);
        }
    }
    public void SpawnBallPool()
    {
        ballPool = new List<BallMaterialMovement>();
        if (!parent)
        {
            parent = new GameObject(name).transform;
        }

        while (ballPool.Count < 3)
        {
            // GameObject obj = Instantiate(prefab, parent);
            GameObject obj = Instantiate(ballPrefab, parent);
            obj.gameObject.SetActive(false);
            BallMaterialMovement ballScript = obj.GetComponent<BallMaterialMovement>();
            ballPool.Add(ballScript);
        }
    }

    public BallMaterialMovement GetBall(RingID ID)
    {
        if (ballPool == null || ballPool.Count == 0)
        {
            SpawnBallPool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }
        foreach (BallMaterialMovement ballScript in ballPool)
        {
            if (!ballScript.gameObject.activeInHierarchy)
            {
                ballScript.ID = ID;
                return ballScript;
            }
        }
        return null;
    }


    public RingMovement GetRing(RingID ID)
    {
        if (ringPool == null || ringPool.Count == 0)
        {
            SpawnRingPool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }
        foreach (RingMovement ringScript in ringPool)
        {
            if (!ringScript.gameObject.activeInHierarchy)
            {
                ringScript.ID = ID;
                return ringScript;
            }
        }
        return null;
    }

    public void DisableRings(int index)
    {
        foreach (RingMovement ringScript in ringPool)
        {
            if (ringScript.index == index)
            {
                ringScript.gameObject.SetActive(false);

            }

        }
    }

    public void SpawnBucketPool()
    {
        Debug.Log("BucketSPawning");
        bucketPool = new List<BucketScript>();
        // if (bucketPool == null || bucketPool.Count == 0)
        // {
        //     bucketPool = new List<BucketScript>();
        // }
        // if (bucketPool.Count >= bucketAmount)
        // {
        //     return;
        // }
        if (!parent)
        {
            parent = new GameObject(name).transform;
        }
        while (bucketPool.Count < bucketAmount)
        {
            // GameObject obj = Instantiate(prefab, parent);
            GameObject obj = Instantiate(bucketPrefab, parent);
            obj.gameObject.SetActive(false);
            BucketScript bucketScript = obj.GetComponent<BucketScript>();
            bucketPool.Add(bucketScript);
        }
    }

    public BucketScript GetBucket(RingID ID)
    {
        if (bucketPool == null || bucketPool.Count == 0)
        {
            SpawnBucketPool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }
        foreach (BucketScript bucketScript in bucketPool)
        {
            if (!bucketScript.gameObject.activeInHierarchy)
            {
                bucketScript.ID = ID;
                return bucketScript;
            }
        }
        return null;
    }


#region Fading/Disabling
    public IEnumerator FadeOutRed()
    {
        yield return new WaitForSeconds(.5f);
        float time = 0;
        while (time < 2f)
        {
            time += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(0, 1, time / 2f);
            RingType[0].defaultMaterial.SetFloat("_FadeAmount", fadeAmount);
            RingType[0].highlightedMaterial.SetFloat("_FadeAmount", fadeAmount);
            RingType[0].passedMaterial.SetFloat("_FadeAmount", fadeAmount);

            yield return null;
        }
        DisableRings(0);

        ResetRedMaterial();
    }

    public IEnumerator FadeOutPink()
    {
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(1, 0, time / 1f);
            RingType[1].defaultMaterial.SetFloat("_Alpha", fadeAmount);
            RingType[1].highlightedMaterial.SetFloat("_Alpha", fadeAmount);
            RingType[1].passedMaterial.SetFloat("_Alpha", fadeAmount);

            yield return null;
        }
        DisableRings(1);

        ResetPinkMaterial();
    }

    public IEnumerator FadeOutGold()
    {
        float time = 0;
        while (time < .4f)
        {
            time += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(0, .7f, time / .4f);
            RingType[2].defaultMaterial.SetFloat("_HitEffectBlend", fadeAmount);
            RingType[2].highlightedMaterial.SetFloat("_HitEffectBlend", fadeAmount);
            RingType[2].passedMaterial.SetFloat("_HitEffectBlend", fadeAmount);

            yield return null;
        }
        time = 0;
        while (time < .5f)
        {
            time += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(.25f, 0, time / .5f);
            RingType[2].defaultMaterial.SetFloat("_Alpha", fadeAmount);
            RingType[2].highlightedMaterial.SetFloat("_Alpha", fadeAmount);
            RingType[2].passedMaterial.SetFloat("_Alpha", fadeAmount);

            yield return null;
        }

        DisableRings(2);

        ResetGoldMaterial();
    }

    public void ResetAllMaterials()
    {
        ResetRedMaterial();
        ResetPinkMaterial();
        ResetGoldMaterial();
    }

    private void ResetRedMaterial()
    {
        RingType[0].defaultMaterial.SetFloat("_FadeAmount", 0);
        RingType[0].highlightedMaterial.SetFloat("_FadeAmount", 0);
        RingType[0].passedMaterial.SetFloat("_FadeAmount", 0);
    }
    private void ResetPinkMaterial()
    {
        RingType[1].defaultMaterial.SetFloat("_Alpha", 1);
        RingType[1].highlightedMaterial.SetFloat("_Alpha", 1);
        RingType[1].passedMaterial.SetFloat("_Alpha", 1);
    }
    private void ResetGoldMaterial()
    {
        RingType[2].defaultMaterial.SetFloat("_HitEffectBlend", 0);
        RingType[2].highlightedMaterial.SetFloat("_HitEffectBlend", 0);
        RingType[2].passedMaterial.SetFloat("_HitEffectBlend", 0);
        RingType[2].defaultMaterial.SetFloat("_Alpha", 1);
        RingType[2].highlightedMaterial.SetFloat("_Alpha", 1);
        RingType[2].passedMaterial.SetFloat("_Alpha", 1);
    }
    #endregion



    private void OnEnable()
    {

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

    #region Testing
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
                placeholder.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        lastCurrentTrigger = currentTriggerEdited;
        showAllPlaceholders = false;
        lastShowAllPlaceholders = showAllPlaceholders;

    }
    #endregion
}
