using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class Egg_Regular : MonoBehaviour
{
    private Vector2 startVelocity;
    [SerializeField] private GameObject player;
    private Rigidbody2D playerRB;
    public PlayerID ID;
    private Rigidbody2D rb;
    private BoxCollider2D coll2D;
    private bool isCracked;
    // private Animator anim;
    private bool isFirstActivation = true;
    private bool hitParticle = false;
    private bool hasHit = false;
    private Pool pool;

    private Vector2 normalColSize = new Vector2(1, 1.3f);
    private Vector2 expandedColSize = new Vector2(1.9f, 1.3f);

    private bool colliderIsExpanded = false;


    // private Vector2 playerForce;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<BoxCollider2D>();
        // coll2D = GetComponent<Collider2D>();
        // anim = GetComponent<Animator>();
        // ready = true;


    }
    private void Start()
    {
        pool = PoolKit.GetPool("EggPool");
    }

    // private void OnEnable()
    // {

    //     // if (colliderIsExpanded)
    //     //     coll2D.size = normalColSize;
    // }

    public void Initialize(float force)
    {
        hasHit = false;
        hitParticle = false;
        rb.linearVelocity = new Vector2(force, -1);

    }


    // Update is called once per frame
    // void Update()
    // {


    //     if (isCracked)
    //     {
    //         transform.Translate(-5 * Time.deltaTime, 0, 0);
    //         if (transform.position.x < BoundariesManager.leftBoundary)
    //         {

    //             gameObject.SetActive(false);
    //         }

    //     }


    // }

    // IEnumerator YolkMovement()
    // {

    //     while (transform.position.x > BoundariesManager.leftBoundary)
    //     {
    //         transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
    //         yield return null;

    //     }
    //     anim.SetTrigger("EnterTrigger");
    //     yield return new WaitForSeconds(.1f);
    //     gameObject.SetActive(false);
    // }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (hasHit) return;
        else if (collider.gameObject.CompareTag("Barn"))
        {
            hasHit = true;
            ID.AddScore(5);
            AudioManager.instance.PlayScoreSound();
            // pool.Despawn(gameObject);
            // return;

        }


        else if (collider.gameObject.CompareTag("Floor"))
        {

            hasHit = true;
            // pool.Spawn("EggParticle", transform.position, Vector3.zero);
            // StartCoroutine(YolkMovement());
            AudioManager.instance.PlayCrackSound();
            pool.Spawn("YolkParent", transform.position, Vector3.zero);
            // pool.Despawn(gameObject);

            // isCracked = true;



        }
        else if (collider.gameObject.CompareTag("Plane"))
        {
            IEggable eggableEntity = collider.gameObject.GetComponent<IEggable>();

            if (eggableEntity != null)
            {
                AudioManager.instance.PlayCrackSound();

                // pool.Despawn(gameObject);
                // pool.Spawn("EggParticle", transform.position, Vector3.zero);

                eggableEntity.OnEgged();
            }
        }
        gameObject.SetActive(false);


    }

    private IEnumerator ExpandColliderForDurationAfterParticleCollision()
    {
        yield return new WaitForSeconds(.2f);
        coll2D.size = normalColSize;
        colliderIsExpanded = false;


    }

    private void ResetParticleColllider()
    {
        hitParticle = false;

    }


    private void OnParticleCollision(GameObject other)
    {

        if (!hitParticle)
        {
            SmokeTrailPool.GetEggParticleCollider?.Invoke(transform.position, rb.linearVelocity);
            hitParticle = true;
            Invoke("ResetParticleColllider", .25f);
            // coll2D.size = expandedColSize;
            // colliderIsExpanded = true;
            // StartCoroutine(ExpandColliderForDurationAfterParticleCollision());


        }
        // Debug.LogError("Egg Hit Smoke");
    }
    // private void OnEnable()
    // {


    //     // rb.velocity = new Vector2(ID.AddEggVelocity, -1.5f);




    // }

    private void OnDisable()
    {


        rb.linearVelocity = Vector2.zero;

    }


}
// private void OnEnable() {


//     if (!isFirstActivation)
//     {
//         rb.velocity = new Vector2(ID.AddEggVelocity, rb.velocity.y);
//     }
//     else
//     {
//         isFirstActivation = false;
//         return;
//     }
// }





// }
// private void GetVelocity(float velx)
// {
//     rb.velocity = new Vector2(velx,rb.velocity.y);

// }

// private void OnEnable() {
//     player.globalEvents.eggVelocity += GetVelocity;
// }
//  private void OnDisable() {
//     player.globalEvents.eggVelocity += GetVelocity;
// }



