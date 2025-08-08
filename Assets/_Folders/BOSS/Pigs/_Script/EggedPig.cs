using UnityEngine;
using DG.Tweening;

using System.Collections;

public class EggedPig : MonoBehaviour, IEggable
{
    [SerializeField] private GameObject eggedYolkPrefab;
    [SerializeField] private Vector2 eggedYolkOffset;
    [SerializeField] private Sprite secondEggedSprite;
    [SerializeField] private float yolkStartScale;
    [SerializeField] private float yolkEndScale;
    [SerializeField] private float yolkStartY;
    [SerializeField] private float yolkEndY;
    [SerializeField] private float yolkDripDuration;
    private SpriteRenderer yolkSR;
    public void OnEgged()
    {
        var e = Instantiate(eggedYolkPrefab, (Vector2)transform.position + eggedYolkOffset, Quaternion.identity, transform);
        StartCoroutine(Egged());

    }

    private IEnumerator Egged()
    {
        yolkSR.transform.DOScale(yolkEndScale, yolkDripDuration).From(yolkStartScale).SetEase(Ease.OutSine);
        yolkSR.transform.DOLocalMoveY(yolkEndY, yolkDripDuration).From(yolkStartY).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(yolkDripDuration * .7f);
        yolkSR.sprite = secondEggedSprite;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
