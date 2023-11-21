using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : PlayerSystem
{
    private InputController controls;
    
    private P_Movement playerMovement;

    protected override void Awake()
    {
        base.Awake();


        controls = new InputController();

        // Get references to other scripts
       

        // Bind actions to methods
        controls.Movement.Jump.started += ctx => player.ID.events.OnJump?.Invoke();
        controls.Movement.JumpRight.performed += ctx => player.ID.events.OnFlipRight?.Invoke();
        controls.Movement.JumpLeft.performed += ctx => player.ID.events.OnFlipLeft?.Invoke();
        controls.Movement.Dash.performed += ctx => player.ID.events.OnDash?.Invoke();
        controls.Movement.Drop.performed += ctx => player.ID.events.OnDrop?.Invoke();
        controls.Movement.DropEgg.performed += ctx => player.ID.events.OnEggDrop?.Invoke();

        
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


