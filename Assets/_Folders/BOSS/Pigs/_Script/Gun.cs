using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    private SpriteRenderer smokeSprite;
    [SerializeField] private ExplosivesPool pool;

    [SerializeField] private AnimationDataSO animData;

    [SerializeField] private Vector2 kickRange;
    [SerializeField] private float angleAdjustment = 70;
    [SerializeField] private float bulletSpeed;
    private WaitForSeconds spriteSwitchDelay;
    


    private void Start()
    {
        spriteSwitchDelay = new WaitForSeconds(animData.constantSwitchTime);
        smokeSprite = bulletSpawnPoint.GetComponent<SpriteRenderer>();
        smokeSprite.enabled = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Fire(bool flipped)
    {
        AudioManager.instance.PlayUziGunShot();
        StartCoroutine(SmokeAnimation());
        int f = flipped ? -1 : 1;
        pool.GetBullet(bulletSpawnPoint.position, transform.eulerAngles.z, bulletSpeed, f);

        if (kickRange != Vector2.zero)
        {

            transform.localEulerAngles = new Vector3(0, 0, Random.Range(kickRange.x, kickRange.y) + angleAdjustment);
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
}
