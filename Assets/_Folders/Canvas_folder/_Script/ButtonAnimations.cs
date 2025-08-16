using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimations : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private PlayerID player;

    [Header("Flips")]
    [SerializeField] private Image flipRightImage;

    [Header("Dash")]
    [SerializeField] private Image dashImage;
    [SerializeField] private RectTransform dashRect;
    [SerializeField] private Image dashCooldownIN;
    [SerializeField] private CanvasGroup dashCooldownGroup;

    [Header("Drop")]
    [SerializeField] private Image dropImage;
    private RectTransform dropRect;
    [SerializeField] private Image dropCooldownIN;
    [SerializeField] private CanvasGroup dropCooldownGroup;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
