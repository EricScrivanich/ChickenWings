using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour, IDamageable
{
    private PigMaterialHandler parentScript;
    public void Damage(int damageAmount, int bullet,int id)
    {
        parentScript.Damage(damageAmount,bullet,id);
    }

    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<PigMaterialHandler>();
    }

    // Update is called once per frame
    
}
