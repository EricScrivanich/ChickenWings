using UnityEngine;
using System.Collections.Generic;

public class BackgroundObjects : MonoBehaviour
{
    public float baseSpeedMultiplier = 1f;
    [SerializeField] private int layerNumber;
    [Header("Cloud Settings")]
    [SerializeField] private Sprite[] cloudSprites;
    [SerializeField] private GameObject cloudPrefab;

    [SerializeField] private Vector2Int cloudCountRange = new Vector2Int(4, 8);
    [SerializeField] private Vector2 cloudSpeedRange = new Vector2(0.3f, 1.3f);
    [SerializeField] private Vector2 cloudScaleRange = new Vector2(0.5f, 1.5f);
    [SerializeField] private Color colorFar = Color.gray;
    [SerializeField] private Color colorClose = Color.white;
    [SerializeField] private Vector2 yChangeByDistance;

    [SerializeField] private Vector2 cloudYRange = new Vector2(-2f, 3f);
    [SerializeField] private Vector2 cloudXRange = new Vector2(-10f, 10f); // for prewarm placement

    [Header("Spawn Control")]
    [SerializeField] private Vector2 spawnDelayRange = new Vector2(1.25f, 2.5f); // seconds
    [SerializeField] private float minYSeparation = 0.6f;  // how different the next Y should be
    [SerializeField] private float rightSpawnOffset = 0.8f; // spawn just off-screen to the right
    [SerializeField] private float leftDespawnOffset = 0.8f; // spawn just off-screen to the right
    [SerializeField] private float initialEdgePadding = 0.6f; // keep away from edges
    [SerializeField] private float initialJitterX = 0.25f;    // tiny randomness per slot
    private float spawnTimer;
    private float nextSpawnDelay;

    [Header("Background Pig Settings")]
    [SerializeField] private int numberOfPigsOnScreen;
    [SerializeField] private int pigsInPool;
    [SerializeField] private GameObject[] pigPrefabs;
    [SerializeField] private float[] pigSpawnChances;
    private BackGroundObject[] pigs;


    // Pool
    private List<SpriteRenderer> pool = new List<SpriteRenderer>();
    private readonly List<Transform> activeClouds = new List<Transform>();
    private readonly List<float> cloudSpeeds = new List<float>();

    private int targetActiveCount;
    private float lastSpawnY = float.NaN;
    [SerializeField] private Material backgroundMaterial;
    private bool trackMovement = false;
    private float lastX = 0f;

    public void SetTrackingMovement(bool track)
    {
        lastX = transform.position.x;
        trackMovement = track;

    }
    private void Start()
    {
        // Build pool up to max
        int poolSize = cloudCountRange.y + 2;
        for (int i = 0; i < poolSize; i++)
        {
            var c = Instantiate(cloudPrefab, transform).GetComponent<SpriteRenderer>();
            c.sortingOrder = layerNumber;
            c.gameObject.SetActive(false);
            pool.Add(c);
        }

        // Prewarm a random amount
        targetActiveCount = Random.Range(cloudCountRange.x, cloudCountRange.y + 1);
        for (int i = 0; i < targetActiveCount; i++)
        {
            SpawnObject(true, initial: true, slotIndex: i, slotCount: targetActiveCount);
        }

        if (pigsInPool > 0)
        {
            pigs = new BackGroundObject[pigsInPool];
            for (int i = 0; i < pigsInPool; i++)
            {
                float roll = Random.value;
                float cumulative = 0f;
                GameObject toSpawn = pigPrefabs[0];
                for (int j = 0; j < pigSpawnChances.Length; j++)
                {
                    cumulative += pigSpawnChances[j];
                    if (roll <= cumulative)
                    {
                        toSpawn = pigPrefabs[j];
                        break;
                    }
                }
                var p = Instantiate(toSpawn, transform).GetComponent<BackGroundObject>();
                p.ApplyMaterialToAllSprites(backgroundMaterial, null, this);
                p.gameObject.SetActive(false);
                pigs[i] = p;
            }

            for (int i = 0; i < numberOfPigsOnScreen; i++)
            {
                SpawnObject(false, initial: true, slotIndex: i, slotCount: numberOfPigsOnScreen);
            }
        }

        // Prime spawn timer so we continue spawning after prewarm (if room)
        ResetSpawnDelay();

    }
    private float timer;

