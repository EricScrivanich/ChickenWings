// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Player_S : MonoBehaviour
// {
//     public PlayerStateMachine StateMachine { get; private set; }

//     public Animator Anim { get; private set; }
//     public P_FallingState FallingState { get; private set; }
//     public P_JumpState JumpState { get; private set; }
//     [SerializeField] private PlayerData PlayerData;
//     public PlayerInputHandler InputHandler { get; private set; }
//     private Vector2 workspace;
//     private Vector2 CurrentVelocity;
//     private Rigidbody2D rb;

//     private void Awake() 
//     {
//         StateMachine = new PlayerStateMachine();
//         FallingState = new P_FallingState(this, StateMachine, PlayerData,"falling");
//         JumpState = new P_JumpState(this, StateMachine,PlayerData,"jump");

        
//     }

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         Anim = GetComponent<Animator>();
//         // StateMachine.Initialize(FallingState)
//         StateMachine.Initialize(FallingState);
//         InputHandler = GetComponent<PlayerInputHandler>();
//     }
//     private void Update()
//     {
//         CurrentVelocity = rb.velocity;
//         StateMachine.CurrentState.LogicUpdate();
//     }
//     private void FixedUpdate() 
//     {
//         StateMachine.CurrentState.PhysicsUpdate();
        
//     }

//     public void SetVelocity(float velocity)
//     {
//         workspace.Set(0,velocity);
//         rb.velocity = workspace;
        
//     }
// }
