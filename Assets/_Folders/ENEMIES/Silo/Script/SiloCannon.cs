using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SiloCannon : MonoBehaviour
{
    public float rotationSpeed = 1f;

    [SerializeField] private ExplosivesPool pool;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minYDistance;
    [SerializeField] private float maxYDistance;

    public float shootInterval = 5f;

    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject explosive;
    [SerializeField] private SpriteRenderer smoke;
    [SerializeField] private Sprite[] smokeSprites;

    [SerializeField] private Vector3 smokeStartScale;
    [SerializeField] private Vector3 smokeEndScale;
    [SerializeField] private float[] smokeOpacities;
    [SerializeField] private float spriteChangeTime;
    [SerializeField] private float shootPointMoveAmount;
    [SerializeField] private float shootPointInitialMoveTime;
    [SerializeField] private float shootPointEndMoveTime;

    public float minXRange = -10f;
    public float maxXRange = 10f;
    public float minZRotation = 25f;
    public float maxZRotation = 65f;
    public Vector2 localMinRotationPosition;
    public Vector2 localMaxRotationPosition;

    private float initialShootPointYPos;

    [SerializeField] private float aimDuration;
    [SerializeField] private float aimToShootDelay;
    [SerializeField] private float shootToAimDelay;

    private bool aiming = true;

    private bool inRange;

    private Transform playerTarget;
    private float aimTimer = 0;
    private float shootTimer;



    void Start()
    {




    }

    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }

    public void SetAiming(bool aim)
    {
        if (aim) Invoke("SetInRange", .18f);
        else inRange = false;
    }

    private void SetInRange()
    {
        inRange = true;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.Kill(this);
        aiming = true;

    }
    private void OnEnable()
    {
        smoke.DOFade(0, 0);
    }



    void Update()
    {
        if (aiming && inRange)
        {
            Debug.Log("Aiming");
            aimTimer += Time.deltaTime;
            RotateBasedOnXDistance();
            InterpolatePosition();

            if (aimTimer >= aimDuration)
            {
                aiming = false;
                aimTimer = 0;
                StartCoroutine(ShootRoutine());
            }
        }
    }

    void RotateBasedOnXDistance()
    {
        if (playerTarget == null) return;

        float xDistance = playerTarget.position.x - transform.position.x;

        float t = Mathf.InverseLerp(minXRange, maxXRange, xDistance);
        float targetZRotation = Mathf.Lerp(minZRotation, maxZRotation, t);

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(aimToShootDelay);

        // Calculate the force based on Y distance
        float yDistance = playerTarget.position.y - transform.position.y;
        float force = CalculateLaunchForce(yDistance);

        pool.GetBomb(shootPoint.position, transform.eulerAngles, force, false);
        AudioManager.instance.PlayBombLaunchSound();
        smoke.transform.DOScale(smokeEndScale, spriteChangeTime * smokeSprites.Length).SetEase(Ease.OutSine);
        smoke.transform.DOLocalMoveY(1.3f, spriteChangeTime * smokeSprites.Length).From(.8f).SetEase(Ease.OutSine);
        shootPoint.DOLocalMoveY(.47f, shootPointInitialMoveTime).From(.65f).SetEase(Ease.OutSine)
            .OnComplete(() => shootPoint.DOLocalMoveY(.65f, shootPointEndMoveTime));

        for (int i = 0; i < smokeSprites.Length; i++)
        {
            smoke.sprite = pool.bombLaunchSmoke[i];
            smoke.DOFade(smokeOpacities[i], spriteChangeTime);
            yield return new WaitForSeconds(spriteChangeTime);
        }

        yield return new WaitForSeconds(shootToAimDelay);
        aiming = true;
    }

    float CalculateLaunchForce(float yDistance)
    {
        // Clamp the yDistance between minYDistance and maxYDistance
        float clampedYDistance = Mathf.Clamp(yDistance, minYDistance, maxYDistance);

        // Calculate the interpolation value based on the yDistance
        float t = Mathf.InverseLerp(minYDistance, maxYDistance, clampedYDistance);

        // Interpolate the force between minLaunchForce and maxLaunchForce
        return Mathf.Lerp(minLaunchForce, maxLaunchForce, t);
    }

    void InterpolatePosition()
    {
        float t = Mathf.InverseLerp(minZRotation, maxZRotation, transform.localEulerAngles.z);
        transform.localPosition = Vector2.Lerp(localMinRotationPosition, localMaxRotationPosition, t);
    }
}