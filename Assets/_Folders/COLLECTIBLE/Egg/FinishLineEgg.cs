using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishLineEgg : MonoBehaviour, ICollectible
{
    private Animator anim;

    [SerializeField] private LevelManagerID levelManagerID;
    [SerializeField] private float moveDuration;

    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;

    public void Collected()
    {
        DOTween.Kill(this);

        Invoke("InvokedFinish", .35f);
        AudioManager.instance.PlayCrackSound(1);
        anim.SetTrigger("Burst");


    }

    private void InvokedFinish()
    {
        levelManagerID.inputEvent.OnEggFinishLine?.Invoke();

    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();


        transform.DOMoveX(endPos.x, moveDuration);

    }

    // Update is called once per frame

}
