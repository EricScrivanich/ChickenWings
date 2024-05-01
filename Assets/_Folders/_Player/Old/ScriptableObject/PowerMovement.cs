using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMovement : PlayerSystem
{
    private bool canDash;
    private bool isDashing;
    private Vector2 dashVector;
    private Vector2 dropVector;
    [SerializeField] private float dashPower = 10.2f;
    [SerializeField] private float dashingTime = .35f;
    [SerializeField] private float dashCoolDown = 1.5f;
    [SerializeField] private float dropCoolDown = 2f;
    private float bounceHeight = 10f;

    [SerializeField] private float dropPower;
    private bool canDrop = true;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        canDash = true;
        dashVector = new Vector2 (dashPower,0);
        dropVector = new Vector2 (0,dropPower);

        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (player.isDashing)
        {
            rb.velocity = dashVector;
        }
        if (player.isDropping)
        {
            rb.velocity = dropVector;
        }
    }
    private void StartDash(bool nothing)
    {
        if (canDash && !player.DisableButtons)
        {
            StartCoroutine(Dash());
        }
       
    }
    private void StartDrop()
    {
        if (canDrop && !player.DisableButtons)
        {
            StartCoroutine(Drop());
        }
       
    }
    private void Bounce()
    {
        player.isDropping = false;
         anim.SetBool("DropBool",false);
        anim.SetTrigger("BounceTrigger");
            AudioManager.instance.PlayBounceSound();
            StartCoroutine(ApplyBounceAfterDelay()); 
            
           

            // playerMovement.maxFallSpeed = maxFallSpeed;
    }

    private IEnumerator ApplyBounceAfterDelay()
{
    yield return new WaitForSeconds(.1f);
    rb.velocity = new Vector2(.5f, bounceHeight);
    player.DisableButtons = false;
}

    private IEnumerator Dash()
    {
        
        player.isDashing = true;
        // transform.rotation = Quaternion.Euler(0, 0, 0);
        player.DisableButtons = true;
        player.isFlipping = false;
        rb.gravityScale = 0f;
        anim.SetTrigger("DashTrigger");
        AudioManager.instance.PlayDashSound();
        yield return new WaitForSeconds(dashingTime);
        canDash = false;
        rb.gravityScale = originalGravityScale;
        player.DisableButtons = false;
        rb.velocity = new Vector2 (2f, 0);
        player.isDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }


    private IEnumerator Drop()
    {
        
        player.isDropping = true;
        anim.SetBool("DropBool",true);
        // transform.rotation = Quaternion.Euler(0, 0, 0);
        player.DisableButtons = true;
        player.isFlipping = false;
        AudioManager.instance.PlayDownJumpSound();
        canDrop = false;
        yield return new WaitForSeconds(dropCoolDown);
        canDrop = true;
    }

    void OnEnable()
    {
        player.ID.events.OnDash += StartDash;
        player.ID.events.OnBounce += Bounce;
        player.ID.events.OnDrop += StartDrop;
       
    }
     void OnDisable()
    {
        player.ID.events.OnDash -= StartDash;
        player.ID.events.OnBounce -= Bounce;
        player.ID.events.OnDrop -= StartDrop;
     
    }
}

