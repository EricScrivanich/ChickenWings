using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class AmmoScrollManager : MonoBehaviour
{
    [SerializeField] private int totalEggs;
    
    private bool canScroll = true;
    public RectTransform content;           // The container holding the ammo items
    public List<AmmoItemUI> ammoItems;      // List of original ammo items (images and text)
    public Button leftArrowButton;          // Left arrow button
    public Button rightArrowButton;         // Right arrow button

    public int currentIndex = 0;            // The current index of the center item
    private int totalAmmoTypes;             // Total number of ammo types

    [Header("Settings")]
    public float centerItemScale = 1.5f;    // Scale of the center item
    public float sideItemScale = 1.0f;      // Scale of the side items
    public int itemSpacing;                 // Distance to move content for each item
    public float moveDuration = 0.5f;       // Duration of the movement animation

    private void Start()
    {
        totalAmmoTypes = ammoItems.Count;

        // Add listeners for the arrow buttons
        leftArrowButton.onClick.AddListener(ScrollLeft);
        rightArrowButton.onClick.AddListener(ScrollRight);

        // Initially set the first element as the center
        UpdateCenterAmmo(currentIndex);
    }

    public void ScrollLeft()
    {
        if (!canScroll) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            // If we go past the first item, move the last item to the start
            MoveLastItemToStart();
            currentIndex = 0;  // Reset to the new first item
        }

        ScrollToIndex(1);  // Move right in local coordinates (visual left)
    }

    public void ScrollRight()
    {
        if (!canScroll) return;

        currentIndex++;
        if (currentIndex >= totalAmmoTypes)
        {
            // If we go past the last item, move the first item to the end
            MoveFirstItemToEnd();
            currentIndex = totalAmmoTypes - 1;  // Reset to the new last item
        }

        ScrollToIndex(-1);  // Move left in local coordinates (visual right)
    }

    private void ScrollToIndex(int direction)
    {
        canScroll = false;

        // Calculate the new position of the content
        float movedAmount = direction * itemSpacing;

        // Smoothly move the content using DOTween
        content.DOLocalMoveX(content.localPosition.x + movedAmount, moveDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // Update the center item (scale it up, hide its text, etc.)
            UpdateCenterAmmo(currentIndex);
            canScroll = true;
        });
    }

    private void UpdateCenterAmmo(int currentInd)
    {
        for (int i = 0; i < ammoItems.Count; i++)
        {
            bool isCenter = (i == currentInd);
            ammoItems[i].SetAsCenter(isCenter);
        }
    }

    // Moves the last item in the list to the start (visually on the left)
    private void MoveLastItemToStart()
    {
        RectTransform lastItem = ammoItems[totalAmmoTypes - 1].GetComponent<RectTransform>();

        // Move the last item to the start of the content
        lastItem.SetAsFirstSibling();

        // Reposition the content to reflect the change without jumping
        content.localPosition += new Vector3(-itemSpacing, 0, 0);

        // Update the list order
        ammoItems.Insert(0, ammoItems[totalAmmoTypes - 1]);
        ammoItems.RemoveAt(totalAmmoTypes);
    }

    // Moves the first item in the list to the end (visually on the right)
    private void MoveFirstItemToEnd()
    {
        RectTransform firstItem = ammoItems[0].GetComponent<RectTransform>();

        // Move the first item to the end of the content
        firstItem.SetAsLastSibling();

        // Reposition the content to reflect the change without jumping
        content.localPosition += new Vector3(itemSpacing, 0, 0);

        // Update the list order
        ammoItems.Add(ammoItems[0]);
        ammoItems.RemoveAt(0);
    }
}