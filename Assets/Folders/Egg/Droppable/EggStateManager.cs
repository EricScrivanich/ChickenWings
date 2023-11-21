
using UnityEngine;

public class EggStateManager : MonoBehaviour
{

    EggBaseState currentState;
    public EggFallingState FallingState = new EggFallingState();
    public EggCrackState CrackingState = new EggCrackState();
    public Rigidbody2D rb {get; private set;}
    public Collider2D coll2D {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>();
        currentState = FallingState;

        currentState.EnterState(this);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        currentState.OnTriggerEnter2D(this, collider);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(EggBaseState state)
    {
        currentState = state;
        state.EnterState(this);

    }
    private void OnDisable() 
    {
        SwitchState(FallingState);
    }
}
