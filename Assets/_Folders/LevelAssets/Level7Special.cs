using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Level7Special : MonoBehaviour
{
    [SerializeField] private GameObject sectionTrigger;
    [SerializeField] private Transform playerTran;
    [SerializeField] private PlayerID player;
    [SerializeField] private SpawnIntensityManager spawnMan;

    [SerializeField] private float startSlidePos;
    [SerializeField] private float swipeAmount;
    [SerializeField] private float swipeDuration;

    private Sequence slideSeq;
    private Sequence touchSeq;
    [SerializeField] private Button hiddenEggButton;
    [SerializeField] private CanvasGroup touchGesture;
    [SerializeField] private Image swipeImage;

    [SerializeField] private SignMovement[] signs;
    private bool hasShownButton = false;
    private bool hasSwipedButton = false;

    [SerializeField] private GameObject specialEgg;

    [SerializeField] private BarnAndEggSpawner eggSpawner;

    public bool DoStart = true;

    // Start is called before the first frame update
    void Start()
    {
        swipeImage.gameObject.SetActive(false);

        StartCoroutine(DelayedStart());


    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSecondsRealtime(.3f);

        if (DoStart)
        {
            hiddenEggButton.enabled = false;
            eggSpawner.enabled = false;

            startSlidePos = swipeImage.rectTransform.position.x;

        }
        else
        {
            specialEgg.SetActive(false);
            eggSpawner.enabled = true;
            hiddenEggButton.enabled = true;
            hasShownButton = true;
            hasSwipedButton = true;
            player.ShotgunAmmo += 3;


        }

    }




    private IEnumerator Delay(int sec)
    {
        yield return new WaitForSecondsRealtime(1f);
        signs[sec].gameObject.SetActive(true);

    }

    public void EnableShowEggButton(bool enable)
    {
        hiddenEggButton.enabled = enable;

        if (enable)
        {
            touchGesture.gameObject.SetActive(true);
            touchSeq = DOTween.Sequence();
            touchSeq.AppendInterval(.5f);
            touchSeq.Append(touchGesture.DOFade(1, .3f).From(0));
            touchSeq.Play().SetUpdate(true);
        }

    }

    // private IEnumerator ChangeTime()
    // {
    //     float elapsed = 0;
    //     float duration = .25f;
    //     while (elapsed < duration)
    //     {
    //         elapsed += Time.unscaledDeltaTime;
    //         Time.timeScale = Mathf.Lerp(0, 1, elapsed / duration);
    //         yield return null;
    //     }
    //     spawnMan.GoToNextIntensity(1);
    // }



    private void RetractAfterShowButton(bool non)
    {

        if (!hasShownButton && !non)
        {
            touchGesture.DOFade(0, .3f).SetUpdate(true).OnComplete(() => touchGesture.gameObject.SetActive(false));
            Debug.LogError("ERERERERER");
            hasShownButton = true;
            signs[0].SpecialRetract();
            StartCoroutine(Delay(1));
            swipeImage.gameObject.SetActive(true);
            swipeImage.DOFade(.9f, .25f).SetUpdate(true).OnComplete(SwipeTween);






        }

    }



    private void SwipeTween()
    {
        slideSeq = DOTween.Sequence();
        slideSeq.Append(swipeImage.rectTransform.DOMoveX(startSlidePos - swipeAmount, swipeDuration).From(startSlidePos));
        slideSeq.Play().SetLoops(-1).SetUpdate(true);

    }

    private void RetractAfterSwipe(int i)
    {
        // Debug.LogError("we ssisisisisis");
        if (!hasSwipedButton && i == 1)
        {
            string[] s = new string[1];
            s[0] = "";

            signs[1].SpecialRetract();

            // player.globalEvents.OnSetInputs?.Invoke(true, s, 0, .2f, true, false);
            sectionTrigger.transform.position = playerTran.position;
            sectionTrigger.SetActive(true);
            sectionTrigger.GetComponent<TriggerNextSection>().SpecialEnter();


            hasSwipedButton = true;
            eggSpawner.enabled = true;

            swipeImage.DOFade(0, .25f).SetUpdate(true).OnComplete(() => slideSeq?.Kill());
            // StartCoroutine(ChangeTime());


        }
    }

    private void FinalSign()
    {

    }


    private void OnEnable()
    {
        // player.globalEvents.OnUseChainedAmmo += ShowChainedAmmo;

        EggAmmoDisplay.HideEggButtonEvent += RetractAfterShowButton;

        EggAmmoDisplay.EquipAmmoEvent += RetractAfterSwipe;
    }

    private void OnDisable()
    {
        EggAmmoDisplay.HideEggButtonEvent -= RetractAfterShowButton;
        EggAmmoDisplay.EquipAmmoEvent -= RetractAfterSwipe;

    }

}

// Update is called once per frame



