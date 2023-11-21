using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFallingState : EggBaseState
{
   public override void EnterState(EggStateManager egg)
    {
        
        egg.rb.simulated = true;
        egg.coll2D.enabled = true;
        
        
    }

    public override void OnTriggerEnter2D(EggStateManager egg, Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Floor"))
        {
     
            
            egg.SwitchState(egg.CrackingState);
        }
        if (collider.gameObject.CompareTag("Barn"))
        {
           
            egg.gameObject.SetActive(false);
            
        }
        
    }

    public override void UpdateState(EggStateManager egg)
    {
      
    }

}
