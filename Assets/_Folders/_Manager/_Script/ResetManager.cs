
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance;
    private InputController controls;
    [SerializeField] private float waitTime; // Time after death before the player may reset
    private float resetTimer; // Timer to track the wait time
    private bool canReset;
    private bool resetTriggered; // Variable to prevent multiple resets
    private PlayerManager playerMan;
    public static event Action onRestartGame;
    private InputAction resetAction;
    

    void Awake()
    {
        
        controls = new InputController();

        // Bind the Reset action to a method
        controls.Special.ResetGame.performed += ctx => ResetGame();
        controls.Special.Disable();

    }
    void Start()
    {
        
        playerMan = GetComponent<PlayerManager>();
        canReset = false;
        resetTriggered = false;
        
       
        
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
                controls.Special.Enable();
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
       if(canReset)
       {
        canReset = false;
        onRestartGame?.Invoke();
        controls.Special.Disable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SpecialReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
