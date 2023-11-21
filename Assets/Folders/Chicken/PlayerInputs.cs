using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private InputController controls;
    private PlayerMovement playerMovement;
    private PowerUps powerUps;
    private eggStuff eggScript;

    private void Awake()
    {
        controls = new InputController();

        // Get references to other scripts
        playerMovement = GetComponent<PlayerMovement>();
        powerUps = GetComponent<PowerUps>();
        eggScript = GetComponent<eggStuff>();

        // Bind actions to methods
        controls.Movement.Jump.started += ctx =>playerMovement.Jump();
        controls.Movement.Jump.performed += ctx =>playerMovement.StartJumpHold();
        controls.Movement.Jump.canceled += ctx =>playerMovement.StopJumpHold();

        
        controls.Movement.JumpRight.performed += ctx => playerMovement.JumpRight();
        controls.Movement.JumpLeft.performed += ctx => playerMovement.JumpLeft();
        controls.Movement.Dash.performed += ctx => powerUps.StartDash();
        controls.Movement.Drop.performed += ctx => powerUps.StartDrop();
        controls.Movement.Fireball.performed += ctx => powerUps.Fireball();
        controls.Movement.DropEgg.performed += ctx => eggScript.DropEgg();
    }

    private void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }
}
