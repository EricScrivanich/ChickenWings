using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class EnemyHeart : MonoBehaviour
{
    [SerializeField] private Sprite[] heartSprites;
    [SerializeField] private TextMeshPro heartText;
    [SerializeField] private SpriteRenderer heartSpriteRenderer;
    private float timer = 0;
    private float changeInterval = .1f;
    private int currentSpriteIndex = 0;
    [SerializeField] private int totalLives = 5;
    private SpriteRenderer sr;
    private Coroutine loseLifeCoroutine;
    private Sequence heartSeq;
    private float startScale;
    private bool skipStart = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (skipStart) return;
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale.x;

        loseLifeCoroutine = StartCoroutine(HideAfterDelay(true));


    }
    public void SetHealthForRecording(int lives)
    {
        skipStart = true;
        Debug.Log("Set Health For Recording: " + lives);
        if (lives > 1)
        {
            heartText.text = lives.ToString();
            if (!this.gameObject.activeInHierarchy)
                this.gameObject.SetActive(true);

        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    public void SetHearts(int lives)
    {
        if (skipStart) return;
        if (lives <= 1)
        {
            gameObject.SetActive(false);
            return;
        }
        else gameObject.SetActive(true);
        totalLives = lives;
        heartText.text = totalLives.ToString();

    }
    public void Damage(int lives)
    {
        if (lives < 0) return;
        if (lives >= totalLives) return;
        if (loseLifeCoroutine != null)
        {
            StopCoroutine(loseLifeCoroutine);
            if (heartSeq != null && heartSeq.IsPlaying())
            {
                heartSeq.Complete();
                heartSpriteRenderer.enabled = false;
            }
        }



        loseLifeCoroutine = StartCoroutine(DoLoseLife(lives));
    }
    private IEnumerator HideAfterDelay(bool intitial = false)
    {
        if (intitial) yield return new WaitForSeconds(1.5f);

        yield return new WaitForSeconds(1.5f);
        transform.DOScale(0, .5f).SetEase(Ease.InSine);
    }

    private IEnumerator DoLoseLife(int newLives)
    {
        // Play lose life animation
        // make it so the coroutine waits until tween is done before continuing
        if (newLives <= 0)
        {
            gameObject.SetActive(false);
            yield break;
        }
        heartSpriteRenderer.sprite = heartSprites[0];
        heartSpriteRenderer.color = new Color(1, 1, 1, 0);
        heartSpriteRenderer.enabled = true;

        heartSeq = DOTween.Sequence();

        float s = transform.localScale.x;
        if (s < startScale && s > 0)
        {
            DOTween.Kill(transform);
            transform.DOScale(startScale, .2f);
            heartSeq.Append(heartSpriteRenderer.DOFade(1, .17f));
        }
        else if (s <= 0)
        {
            heartSpriteRenderer.color = Color.white;
            transform.DOScale(startScale, .2f);
        }
        else if (s >= startScale)
        {
            heartSeq.Append(heartSpriteRenderer.DOFade(1, .17f));
        }





        // heartSeq.Append(heartSpriteRenderer.transform.DOScale(1.1f, .32f).SetEase(Ease.OutSine));

        // heartSeq.Join(heartSpriteRenderer.DOFade(.5f, .32f).SetEase(Ease.OutSine));
        heartSeq.Play();

        yield return heartSeq.WaitForCompletion();

        heartText.text = newLives.ToString();
        WaitForSeconds wait = new WaitForSeconds(.06f);

        for (int i = 1; i < heartSprites.Length; i++)
        {
            heartSpriteRenderer.sprite = heartSprites[i];
            yield return wait;
        }
        heartSpriteRenderer.enabled = false;
        loseLifeCoroutine = StartCoroutine(HideAfterDelay());


    }
}
