using UnityEngine;

public class ParryableAttack : MonoBehaviour, IParryable
{
    private TenderizerPig pig;
    [SerializeField] private float parryWin;

    public float parryWindow => parryWin;
    private Animator anim;

    private void Start()
    {
        // pig = GetComponentInParent<TenderizerPig>();
        anim = GetComponentInParent<Animator>();

    }
    public void Glow(float duration)
    {

    }

    public void Parry()
    {
        // if (pig != null)
        //     pig.GetParried();
        if (anim != null) anim.SetTrigger("Parried");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


}
