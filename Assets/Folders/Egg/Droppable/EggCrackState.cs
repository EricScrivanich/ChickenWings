using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCrackState : EggBaseState
{
    public override void EnterState(EggStateManager egg)
    {
        
        egg.rb.simulated = false;
        egg.coll2D.enabled = false;
        Animator anim = egg.GetComponent<Animator>();
        anim.SetBool("Cracked",true);
        Debug.Log("crakcedddd");
        
        
    }

    public override void OnTriggerEnter2D(EggStateManager egg, Collider2D collider)
    {
        
    }

    public override void UpdateState(EggStateManager egg)
    {
        egg.transform.Translate( -5 * Time.deltaTime,0,0);
        if (egg.transform.position.x < BoundariesManager.leftBoundary)
        {
            egg.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
   
}
