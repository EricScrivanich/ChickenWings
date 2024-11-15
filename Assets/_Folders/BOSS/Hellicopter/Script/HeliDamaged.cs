using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliDamaged : MonoBehaviour, IDamageable
{
    [SerializeField] private HelicopterID ID;
    // public void Damage(int damageAmount)
    // {

    // }

    public void Damage(int damageAmount = 1, int type = -1, int id = -1)
    {
        ID.events.Damaged(damageAmount);
    }
}
