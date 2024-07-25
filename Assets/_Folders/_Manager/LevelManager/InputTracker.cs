using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracker : MonoBehaviour
{

    private InputController controls;
    private LevelManager lvlMangager;
    [SerializeField] private PlayerID ID;
    private bool trackInputsBool;
    [SerializeField] private bool allInputsCount;
    [SerializeField] private List<string> inputsChecked;

    void Awake()
    {

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

            if (CheckInputs("Dash")) ID.events.OnDash?.Invoke(true);
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
        Debug.Log("tracking eneabled is: " + trackInputsBool);
    }

    public bool CheckInputs(string s)
    {
        if (allInputsCount)
        {
            lvlMangager.NextUIFromInput();

            return true;
        }
        else
        {
            foreach (var type in inputsChecked)
            {
                if (type == s)
                {
                    lvlMangager.NextUIFromInput();

                    return true;


                }
            }

        }
        return false;
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
