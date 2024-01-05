using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigStateManager : MonoBehaviour
{
    [ExposedScriptableObject]
    public PigID ID;
    public Rigidbody2D rb;
    public Animator anim { get; private set; }
    public Animator animWings { get; private set; }

    public Transform playerTransform;

    PigBaseState currentState;

    public PigIdleState IdleState = new PigIdleState();
    public PigAttackState AttackState = new PigAttackState();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentState = AttackState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);

        MaxFallSpeed();
    }

    public void SwitchState(PigBaseState newState)
    {
        currentState = newState;
        newState.EnterState(this);
    }

    private void MaxFallSpeed()
    {

    }

    
}
