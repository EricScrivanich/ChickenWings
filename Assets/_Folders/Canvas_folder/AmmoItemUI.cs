using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AmmoItemUI : MonoBehaviour
{
    
    private bool centered = false;
    private Vector3 centerScale = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 sideScale = new Vector3(1, 1, 1);
    public Image ammoImage;      // The image showing the ammo type
    public TextMeshProUGUI ammoAmountText;  // The text showing the current amount of ammo
    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        Debug.Log(gameObject + " pos:" + rect.position.x);
    }

    // This method will dynamically scale the item and show/hide the amount text based on whether it is the center
    public void SetAsCenter(bool isCenter)
    {
        // If this ammo is the center one, hide the amount and increase the size


        int fadeAmount = isCenter ? 0 : 1;

        if (isCenter)
        {
            ammoAmountText.DOFade(0, .25f);
            rect.DOScale(centerScale, .3f);
            rect.DOLocalMoveY(0, .3f);

            centered = true;
        }
        else
        {
            if (centered)
            {
                ammoAmountText.DOFade(1, .25f);
                rect.DOScale(sideScale, .3f);
                rect.DOLocalMoveY(-24, .3f);
                centered = false;
            }


        }


        // ammoAmountText.gameObject.SetActive(!isCenter);

        // Scale the item based on whether it's the center item or a side item
        // float targetScale = isCenter ? centerScale : sideScale;
        // transform.localScale = new Vector3(targetScale, targetScale, 1f);
    }
}