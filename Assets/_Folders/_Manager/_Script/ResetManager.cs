
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance;
    [SerializeField] private ShaderVariantCollection shaderVariantCollection;

    public LevelManagerID lvlID;
    private bool loadedAssets;

    private GameObject blockButtons;
    [SerializeField] private PreloadAssetsContainer preLoadAssets;
    private InputController controls;
    [SerializeField] private float waitTime; // Time after death before the player may reset
    private float resetTimer; // Timer to track the wait time
                              // private bool canReset;
                              // private bool resetTriggered; // Variable to prevent multiple resets

    public static event Action onRestartGame;
    private InputAction resetAction;
    public int checkPoint;

    public static Action GameOverEvent;


    void Awake()
    {
        loadedAssets = false;

        controls = new InputController();

        // Bind the Reset action to a method
        // controls.Special.ResetGame.performed += ctx => ResetGame();
        // controls.Special.Disable();
        checkPoint = 0;

    }
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu")
        {
            // if (shaderVariantCollection != null)
            // {
            //     Debug.Log("Prewarming Shaders...");
            //     shaderVariantCollection.WarmUp();
            //     Debug.Log("Shaders prewarmed.");
            // }
            // else
            // {
            //     Debug.LogError("Shader Variant Collection not assigned.");
            // }


            // canReset = false;
            // resetTriggered = false;
            // if (SceneManager.GetActiveScene().name == "MainMenu")
            StartCoroutine(PreloadAssetsCoroutine());
        }

        // CollectShaderVariants();







    }

    private void CollectShaderVariants()
    {
        if (preLoadAssets == null || preLoadAssets.materials == null)
        {
            Debug.LogError("PreloadAssetsContainer or materials list is not assigned.");
            return;
        }

        if (preLoadAssets.shaderVarients == null)
        {
            preLoadAssets.shaderVarients = new List<string>(); // Initialize the list if it's null
        }

        HashSet<string> loggedVariants = new HashSet<string>(); // To prevent duplicates

        foreach (Material material in preLoadAssets.materials)
        {
            if (material == null || material.shader == null) continue;

            string shaderName = material.shader.name; // Get the shader name
            string[] keywords = material.shaderKeywords; // Get the active shader keywords
            string variantString = $"{shaderName}:{string.Join(",", keywords)}"; // Combine shader and keywords

            // Add to the hash set to ensure uniqueness
            if (!loggedVariants.Contains(variantString))
            {
                loggedVariants.Add(variantString);
                Debug.Log($"Collected Shader Variant: {variantString}");
            }
        }

        // Add collected variants to the ScriptableObject
        preLoadAssets.shaderVarients.Clear(); // Clear any old data
        preLoadAssets.shaderVarients.AddRange(loggedVariants);

        // Optional: Log the total collected variants
        Debug.Log($"Total Shader Variants Collected: {preLoadAssets.shaderVarients.Count}");
    }

    // void Update()
    // {
    //     if (resetTriggered && !canReset)
    //     {
    //         resetTimer += Time.deltaTime; // Increment the timer
    //         if (resetTimer >= waitTime)
    //         {
    //             canReset = true;
    //             resetTriggered = false;
    //             // controls.Special.Enable();
    //             resetTimer = 0f;
    //         }
    //     }

    //     // if (canReset)
    //     // {
    //     //     if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetMouseButtonDown(0))
    //     //     {
    //     //         canReset = false;
    //     //         resetTriggered = false; // Reset the trigger
    //     //         ResetGame();
    //     //     }
    //     // }
    // }

    private IEnumerator PreloadAssetsCoroutine()
    {




        // Preload materials
        foreach (Material material in preLoadAssets.materials)
        {
            GameObject tempObject = new GameObject("TempMaterialObject");
            SpriteRenderer renderer = tempObject.AddComponent<SpriteRenderer>();

            renderer.material = material;
            tempObject.transform.position = new Vector3(-1000, -1000, 0);

            // Render the object for one frame
            tempObject.SetActive(true);
            yield return null; // Wait for one frame
            tempObject.SetActive(false);

            // Destroy the temporary object
            Destroy(tempObject);
        }

        // Preload particle systems
        foreach (ParticleSystem ps in preLoadAssets.particles)
        {
            ParticleSystem tempPS = Instantiate(ps);
            tempPS.transform.position = new Vector3(-1000, -1000, 0);
            tempPS.gameObject.SetActive(true);
            tempPS.Play();
            yield return new WaitForSeconds(0.1f); // Wait for a short duration
            Destroy(tempPS.gameObject);
        }


        loadedAssets = true;


    }

    public void StartResetTime()
    {
        // resetTriggered = true; // Set the reset trigger
        ResetManager.GameOverEvent?.Invoke();
    }

    // void OnEnable()
    // {
    //     PlayerManager.onPlayerDeath += StartResetTime;
    // }

    // void OnDisable()
    // {
    //     PlayerManager.onPlayerDeath -= StartResetTime;
    // }

    public void ResetGame()
    {


        // canReset = false;
        onRestartGame?.Invoke();
        // controls.Special.Disable();

        if (GameObject.Find("LevelManager") != null)
        {

            checkPoint = GameObject.Find("LevelManager").GetComponent<LevelManager>().reachedCheckpoint;

            Debug.Log("Check Point is: " + checkPoint);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        lvlID.usingCheckPoint = true;

    }

    public void PausedReset()
    {
        onRestartGame?.Invoke();
        // controls.Special.Disable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void SpecialReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
