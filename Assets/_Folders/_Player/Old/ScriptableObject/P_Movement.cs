using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Movement : PlayerSystem
{
    private float floatingForce;
    private float startFloating = .2f;
    private float floatingDuration = .6f;
    private float floatingTimer;
    private float addJumpForceVar;
    private float flipRotVar;
    private float jumpForce = 11.3f;
    private float flipForceX = 6.5f;
    private float flipForceY = 10f;
    private int flipRotSpeed = 320;
   

    
   protected override void Awake()
    {
        base.Awake();
      
        
        addJumpForceVar = floatingForce;
       
        flipRotVar = 0;
       
        
       
        
       
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(0, 5f);  
    }
    private void Update() 
    {
        if (rb.velocity.y <= 0)
        {
            

        }
    }

    public void Jump()
    {
         if (player.DisableButtons)
        {
            return;
        }
        rb.velocity = new Vector2(0, jumpForce);
        AudioManager.instance.PlayCluck();
        anim.SetTrigger("JumpTrigger");
        // transform.rotation = Quaternion.Euler(0, 0, 20);
        if (player.isFlipping)
        {
            player.justFlipped = true;
            player.isFlipping = false;
        }
        
    }

     public void FlipRight(bool holding)
    {
         if (player.DisableButtons && !holding)
        {
            return;
        }
       
        rb.velocity = new Vector2(flipForceX, flipForceY);
        player.FlipRotVar = -flipRotSpeed;
     
        player.isFlipping = true;
        AudioManager.instance.PlayCluck();
        
    
        
    }

    public void FlipLeft(bool holding)
    { 
        if (player.DisableButtons && !holding)
        {
            return;
        }
       

        rb.velocity = new Vector2(-flipForceX - .2f, flipForceY - .4f);
        player.FlipRotVar = flipRotSpeed;
        
        player.isFlipping = true;
        AudioManager.instance.PlayCluck();
        

        
    }
    public void Dash()
    {

    }
    public void Drop()
    {

    }

    void OnEnable()
    {
        player.ID.events.OnJump += Jump;
        player.ID.events.OnFlipRight += FlipRight;
        player.ID.events.OnFlipLeft += FlipLeft;
    }
     void OnDisable()
    {
        player.ID.events.OnJump -= Jump;
        player.ID.events.OnFlipRight -= FlipRight;
        player.ID.events.OnFlipLeft -= FlipLeft;
    }
}
