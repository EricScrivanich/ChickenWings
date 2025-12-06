using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour
{
    [SerializeField] private ushort gunType;
    [SerializeField] private Transform bulletSpawnPoint;
    private SpriteRenderer smokeSprite;
    [SerializeField] private SpriteRenderer gunSprite;
    [SerializeField] private ExplosivesPool pool;
    [SerializeField] private QPool bulletPool;
    // [SerializeField] private QPool bulletPool;
    [SerializeField] private float bulletScale;

    private enum RecoilType
    {
        None,
        MoveY,
        MoveX,
        Rotate,
        RotateAndMoveY
    }
    [SerializeField] private RecoilType recoilType;

    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private AnimationDataSO gunAnimData;

    [SerializeField] private float rotationTarget;
    [SerializeField] private float returnRotationTarget;
    [SerializeField] private float yTarget;
    [SerializeField] private float recoilInDuration;
    [SerializeField] private float recoilOutDuration;
    private float startY;

    private Sequence recoilSeq;






    [SerializeField] private Vector2 kickRange;
    [SerializeField] private float angleAdjustment = 70;
    [SerializeField] private float shootingAngleAdjustment;
    [SerializeField] private float bulletSpeed;
    private WaitForSeconds spriteSwitchDelay;
    private WaitForSeconds spriteGunSwitchDelay;
    [SerializeField] private bool useCurrentRotation = false;

    [SerializeField] private Transform moveRecoil;
    private bool doGunSprite;



    private void Start()
    {
        spriteSwitchDelay = new WaitForSeconds(animData.constantSwitchTime);
        smokeSprite = bulletSpawnPoint.GetComponent<SpriteRenderer>();
        smokeSprite.enabled = false;
        startY = moveRecoil.localPosition.y;
        if (gunAnimData != null)
        {
            doGunSprite = true;
            spriteGunSwitchDelay = new WaitForSeconds(gunAnimData.constantSwitchTime);
        }
        else doGunSprite = false;


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Fire(bool flipped)
    {
        AudioManager.instance.PlayUziGunShot(gunType);
        StartCoroutine(SmokeAnimation());
        if (doGunSprite)
        {
            StartCoroutine(GunAnimation());
        }
        int f = flipped ? -1 : 1;
        // pool.GetBullet(bulletSpawnPoint.position, moveRecoil.eulerAngles.z + shootingAngleAdjustment, bulletSpeed, f);
        bulletPool.SpawnWithVelocityAndRotationAndScale(bulletSpawnPoint.position, bulletSpeed * f, moveRecoil.eulerAngles.z + shootingAngleAdjustment, bulletScale, 1);
        // bulletPool.SpawnWithRotationAndSpeed(ulletSpawnPoint.position,moveRecoil,bulletSpeed)
        // bulletPool.SpawnWithVelocityAndRotationAndScale(bulletSpawnPoint.position,)

        if (kickRange != Vector2.zero)
        {
            if (useCurrentRotation)
            {

                transform.localEulerAngles = new Vector3(0, 0, Random.Range(kickRange.x, kickRange.y) + transform.localEulerAngles.z);
            }
            else

                transform.localEulerAngles = new Vector3(0, 0, Random.Range(kickRange.x, kickRange.y) + angleAdjustment);
        }
        Recoil();

        // if (yTarget)

        //     if (rotationTarget != 0)
        //     {

        //     }
    }

    private void Recoil()
    {
        if (recoilType == RecoilType.None)
        {
            return;
        }
        if (recoilSeq != null)
        {
            recoilSeq.Kill();
        }
        recoilSeq = DOTween.Sequence();
        rotationTarget *= -1;
        returnRotationTarget *= -1;
        switch (recoilType)
        {

            case RecoilType.MoveY:
                recoilSeq.Append(moveRecoil.DOLocalMoveY(yTarget, recoilInDuration).SetEase(Ease.OutSine));
                recoilSeq.Append(moveRecoil.DOLocalMoveY(startY, recoilOutDuration).SetEase(Ease.OutSine));
                break;
            case RecoilType.MoveX:
                break;
            case RecoilType.Rotate:
                recoilSeq.Append(moveRecoil.DOLocalRotate(new Vector3(0, 0, rotationTarget + angleAdjustment), recoilInDuration).SetEase(Ease.OutSine));
                recoilSeq.Append(moveRecoil.DOLocalRotate(new Vector3(0, 0, returnRotationTarget + angleAdjustment), recoilOutDuration).SetEase(Ease.OutSine));
                break;
            case RecoilType.RotateAndMoveY:
                recoilSeq.Append(moveRecoil.DOLocalRotate(new Vector3(0, 0, rotationTarget + angleAdjustment), recoilInDuration).SetEase(Ease.OutSine));
                recoilSeq.Join(moveRecoil.DOLocalMoveY(yTarget, recoilInDuration).SetEase(Ease.OutSine));
                recoilSeq.Append(moveRecoil.DOLocalRotate(new Vector3(0, 0, returnRotationTarget + angleAdjustment), recoilOutDuration).SetEase(Ease.OutSine));
                recoilSeq.Join(moveRecoil.DOLocalMoveY(startY, recoilOutDuration).SetEase(Ease.OutSine));
                break;
        }


        recoilSeq.Play();

    }

    private void OnDisable()
    {
        if (recoilSeq != null)
        {
            recoilSeq.Kill();
        }
    }

    private IEnumerator SmokeAnimation()
    {
        smokeSprite.enabled = true;
        for (int i = 0; i < animData.sprites.Length; i++)
        {
            smokeSprite.sprite = animData.sprites[i];
            yield return spriteSwitchDelay;
        }
        smokeSprite.enabled = false;



    }

    private IEnumerator GunAnimation()
    {

        for (int i = 0; i < gunAnimData.sprites.Length; i++)
        {
            yield return spriteGunSwitchDelay;
            gunSprite.sprite = gunAnimData.sprites[i];

        }




    }


}