    private void Update()
    {
        if (trackMovement)
        {
            float s = (transform.position.x - lastX) / Time.deltaTime;

            if (s > 0)
            {
                baseSpeedMultiplier = s * .25f;

                if (s < 1) baseSpeedMultiplier = 1f;
            }
            else
            {
                baseSpeedMultiplier = s * .3f;

                if (s > -1) baseSpeedMultiplier = -1f;

            }
            Debug.Log("BG speed: " + s);
            // set bg objects speed, use 5 as estimate of ground speed to keep values consistent
            baseSpeedMultiplier = s;




            lastX = transform.position.x;
        }

        // Move active clouds
        for (int i = activeClouds.Count - 1; i >= 0; i--)
        {
            var t = activeClouds[i];
            float speed = cloudSpeeds[i];

            t.position += Vector3.left * speed * Time.deltaTime * baseSpeedMultiplier;

            // If gone off-screen left, recycle it

        }
        timer += Time.deltaTime;

        if (timer > .2f)
        {
            for (int i = activeClouds.Count - 1; i >= 0; i--)
            {
                var t = activeClouds[i];


                // If gone off-screen left, recycle it
                if (t.position.x < BoundariesManager.leftBoundary - leftDespawnOffset + transform.position.x)
                {
                    RecycleAt(i);
                }
            }

            if (pigsInPool > 0)
            {
                for (int i = 0; i < pigs.Length; i++)
                {
                    if (pigs[i].CheckForDespawn(BoundariesManager.leftBoundary - leftDespawnOffset + transform.position.x))
                    {
                        SpawnObject(false);
                        break;
                        // pig was despawned


                    }

                }
            }
            timer = 0f;
        }

        // Timed spawning (if we have pooled items available)
        spawnTimer += Time.deltaTime * baseSpeedMultiplier;
        if (spawnTimer >= nextSpawnDelay)
        {
            // Only spawn if we have a pooled cloud left (prevents unbounded growth)
            if (TryHasPooled())
            {
                SpawnObject(true);
            }
            ResetSpawnDelay();
        }
    }

    // --- Spawning / Recycling ---

    private void SpawnObject(bool cloud, bool initial = false, int slotIndex = 0, int slotCount = 0)
    {


        // ----- Even initial X distribution -----

        float x;

        if (initial && slotCount > 0)
        {
            float left = Mathf.Max(BoundariesManager.leftBoundary, cloudXRange.x);
            float right = Mathf.Min(BoundariesManager.rightBoundary, cloudXRange.y);
            // Place at the center of each segment, then add a small jitter
            float usableLeft = left + initialEdgePadding;
            float usableRight = right - initialEdgePadding;
            float t = (slotIndex + 0.5f) / slotCount; // centers in each segment
            x = Mathf.Lerp(usableLeft, usableRight, t) + Random.Range(-initialJitterX, initialJitterX);
        }
        else
        {
            // Standard spawn on the right for runtime spawns
            x = BoundariesManager.rightBoundary + rightSpawnOffset;
        }
        if (cloud)
        {
            var sr = GetPooled();
            if (sr == null) return;

            float distance = Random.value;

            // Look/feel
            sr.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            sr.color = Color.Lerp(colorClose, colorFar, distance);

            bool flip = Random.value > 0.45f;
            float baseScale = Mathf.Lerp(cloudScaleRange.x, cloudScaleRange.y, distance);
            float sx = baseScale * (flip ? -1f : 1f) + Random.Range(-0.2f, 0.2f) * Mathf.Sign(flip ? -1f : 1f);
            float sy = baseScale + Random.Range(-0.2f, 0.2f);
            sr.transform.localScale = new Vector3(sx, sy, 1f);

            float y = PickNextY(distance);

            sr.transform.position = new Vector2(x + transform.position.x, y);
            sr.gameObject.SetActive(true);

            float speed = Mathf.Lerp(cloudSpeedRange.x, cloudSpeedRange.y, distance);
            activeClouds.Add(sr.transform);
            cloudSpeeds.Add(speed);

            lastSpawnY = y;
        }
        else
        {
            const int maxAttempts = 6;
            for (int i = 0; i < maxAttempts; i++)
            {
                int r = Random.Range(0, pigs.Length);
                if (!pigs[r].gameObject.activeSelf)
                {
                    float distance = Random.value;
                    float y = PickNextY(distance);
                    pigs[r].Initialize(new Vector2(x + transform.position.x, y), distance);
                    pigs[r].gameObject.SetActive(true);
                    lastSpawnY = y;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

    }
    private void RecycleAt(int index)
    {
        var tf = activeClouds[index];
        var sr = tf.GetComponent<SpriteRenderer>();

        // Return to pool
        sr.gameObject.SetActive(false);

        activeClouds.RemoveAt(index);
        cloudSpeeds.RemoveAt(index);
        // (We don't shrink pool; SpawnCloud will reuse this entry)
    }

    private SpriteRenderer GetPooled()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf) return pool[i];
        }
        return null;
    }

    private bool TryHasPooled()
    {
        for (int i = 0; i < pool.Count; i++)
            if (!pool[i].gameObject.activeSelf) return true;
        return false;
    }

    // --- Helpers ---

    private float PickNextY(float distance)
    {
        // Try a few samples to get enough separation from the last Y
        const int tries = 6;
        float addedYByDistance = Mathf.Lerp(yChangeByDistance.x, yChangeByDistance.y, distance);
        float chosen = Random.Range(cloudYRange.x, cloudYRange.y) + addedYByDistance;

        if (float.IsNaN(lastSpawnY))
            return chosen;

        float best = chosen;
        float bestDist = Mathf.Abs(chosen - lastSpawnY);

        for (int i = 1; i < tries; i++)
        {
            float cand = Random.Range(cloudYRange.x, cloudYRange.y);
            float d = Mathf.Abs(cand - lastSpawnY);

            // Prefer farther from last; accept as soon as it meets minYSeparation
            if (d >= minYSeparation)
                return cand;

            if (d > bestDist)
            {
                bestDist = d;
                best = cand;
            }
        }
        return best; // best we found this frame
    }

    private void ResetSpawnDelay()
    {
        spawnTimer = 0f;
        nextSpawnDelay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
    }
}