using System.Collections.Generic;
using UnityEngine;


public class Scythe : MonoBehaviour
{

    [SerializeField] private bool useStuck;
    [SerializeField] private float activateBox1Time;
    [SerializeField] private float deactivateBox1Time;
    [SerializeField] private float activateBox2Time;
    [SerializeField] private float activateBox3Time;
    private List<Collider2D> blockedObjects; // Stores objects after block detection

    [SerializeField] private Transform rayTopOrg;
    [SerializeField] private Transform rayOrg;
    [SerializeField] private Transform rayOrg2;
    [SerializeField] private Transform rayTop;
    [SerializeField] private Transform raySide;
    [SerializeField] private Transform raySide2;
    private int damageAmount = 1;

    [SerializeField] private BoxCollider2D[] cols;
    private float timer;

    private Animator anim;

    private byte activeBox;
    private Transform playerTran;
    private Vector3 scaleTopFront = new Vector3(1, 1, 1);
    private Vector3 scaleTopBack = new Vector3(-1, 1, 1);
    private Vector3 scaleBotFront = new Vector3(1, -1, 1);
    private Vector3 scaleBotBack = new Vector3(-1, -1, 1);

    private bool box1Active;
    private bool box2Active;
    private bool box3Active;


    private bool follow;



    private float maxRotation = 80;

    private void CheckForBlockingObjects()
    {
        bool foundBlock = false;
        blockedObjects = new List<Collider2D>();

        // First Raycast: From rayOrg to rayTop
        RaycastHit2D[] hits1 = Physics2D.RaycastAll(rayOrg.position, (rayTop.position - rayOrg.position).normalized, Vector2.Distance(rayOrg.position, rayTop.position));
        Debug.DrawLine(rayOrg.position, rayTop.position, Color.red, 0.5f);

        // Second Raycast: From rayOrg to raySide
        RaycastHit2D[] hits2 = Physics2D.RaycastAll(rayOrg.position, (raySide.position - rayOrg.position).normalized, Vector2.Distance(rayOrg.position, raySide.position));
        Debug.DrawLine(rayOrg.position, raySide.position, Color.blue, 0.5f);

        RaycastHit2D[] hits3 = Physics2D.RaycastAll(rayOrg2.position, (raySide2.position - rayOrg2.position).normalized, Vector2.Distance(rayOrg2.position, raySide2.position));
        Debug.DrawLine(rayOrg.position, raySide.position, Color.blue, 0.5f);


        // Combine both raycast results into a single array
        List<RaycastHit2D> allHits = new List<RaycastHit2D>(hits1);
        allHits.AddRange(hits2);
        allHits.AddRange(hits3);

        for (int i = 0; i < allHits.Count; i++)
        {
            if (allHits[i].collider == null) continue;

            if (!foundBlock && allHits[i].collider.CompareTag("Block"))
            {
                foundBlock = true;
                Debug.Log("Blocking object detected: " + allHits[i].collider.name);
            }
            else if (foundBlock)
            {
                // If we've already found a block, store all future allHits[i]s
                blockedObjects.Add(allHits[i].collider);
                Debug.Log("Adding to blocked list: " + allHits[i].collider.name);
            }
        }
    }


    public void SetColorsAndPlayer(Color col, Transform p)
    {
        playerTran = p;
        Transform s = transform.Find("Scythe");

        Transform blur = s.Find("Blur");
        Transform swoop = s.Find("Swoop");

        Transform spark = transform.Find("Spark");



        // Set the color if they exist and have a SpriteRenderer
        if (blur != null)
        {
            SpriteRenderer blurRenderer = blur.GetComponent<SpriteRenderer>();
            if (blurRenderer != null) blurRenderer.color = col;
        }

        if (spark != null)
        {
            SpriteRenderer sparkRenderer = spark.GetComponent<SpriteRenderer>();
            if (sparkRenderer != null) sparkRenderer.color = col;
        }

        if (swoop != null)
        {
            SpriteRenderer swoopRenderer = swoop.GetComponent<SpriteRenderer>();
            if (swoopRenderer != null) swoopRenderer.color = col;
        }


    }

    public LayerMask enemyLayer; // Assign enemy layer
    public LayerMask blockingLayer; // Assign blocking objects like shields

    private bool IsBlocked(Vector2 attackPosition, Vector2 enemyPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPosition, (enemyPosition - attackPosition).normalized, Vector2.Distance(attackPosition, enemyPosition), blockingLayer);

        if (hit.collider != null)
        {
            Debug.Log("Attack Blocked by: " + hit.collider.name);
            return true; // Blocked by something
        }

        return false; // No block, attack goes through
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Attack(byte type)
    {
        switch (type)
        {
            case 0:
                transform.localScale = scaleTopFront;
                break;
            case 1:
                transform.localScale = scaleBotFront;
                break;

            case 2:
                transform.localScale = scaleBotBack;
                break;
            case 3:
                transform.localScale = scaleTopBack;
                break;
        }
        // transform.position = playerTrans.position;

        gameObject.SetActive(true);

    }
    private bool hasBeenBlocked;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Hit object: " + collider.gameObject + "Active box is: " + activeBox);
        if (hasBeenBlocked) return;

