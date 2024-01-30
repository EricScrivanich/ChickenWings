using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDamage : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private int damageAmount = 1;  // Amount of damage the slash does

    private void OnTriggerEnter2D(Collider2D collider)
    {
        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null)
        {
            damageableEntity.Damage(damageAmount);
        }
    }

    public void AttackFinished()
    {
        Debug.Log("finished");
        ID.events.OnAttack?.Invoke(false);
    }
}
