
using UnityEngine;

public class HeliStateManager : MonoBehaviour
{
    public HelicopterID ID;

    public GameObject player;
    HeliBaseState currentState;
    public HeliNormalState NormalState = new HeliNormalState();
    public HeliFlipState FlipState = new HeliFlipState();
    public HeliFollowState FollowState = new HeliFollowState();
    
    public Rigidbody2D rb {get; private set;}
  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        currentState = NormalState;

        currentState.EnterState(this);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(HeliBaseState state)
    {
        currentState = state;
        state.EnterState(this);

    }
    private void OnDisable() 
    {
       
    }
}