        // if (IsBlocked(cols[activeBox - 1].transform.position, collider.transform.position))
        // {
        //     AudioManager.instance.PlayParrySound();
        //     hasBeenBlocked = true;
        //     return;
        // }
        if (blockedObjects.Contains(collider))
        {
            AudioManager.instance.PlayParrySound();
            hasBeenBlocked = true;
            Debug.Log("Attack was blocked by: " + collider.name);
            return;
        }


        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null)
        {


            if (damageAmount == -1 && damageableEntity.CanPerfectScythe)
            {
                if (anim == null) anim = GetComponent<Animator>();
                anim.SetTrigger("Stuck");
                follow = true;
            }

            damageableEntity.Damage(damageAmount, 3);

        }


    }

    public void SwipeAttack(Vector2 pos, float angle, Vector3 scale)
    {
        follow = true;
        damageAmount = 1;
        SetCollidersUnactive();
        timer = 0;
        activeBox = 0;
        transform.position = pos;
        transform.localScale = scale;
        transform.eulerAngles = Vector3.forward * angle;
        hasBeenBlocked = false;

        gameObject.SetActive(true);
        CheckForBlockingObjects();
        AudioManager.instance.PlaySwordSlashSound();

    }

    public void StopFollow()
    {
        follow = false;
    }
    private void LateUpdate()
    {
        if (follow)
            transform.position = playerTran.position;
    }

    private void SetCollidersUnactive()
    {
        foreach (var c in cols)
        {
            c.enabled = false;
        }

    }

    public void AttackSwipe(float angle)
    {


        // switch (type)
        // {
        //     case 0:
        //         transform.localScale = scaleTopFront;
        //         break;
        //     case 1:
        //         transform.localScale = scaleBotFront;
        //         break;

        //     case 2:
        //         transform.localScale = scaleBotBack;
        //         break;
        //     case 3:
        //         transform.localScale = scaleTopBack;
        //         break;
        // }

        if (angle == 0) angle = 360;

        // Debug.Log("Recieved angle of: " + angle);

        if (angle >= 270)
        {
            transform.localScale = scaleTopFront;
            SetRotation(-ReturnLerpedAngle(360 - angle));

        }
        else if (angle <= 90)
        {
            transform.localScale = scaleBotFront;
            SetRotation(ReturnLerpedAngle(Mathf.Abs(angle)));
        }
        else if (angle > 90 && angle < 180)
        {
            transform.localScale = scaleBotBack;
            SetRotation(-ReturnLerpedAngle(180 - angle));
        }
        else
        {
            transform.localScale = scaleTopBack;
            SetRotation(ReturnLerpedAngle(angle - 180));
        }
        // else if (angle <)
        // transform.position = playerTrans.position;



        gameObject.SetActive(true);
        Debug.Log("Scythe rotation is: " + transform.rotation.eulerAngles);

    }

    private void FixedUpdate()
    {

        timer += Time.fixedDeltaTime;
        // if (box1Active)
        // {
        //     cols[0].enabled = false;
        //     box1Active = false;
        //     return;
        // }
        if (box2Active)
        {
            cols[1].enabled = false;
            box2Active = false;
            return;
        }
        else if (box3Active)
        {
            cols[2].enabled = false;
            box3Active = false;
            return;
        }

        if (activeBox == 0 && timer >= activateBox1Time)
        {
            activeBox = 1;

            cols[0].enabled = true;
            box1Active = true;

        }
        else if (activeBox == 1 && box1Active && timer >= deactivateBox1Time)
        {
            cols[0].enabled = false;
            box1Active = false;

            if (useStuck)
                damageAmount = -1;
        }

        if (activeBox == 1 && timer >= activateBox2Time)
        {
            // cols[0].enabled = false;
            // box1Active = false;
            // damageAmount = -1;
            activeBox = 2;

            cols[1].enabled = true;
            box2Active = true;


        }
        else if (activeBox == 2 && timer >= activateBox3Time)
        {
            activeBox = 3;

            cols[2].enabled = true;
            box3Active = true;


        }

    }

    private float ReturnLerpedAngle(float dif)
    {
        float p = Mathf.Lerp(0, maxRotation, dif / 90);
        Debug.Log("Difference is: " + dif + " added angle is: " + p);
        return p;
    }

    private void SetRotation(float angle)
    {
        transform.eulerAngles = Vector3.forward * angle;
    }


    // private float SetRotation(float ang)
    // {
    //     if (ang >= 270)
    //     {


    //     }
    //     else if (ang <= 90)
    //     {
    //         transform.localScale = scaleBotFront;
    //     }
    //     else if (ang > 90 && ang < 180)
    //     {
    //         transform.localScale = scaleBotBack;
    //     }
    //     else
    //     {
    //         transform.localScale = scaleTopBack;
    //     }


    // }

    // private void LateUpdate()
    // {
    //     transform.position = playerTrans.position;
    // }


    public void SetUnactive()
    {
        foreach (var c in cols)
        {
            c.enabled = false;
        }
        gameObject.SetActive(false);
    }
}
