using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NinjaPig : MonoBehaviour
{
    // ===========================================
    // General Components and References
    // ===========================================
    [Header("General Components")]
    [SerializeField] private bool doDash;
    [SerializeField] private GameObject sword;
    [SerializeField] private SpriteRenderer swordSheath;
    private Animator anim;
    private EnemyPositioningLogic logic;

    private Vector2 SwingDownLeftOffset = new Vector2(1.9f, .5f);


    // ===========================================
    // Animation and State
    // ===========================================
    [Header("Animation and State")]
    private readonly int TurnLeftHash = Animator.StringToHash("TurnLeft");
    private readonly int TurnRightHash = Animator.StringToHash("TurnRight");
    private readonly int UnsheathHash = Animator.StringToHash("Unsheath");
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int SwingDownLeftHash = Animator.StringToHash("SwingDownLeft");
    private readonly int SwingDownRightHash = Animator.StringToHash("SwingDownRight");
    private readonly int SwingUpLeftHash = Animator.StringToHash("SwingUpLeft");
    private readonly int SwingUpRightHash = Animator.StringToHash("SwingUpRight");
    private readonly Queue<int> animQue = new Queue<int>();
    private bool facingLeft = true;
    private bool isDashing = false;

    // ===========================================
    // Movement
    // ===========================================
    [Header("Movement Settings")]
    [SerializeField] private Transform target1;
    [SerializeField] private Transform target2;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 dashOffset;
    [SerializeField] private float dashSpeed = 10f;
    private Vector2 currentTarget;

    // ===========================================
    // Idle Behavior
    // ===========================================
    [Header("Idle Behavior")]
    [SerializeField] private bool idle = true;
    [SerializeField] private Vector2 idleMovementRange; // Range for each idle movement step
    [SerializeField] private float idleMoveInterval = 2f; // Time between movements
    [SerializeField] private Vector2 idleMovementBoundary; // M
    private Vector2 idleStartPosition;


    [SerializeField] private float idleMovementAmplitude = 1f; // Maximum distance from the initial position
    [SerializeField] private float idleMovementFrequency = 1f; // Speed of oscill


    // ===========================================
    // Dash Clones and Effects
    // ===========================================
    [Header("Dash Clones and Effects")]
    [SerializeField] private ParticleSystem smokeInPrefab;
    [SerializeField] private ParticleSystem smokeOutPrefab;
    [SerializeField] private GameObject dashClonePrefab;
    [SerializeField] private Color[] dashCloneColors;
    [SerializeField] private float dashCloneSpawnDelay;

    private ParticleSystem smokeOut;
    private ParticleSystem smokeIn;
    private NinjaPigDashClone[] dashClones;
    private Transform[] pigParts;
    private SpriteRenderer[] changableSprites;
    private int currentDashCloneIndex;
    private float dashCloneTimer;

    private bool hasTriggeredAttack = false;

    private void OnEnable()
    {
        logic.OnMovementTriggered += TriggerAttack;
    }
    private void OnDisable()
    {
        logic.OnMovementTriggered -= TriggerAttack;
    }

    private void Awake()
    {
        logic = GetComponent<EnemyPositioningLogic>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        smokeOut = Instantiate(smokeOutPrefab);
        smokeIn = Instantiate(smokeInPrefab);

        // currentTarget = target1.position;
        idleStartPosition = transform.position;

        // Initialize dash clones
        dashClones = new NinjaPigDashClone[dashCloneColors.Length];
        for (int i = 0; i < dashCloneColors.Length; i++)
        {
            dashClones[i] = Instantiate(dashClonePrefab).GetComponent<NinjaPigDashClone>();
            dashClones[i].gameObject.SetActive(false);
        }
        // StartCoroutine(Yug());

        // EquipSword(false);


    }

    private void TriggerAttack()
    {
        anim.SetTrigger(SwingDownLeftHash);
    }

    private IEnumerator SwingDown()
    {
        yield return new WaitForSeconds(4f);
        logic.HandleMovement(false);
        anim.SetTrigger("PrepareDownLeft");

        yield return new WaitForSeconds(.7f);

        logic.MoveToPlayerPosition(SwingDownLeftOffset, .4f, .7f, false);

    }

    private void EquipSword(bool equip)
    {
        if (equip)
        {
            anim.SetTrigger(UnsheathHash);
            sword.SetActive(true);
            swordSheath.enabled = false;
            anim.SetTrigger(IdleHash);
        }
        else
        {
            swordSheath.enabled = true;
            sword.SetActive(false);
        }
    }

    // ===========================================
    // Idle Logic
    // ===========================================
    private IEnumerator IdleMovement()
    {
        while (idle)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-idleMovementRange.x, idleMovementRange.x),
                Random.Range(-idleMovementRange.y, idleMovementRange.y)
            );

            Vector2 targetPosition = (Vector2)transform.position + randomOffset;

            // Ensure the target position stays within the idle movement boundary
            targetPosition.x = Mathf.Clamp(targetPosition.x, idleStartPosition.x - idleMovementBoundary.x, idleStartPosition.x + idleMovementBoundary.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, idleStartPosition.y - idleMovementBoundary.y, idleStartPosition.y + idleMovementBoundary.y);

            float elapsedTime = 0f;
            Vector2 startPosition = transform.position;

            while (elapsedTime < idleMoveInterval)
            {
                transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / idleMoveInterval);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition; // Ensure it lands exactly at the target
            yield return new WaitForSeconds(idleMoveInterval);
        }
    }
    // ===========================================
    // Dash Logic
    // ===========================================
    public void DashToPlayer()
    {
        if (!isDashing)
        {
            StartCoroutine(DashTowardsTarget(playerTransform.position + (Vector3)dashOffset));
        }
    }

    private void Attack()
    {


    }

    private IEnumerator Yug()
    {
        while (true)
        {
            yield return new WaitForSeconds(4.5f);
            if (facingLeft)
            {
                if (transform.position.y < playerTransform.position.y)
                {
                    anim.SetTrigger(SwingUpLeftHash);
                }
                else
                {
                    anim.SetTrigger(SwingDownLeftHash);
                }


            }
            else
            {


                if (transform.position.y < playerTransform.position.y)
                {
                    anim.SetTrigger(SwingUpRightHash);
                }
                else
                {
                    anim.SetTrigger(SwingDownRightHash);
                }

            }
            yield return new WaitForSeconds(.3f);
            logic.Attack();
        }
    }

    private void Update()
    {

        if (!hasTriggeredAttack)
        {
            float distanceToPlayer = Vector2.Distance((Vector2)transform.position, playerTransform.position);

            if (distanceToPlayer > 5 && playerTransform.position.y < 0 && playerTransform.position.x < transform.position.x)
            {
                StartCoroutine(SwingDown());
                hasTriggeredAttack = true;
            }
        }
        if (transform.position.x < playerTransform.position.x && facingLeft)
        {
            anim.SetTrigger(TurnRightHash);
            anim.SetTrigger(IdleHash);
            facingLeft = false;
        }
        else if (transform.position.x > playerTransform.position.x && !facingLeft)
        {
            anim.SetTrigger(TurnLeftHash);
            anim.SetTrigger(IdleHash);

            facingLeft = true;

        }
        // if (idle)
        // {
        //     PerformIdleMovement();
        // }

        // if (doDash)
        // {
        //     idle = false;
        //     DashToPlayer();
        //     doDash = false;
        // }
    }

    private void PerformIdleMovement()
    {
        // Calculate sinusoidal offsets for X and Y
        float offsetX = Mathf.Sin(Time.time * idleMovementFrequency) * idleMovementAmplitude;
        float offsetY = Mathf.Cos(Time.time * idleMovementFrequency) * idleMovementAmplitude;

        // Apply the calculated offset to the starting position
        Vector2 targetPosition = idleStartPosition + new Vector2(offsetX, offsetY);

        // Update the pig's position
        transform.position = targetPosition + idleStartPosition;
    }

    private IEnumerator DashTowardsTarget(Vector2 targetPosition)
    {
        isDashing = true;

        // Play swipe down animation
        anim.SetTrigger(facingLeft ? SwingDownLeftHash : SwingDownRightHash);
        yield return new WaitForSeconds(.2f);
        idleStartPosition = targetPosition;

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;



        yield return new WaitForSeconds(.5f);
        anim.SetTrigger(IdleHash);

        // idle = true;
    }

    // ===========================================
    // Animation Queue Logic
    // ===========================================
    public void OnAnimationEnd()
    {
        if (animQue.Count > 0)
        {
            anim.SetTrigger(animQue.Dequeue());
        }
    }

    public void OnFinishSwingPrep(int type)
    {

    }

    public void SwitchState(int type)
    {

    }

    public void GetSmoke()
    {
        smokeIn.transform.position = transform.position;
        smokeIn.Play();
    }

    private IEnumerator SpawnDashClone()
    {
        int startVal = 0;
        while (startVal < dashClones.Length)
        {
            yield return new WaitForSeconds(dashCloneSpawnDelay);

            if (currentDashCloneIndex < dashClones.Length && startVal == 0)
                dashClones[currentDashCloneIndex].Activate(pigParts, changableSprites, dashCloneColors[0]);

            for (int i = startVal; i < currentDashCloneIndex; i++)
            {

                dashClones[i].Recolor(dashCloneColors[currentDashCloneIndex - i]);

            }

            if (currentDashCloneIndex == dashClones.Length - 1)
            {
                dashClones[startVal].gameObject.SetActive(false);
                startVal++;

            }
            else
                currentDashCloneIndex++;


        }
        currentDashCloneIndex = 0;

    }
}




