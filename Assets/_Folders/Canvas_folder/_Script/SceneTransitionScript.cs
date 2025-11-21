using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SceneTransitionScript : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SceneManagerScript.instance.OnFadeTranstion += Fade;
    }
    void OnDestroy()
    {
        SceneManagerScript.instance.OnFadeTranstion -= Fade;
    }

    private void Fade(bool fadeIn, float duration, string scene)
    {
        var img = GetComponent<Image>();
        var mat = img.material;
        if (fadeIn)
        {
            mat.SetFloat("_FadeAmount", 1);
            gameObject.SetActive(true);
            // make it so this gameobject is last in parent hierarchy to be on top of other ui
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            mat.DOFloat(-.1f, "_FadeAmount", duration).SetEase(Ease.InOutSine).SetUpdate(true);

            img.DOFade(.8f, duration * .4f).SetEase(Ease.InOutSine).SetUpdate(true);

        }
        else
        {
            mat.SetFloat("_FadeAmount", -.1f);
            img.DOFade(0, duration).From(.8f).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() =>
            {

                gameObject.SetActive(false);
            });
        }

    }


}
