using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracker : MonoBehaviour
{
    public GameObject pressButtonPrefab;
    private Canvas canvas;
    private FlashGroup pressButton;
    private InputController controls;
    private LevelManager lvlMangager;
    [SerializeField] private PlayerID ID;
    private bool trackInputsBool;
    private bool trackSpecialInputsBool;
    [SerializeField] private bool allInputsCount;
    [SerializeField] private List<string> inputsChecked;
    [SerializeField] private List<string> specialInputsChecked;
    public bool dashSlash;
    private bool showingPressButtons;

    void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        dashSlash = false;
        showingPressButtons = false;


        lvlMangager = GetComponent<LevelManager>();
        controls = new InputController();

        // Bind existing actions to methods

        controls.Movement.Jump.performed += ctx =>
        {
            if (!trackInputsBool) return;
            Debug.Log("Tracked");

            if (CheckInputs("Jump")) ID.events.OnJump?.Invoke();
        };

        controls.Movement.JumpRight.started += ctx =>
        {
            if (!trackInputsBool) return;
            Debug.Log("Tracked");


            if (CheckInputs("FlipRight")) ID.events.OnFlipRight?.Invoke(true);
        };


        controls.Movement.JumpLeft.started += ctx =>
        {
            if (!trackInputsBool) return;

            if (CheckInputs("FlipLeft")) ID.events.OnFlipLeft?.Invoke(true);
        };


        controls.Movement.Dash.performed += ctx =>
        {
            if (!trackInputsBool) return;

            if (CheckInputs("Dash"))
            {
                Debug.Log("STILLL TRACKKKKKING");

                ID.events.OnDash?.Invoke(true);
            }
        };


        controls.Movement.Drop.performed += ctx =>
        {
            if (!trackInputsBool) return;

            if (CheckInputs("Drop")) ID.events.OnDrop?.Invoke();
        };
        controls.Movement.DropEgg.performed += ctx =>
        {
            if (!trackInputsBool) return;

            if (CheckInputs("Egg")) ID.events.OnEggDrop?.Invoke();
        };

    }

    public void EnableTracking(bool track)
    {
        trackInputsBool = track;
        trackSpecialInputsBool = false;
        ShowPressButtons(track);

        Debug.Log("tracking eneabled is: " + trackInputsBool);
    }

    private void ShowPressButtons(bool show)
    {
        if (show)
        {
            if (pressButton.gameObject.activeInHierarchy)
            {
                pressButton.gameObject.SetActive(false);
            }
            pressButton.gameObject.SetActive(true);
            showingPressButtons = true;

        }

        else
        {
            if (showingPressButtons)
            {
                pressButton.FadeOut();
                showingPressButtons = false;

            }
        }

    }




    public bool CheckInputs(string normalInput)
    {

        if (allInputsCount)
        {
            lvlMangager.NextUIFromInput(0);

            return true;
        }
        else
        {

            foreach (var type in inputsChecked)
            {
                if (type == normalInput)
                {
                    lvlMangager.NextUIFromInput(0f);

                    return true;


                }
            }




        }
        return false;
    }
    private void Start()
    {
        // pressButton = Instantiate(pressButtonPrefab).GetComponent<FlashGroup>();
        // pressButton.transform.parent = canvas.transform;
        // pressButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -355);

        // string s = "";

        // if (allInputsCount) s = "any";
        // else if (inputsChecked.Count == 1) s = ("the " + inputsChecked[0]);
        // else if (inputsChecked.Count == 2 && (inputsChecked[0] == "FlipLeft" || inputsChecked[0] == "FlipRight"))
        // {
        //     s = ("a Flip");
        // }

        // pressButton.SetText(s);
        // pressButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Movement.Enable();

    }

    private void OnDisable()
    {
        controls.Movement.Disable();


    }
    // Update is called once per frame

}
