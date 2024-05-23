using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliDamaged : MonoBehaviour, IDamageable
{
    [SerializeField] private HelicopterID ID;
    public void Damage(int damageAmount)
    {
        ID.events.Damaged(damageAmount);
    }

   
}
