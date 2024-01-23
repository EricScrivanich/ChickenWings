using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCooldown;
    public BoxCollider2D slashBox;
    private float slashDuration = .4f;
    private float slashTime;
    private Animator anim;
    private bool canSlash; 
    // Start is called before the first frame update
    void Awake()
    {
        canSlash = true;
    }
    void Start()
    {
        anim = GetComponent<Animator>(); 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Attack() 
    {
        if (canSlash)
        {
            anim.SetTrigger("Slash");
            StartCoroutine(AttackTime());
        }
        
    }

    private IEnumerator AttackTime()
    {
        canSlash = false;
        slashBox.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        slashBox.enabled = false;
        yield return new WaitForSeconds(attackCooldown);
        canSlash = true;

        


    }

    private void OnEnable() {
       
        // ID.events.OnAttack += Attack;
    }
    private void OnDisable() 
    {
        
        // ID.events.OnAttack -= Attack;
       
    }
}
