
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using System.Collections;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance;
    private bool loadedAssets;

    private GameObject blockButtons;
    [SerializeField] private PreloadAssetsContainer preLoadAssets;
    private InputController controls;
    [SerializeField] private float waitTime; // Time after death before the player may reset
    private float resetTimer; // Timer to track the wait time
    private bool canReset;
    private bool resetTriggered; // Variable to prevent multiple resets
    private PlayerManager playerMan;
    public static event Action onRestartGame;
    private InputAction resetAction;
    public int checkPoint;


    void Awake()
    {
        loadedAssets = false;

        controls = new InputController();

        // Bind the Reset action to a method
        controls.Special.ResetGame.performed += ctx => ResetGame();
        controls.Special.Disable();
        checkPoint = 0;

    }
    void Start()
    {




        playerMan = GetComponent<PlayerManager>();
        canReset = false;
        resetTriggered = false;
        if (SceneManager.GetActiveScene().name == "MainMenu")
            StartCoroutine(PreloadAssetsCoroutine());




    }

    void Update()
    {
        if (resetTriggered && !canReset)
        {
            resetTimer += Time.deltaTime; // Increment the timer
            if (resetTimer >= waitTime)
            {
                canReset = true;
                resetTriggered = false;
                // controls.Special.Enable();
                resetTimer = 0f;
            }
        }

        // if (canReset)
        // {
        //     if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetMouseButtonDown(0))
        //     {
        //         canReset = false;
        //         resetTriggered = false; // Reset the trigger
        //         ResetGame();
        //     }
        // }
    }

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
        resetTriggered = true; // Set the reset trigger
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


        canReset = false;
        onRestartGame?.Invoke();
        controls.Special.Disable();

        if (GameObject.Find("LevelManager") != null)
        {

            checkPoint = GameObject.Find("LevelManager").GetComponent<LevelManager>().reachedCheckpoint;
            Debug.Log("Check Point is: " + checkPoint);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void PausedReset()
    {
        onRestartGame?.Invoke();
        controls.Special.Disable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void SpecialReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
