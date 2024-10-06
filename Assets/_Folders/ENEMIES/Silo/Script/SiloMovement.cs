using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiloMovement : MonoBehaviour
{
    [SerializeField] private ExplosivesPool pool;
    [SerializeField] private Vector2 startPos;

    [SerializeField] private SiloCannon leftCannon;
    [SerializeField] private SiloCannon rightCannon;

    [SerializeField] private SpriteRenderer leftCover;
    [SerializeField] private SpriteRenderer rightCover;

    [SerializeField] private Transform siloBase;
    [SerializeField] private BoxCollider2D siloBaseCollider;

    private bool usingLeftCannon;
    private bool usingRightCannon;
    [SerializeField] private float resetTime;

    [SerializeField] private bool ignoreMove;
    private float timer = 0;

    public int type;
    public float baseHeightMultiplier;
    private bool moving;
    private bool aimingLeft = false;
    private bool reset = false;

    private Transform playerTransform;
    // Start is called before the first frame update


    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;



        leftCannon.SetTarget(playerTransform);
        rightCannon.SetTarget(playerTransform);








    }

    private void UpdateSiloBaseScale()
    {
        if (siloBaseCollider == null)
        {
            Debug.LogError("BoxCollider2D reference not set on siloBase.");
            return;
        }

        // Calculate the desired height from the siloBase's top (its pivot assumed to be at the top) to the ground
        float desiredHeight = Mathf.Abs(siloBaseCollider.transform.position.y - BoundariesManager.GroundPosition);

        // Calculate the necessary scale factor
        float currentHeight = siloBaseCollider.size.y * siloBaseCollider.transform.localScale.y;
        float scaleRatio = desiredHeight / currentHeight;

        // Apply this scale ratio to the y-scale of siloBase directly
        siloBaseCollider.transform.localScale = new Vector3(siloBaseCollider.transform.localScale.x, scaleRatio, siloBaseCollider.transform.localScale.z);
    }

    private void OnEnable()
    {
        if (type == 0)
        {
            usingLeftCannon = false;
            usingRightCannon = false;

        }
        else if (type == 1)
        {
            usingLeftCannon = true;
            usingRightCannon = false;

        }
        else if (type == 2)
        {
            usingLeftCannon = false;
            usingRightCannon = true;

        }
        else if (type == 3)
        {
            usingLeftCannon = true;
            usingRightCannon = true;
        }

        UpdateSiloBaseScale();
        leftCannon.gameObject.SetActive(usingLeftCannon);
        rightCannon.gameObject.SetActive(usingRightCannon);
        leftCover.enabled = !usingLeftCannon;
        rightCover.enabled = !usingRightCannon;
        aimingLeft = false;
        leftCannon.SetAiming(false);
        rightCannon.SetAiming(false);
    }


    private void CheckSide()
    {
        if (aimingLeft && playerTransform.position.x > transform.position.x)
        {
            aimingLeft = false;
            leftCannon.SetAiming(false);
            rightCannon.SetAiming(true);
        }
        else if (!aimingLeft && playerTransform.position.x < transform.position.x && transform.position.x < BoundariesManager.rightBoundary)
        {
            aimingLeft = true;
            leftCannon.SetAiming(true);
            rightCannon.SetAiming(false);

        }

    }

    // Update is called once per frame
    void Update()
    {


        CheckSide();
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
        if (transform.position.x < BoundariesManager.leftPlayerBoundary - 2)
        {
            gameObject.SetActive(false);
        }




    }
}
