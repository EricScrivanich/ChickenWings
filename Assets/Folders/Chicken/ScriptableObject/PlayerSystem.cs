
using UnityEngine;

public abstract class PlayerSystem : MonoBehaviour
{
    protected Player player;
    protected Transform _transform;
    
    
    protected float originalGravityScale;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected virtual void Awake()
    {

        player = transform.root.GetComponent<Player>();
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalGravityScale = rb.gravityScale;
       
    }
}
    // private void Update() {
    //     PlayerRotation();
    // }

    // protected virtual void PlayerRotation()
    // {

    // }

    

