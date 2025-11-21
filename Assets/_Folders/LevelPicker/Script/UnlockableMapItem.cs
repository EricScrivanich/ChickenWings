using UnityEngine;
using DG.Tweening;

public class UnlockableMapItem : MonoBehaviour
{
    [field: SerializeField] public short mapItemID { get; private set; }
    [SerializeField] private short worldNumber;
    [SerializeField] private short subID;

    [SerializeField] private UIButton button;

    public enum UnlockableItemType
    {
        Steroid,
        Chest,
        Other,

    }

    [SerializeField] private UnlockableItemType itemType;

    [SerializeField] private float unlockedScale = 1.1f;
    [SerializeField] private float dur = 1.1f;
    [SerializeField] private SpriteRenderer blurSprite;
    [SerializeField] private SpriteRenderer shadowSprite;

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
        DoUnlockedAnimation(targetPos);



    }
    public void Initializeitem(LevelPickerManager l, bool isUnlocked)
    {
        levelPickerManager = l;
        if (isUnlocked)
        {
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
