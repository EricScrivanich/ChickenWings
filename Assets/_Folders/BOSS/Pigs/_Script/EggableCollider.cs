using UnityEngine;
using System.Collections;

public class EggableCollider : MonoBehaviour
{
    private SpawnedObject thisObject;
    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private float blindedDuration;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isHit = false;
    private bool isEgged = false;
    private float baseScale;
    private Vector2 initialScalePercent = new Vector2(.85f, 1.15f);
    private float scaleDuration = .4f;
    private float switchSpriteTime = .2f;
    private float timer;
    private bool colliderChecked = false;
    private bool switchSprite = false;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private float addedEggVel = 2.3f;


    private float yMoveAmount = .6f;
    private float baseYpos;
    private float baseXpos;
    [SerializeField] private bool useOverrideLogic = false;



    public void GetEgged(Egg_Regular egg)
    {
        // eggable.OnEgged();
        if (isEgged) return;

        isEgged = true;
        float angle = transform.position.x - egg.transform.position.x;
        transform.position = new Vector2(Mathf.Lerp(egg.transform.position.x, transform.position.x, 0.4f), transform.position.y);
        StartCoroutine(OnEggRoutine(egg, angle));

    }
    public void DoSprite()
    {


        spriteRenderer.sprite = animData.sprites[0];
        spriteRenderer.transform.localScale = new Vector3(baseScale * initialScalePercent.x, baseScale * initialScalePercent.y, 1);
        transform.localPosition = new Vector3(transform.localPosition.x, baseYpos + yMoveAmount, transform.localPosition.z);
        spriteRenderer.enabled = true;
        timer = 0;
        this.enabled = true;
    }
    void Update()
    {

        timer += Time.deltaTime;
        float scaleX = Mathf.Lerp(baseScale * initialScalePercent.x, baseScale, timer / scaleDuration);
        float scaleY = Mathf.Lerp(baseScale * initialScalePercent.y, baseScale, timer / scaleDuration);
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(baseYpos + yMoveAmount, baseYpos, timer / scaleDuration), transform.localPosition.z);
        transform.localScale = new Vector3(scaleX, scaleY, 1);

        if (!switchSprite && timer >= switchSpriteTime)
        {
            spriteRenderer.sprite = animData.sprites[1];
            switchSprite = true;
        }
        else if (switchSprite && timer >= scaleDuration)
        {
            boxCollider.enabled = true;
            // isEgged = false;
            timer = 0;
            this.enabled = false;
        }



    }

    public void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // try and get the SpawnedObject component int parent, if null check nextparent, and do one more if still null
        thisObject = GetComponentInParent<SpawnedObject>();
        int tryCount = 0;
        if (thisObject == null)
        {
            Transform parent = transform.parent;
            while (parent != null && thisObject == null && tryCount < 3)
            {
                thisObject = parent.GetComponent<SpawnedObject>();
                parent = parent.parent;
                tryCount++;
            }
        }


        spriteRenderer.enabled = false;
        baseScale = transform.localScale.x;
        baseYpos = transform.localPosition.y;
        baseXpos = transform.localPosition.x;
        timer = 0;
    }

    private IEnumerator OnEggRoutine(Egg_Regular egg, float angle)
    {
        Vector2 lastPos = thisObject.GetPosition();
        yield return waitForEndOfFrame;
        yield return waitForFixedUpdate;

        DoSprite();
        // float speed
        egg.EggPig();
        if (blindedDuration > 0)
        {
            thisObject.EggPig(0, Vector2.zero, 0);
            // float head
            yield return new WaitForSeconds(blindedDuration);


            thisObject.EggPig(-1, Vector2.zero, 0);

        }
        else
        {
            Vector2 currentVelocity = (thisObject.GetPosition() - lastPos) / Time.fixedDeltaTime;
            Debug.Log("EggableCollider currentVelocity: " + currentVelocity);
            currentVelocity.y = (currentVelocity.y * .5f) - addedEggVel;

            thisObject.EggPig(0, currentVelocity, angle);
        }


    }
    public void KillOnGround()
    {
        Debug.Log("EggableCollider KillOnGround");
        thisObject.KillOnGround();
        spriteRenderer.enabled = false;
        transform.localPosition = new Vector2(baseXpos, baseYpos);
    }

    public Transform GetTransform()
    {
        if (!isEgged) return null;
        boxCollider.enabled = false;
        isEgged = false;
        return transform;
    }

    public void SetBoxCollider(bool isActive)
    {
        colliderChecked = isActive;
        if (!isEgged)
            boxCollider.enabled = isActive;

    }



}
// Start is called once before the first execution of Update after the MonoBehaviour is created


// Update is called once per frame


