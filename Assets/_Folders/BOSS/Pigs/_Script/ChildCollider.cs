using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour, IDamageable
{
    private SpawnedPigObject parentScript;

    public bool CanPerfectScythe => true;

    public void Damage(int damageAmount, int bullet, int id)
    {
        parentScript.Damage(damageAmount, bullet, id);
    }

    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<SpawnedPigObject>();
    }

    // Update is called once per frame

}
