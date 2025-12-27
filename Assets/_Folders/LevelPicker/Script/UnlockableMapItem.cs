using UnityEngine;
using DG.Tweening;

public class UnlockableMapItem : MonoBehaviour
{
    [field: SerializeField] public short mapItemID { get; private set; }
    [SerializeField] private short worldNumber;
    [SerializeField] private short subID;

    [field: SerializeField]
    public bool ignoreNextLevelCheck { get; private set; } = true;

    [SerializeField] private UIButton button;

    public enum UnlockableItemType
    {
        Steroid,
        Chest,
        Other,

        Cage

    }

    [SerializeField] private UnlockableItemType itemType;

    [SerializeField] private float unlockedScale = 1.1f;
    [SerializeField] private float dur = 1.1f;
    [SerializeField] private SpriteRenderer blurSprite;
    [SerializeField] private SpriteRenderer shadowSprite;

    [SerializeField] private GameObject unlockedGameObject;
    [SerializeField] private Vector2 unlockedGameObjectOffset;
    [SerializeField] private float unlockedGameObjectScale;

    private Sequence unlockSeq;
    private Sequence rotateSeq;





    public void ResetUnlockAnimation()
    {
        unlockSeq.Restart();
        rotateSeq.Restart();
    }
    [SerializeField] private float totalRotateDur;
    [SerializeField] private float maxRot;
    [SerializeField] private int amountToReachMax;
    [SerializeField] private int amountToReturn;
    private LevelPickerManager levelPickerManager;

    public void UnlockItem(Vector2 targetPos)
    {
        Debug.Log(" Unlockable Item Unlcoked ");

        switch (itemType)
        {
            case UnlockableItemType.Steroid:
                DoUnlockedAnimation(targetPos);
                break;
            case UnlockableItemType.Chest:

                break;
            case UnlockableItemType.Other:

                break;
            case UnlockableItemType.Cage:
                var o = Instantiate(unlockedGameObject, (Vector2)transform.position + unlockedGameObjectOffset, Quaternion.identity, transform.parent);
                o.transform.localScale = Vector3.one * unlockedGameObjectScale;
                o.GetComponent<ChicTween>().DoBarnTween(true);
                gameObject.SetActive(false);
                FinishUnlock(levelPickerManager);

                break;
        }




    }
    public void Initializeitem(LevelPickerManager l, bool isUnlocked)
    {
        Debug.Log("Initialize Unlockable Item ID: " + mapItemID + "-" + subID + " | Unlocked: " + isUnlocked);
        levelPickerManager = l;
        if (isUnlocked)
        {


            switch (itemType)
            {
                case UnlockableItemType.Steroid:

                    break;
                case UnlockableItemType.Chest:

                    break;
                case UnlockableItemType.Other:

                    break;
                case UnlockableItemType.Cage:
                    var o = Instantiate(unlockedGameObject, (Vector2)transform.position + unlockedGameObjectOffset, Quaternion.identity, transform.parent);
                    o.transform.localScale = Vector3.one * unlockedGameObjectScale;
                    o.GetComponent<ChicTween>().DoBarnTween(false);


                    break;
            }

            gameObject.SetActive(false);

        }

    }


    public void DoUnlockedAnimation(Vector2 targetPos)
    {
        // Kill if already exists
        unlockSeq?.Kill();
        rotateSeq?.Kill();

        // -----------------------
        // UNLOCK ANIMATION
        // -----------------------

        unlockSeq = DOTween.Sequence().SetAutoKill(false);

        if (shadowSprite != null)
        {
            unlockSeq.Append(shadowSprite.DOFade(0, .2f));
        }

        unlockSeq.Append(transform.DOMove(targetPos, dur));
        unlockSeq.Join(transform.DOScale(transform.localScale * unlockedScale, dur));
        unlockSeq.Join(blurSprite.DOFade(1, dur));


        unlockSeq.Play().SetUpdate(true).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            FinishUnlock(levelPickerManager);
        });

        // -----------------------
        // ROTATION SEQUENCE
        // -----------------------
        BuildRotationSequence();
    }

    private void FinishUnlock(LevelPickerManager levelPickerManager)
    {
        switch (itemType)
        {
            case UnlockableItemType.Steroid:
                LevelDataConverter.instance.SaveSteroidData(null, (int)subID, (int)subID);
                button.OnPress(false);
                transform.DOScale(Vector3.zero, .5f).SetUpdate(true).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });

                break;
            case UnlockableItemType.Chest:
                Debug.Log("Unlocked Chest Item ID: " + mapItemID + "-" + subID);
                break;
            case UnlockableItemType.Other:
                Debug.Log("Unlocked Other Item ID: " + mapItemID + "-" + subID);
                break;
        }

        LevelDataConverter.instance.ResetNextUnlockedIndex(worldNumber);

        if (ignoreNextLevelCheck)
            levelPickerManager.DoNextLevelAfterUnlock();
    }


    private void BuildRotationSequence()
    {
        rotateSeq = DOTween.Sequence().SetAutoKill(false);

        int totalSteps = amountToReachMax + amountToReturn;
        float stepDur = totalRotateDur / totalSteps;
        int flip = 1;   // duration of each rotation step
        rotateSeq.AppendInterval(0.25f);

        // ---- STEP 1: Lerp up to max rotation in increments ----
        for (int i = 1; i <= amountToReachMax; i++)
        {
            float targetRot = Mathf.Lerp(0f, maxRot, (float)i / amountToReachMax) * flip;
            rotateSeq.Append(
                transform
                    .DORotate(new Vector3(0, 0, targetRot), stepDur)
                    .SetEase(Ease.InOutSine)
            );
            flip *= -1;

            if (i == amountToReachMax)
            {
                rotateSeq.AppendCallback(() =>
                {
                    AudioManager.instance.PlayBucketSuccessSound();

                });
            }
        }

        // ---- STEP 2: Lerp back down to 0 in increments ----
        for (int i = 1; i <= amountToReturn; i++)
        {
            float targetRot = Mathf.Lerp(maxRot, 0f, (float)i / amountToReturn) * flip;
            rotateSeq.Append(
                transform
                    .DORotate(new Vector3(0, 0, targetRot), stepDur)
                    .SetEase(Ease.InOutSine)
            );
            flip *= -1;
        }


        rotateSeq.Play().SetUpdate(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
