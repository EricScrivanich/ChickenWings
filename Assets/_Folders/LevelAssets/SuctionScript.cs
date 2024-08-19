using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SuctionScript : MonoBehaviour
{

    [SerializeField] private float rotateSpeed;
    private TriggerNextSection parentScript;
    [SerializeField] private PlayerID player;
    [SerializeField] private float duration;
    [SerializeField] private bool clockwise;


    private Collider2D colliderObj;
    private Vector3 initialScale = new Vector3(1, 1, 1);
    // [SerializeField] private CircleCollider2D centerCollider;

    [SerializeField] private Transform spriteTrans;

    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        colliderObj = GetComponent<Collider2D>();
        parentScript = GetComponentInParent<TriggerNextSection>();
        anim = GetComponent<Animator>();

        if (parentScript.isCheckPoint)
        {
            colliderObj.enabled = false;
            MakeSmallTween(true);
        }



    }
    private void Awake()
    {

    }






    private void OnTriggerEnter2D(Collider2D other)
    {



        parentScript.TriggerEventOnEnterSuction();
        player.globalEvents.OnEnterNextSectionTrigger?.Invoke(duration, parentScript.duration, clockwise, transform, parentScript.setPlayerPositionTransform.position, true);
        colliderObj.enabled = false;
        // Vector2 direction = other.transform.position - transform.position;

        // // Determine the angle in degrees
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // // Apply the rotation with an offset of -90 degrees
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
        // // anim.SetTrigger("EnterTrigger");
        // MakeSmallTween();
        MakeSmallTween(false);



    }


    private void MakeSmallTween(bool checkPoint)
    {
        if (checkPoint)
            spriteTrans.DOScale(initialScale * .55f, duration).SetEase(Ease.InSine).SetUpdate(true);

        else
            spriteTrans.DOScale(initialScale * .55f, duration).SetEase(Ease.InSine).OnComplete(() => { parentScript.EnterSection(); });
        // StartCoroutine(ShrinkCollider());



    }
    private IEnumerator ShrinkCollider()
    {
        float elapsedTime = 0;
        float duration = .8f;
        // float startRadius = centerCollider.radius;
        // float targetRadius = startRadius * .3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;



            yield return new WaitForFixedUpdate();

        }




        // parentTriggerCollider.enabled = true;

    }


    private void OnTriggerExit2D(Collider2D other)
    {

        // HandleSectionActivication(false);
        // player.events.EnableButtons(true);

    }


    private void OnEnable()
    {
        // player.globalEvents.OnFinishSectionTrigger += SetSectionActive;
        spriteTrans.localScale = initialScale;

    }

    private void OnDisable()
    {
        // player.globalEvents.OnFinishSectionTrigger -= SetSectionActive;


    }

    // Update is called once per frame

}
