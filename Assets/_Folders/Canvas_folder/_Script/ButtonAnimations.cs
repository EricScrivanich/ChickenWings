using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class ButtonAnimations : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private bool isColorPicker = false;
    [SerializeField] private PlayerID player;

    [Header("Flips")]
    [SerializeField] private Image flipRightImage;
    [SerializeField] private Image flipLeftImage;




    private readonly short[] rotations = new short[]
       {
    //     new Vector3(0, 180, 40),
    //     new Vector3(0, 180, -35),
    //     new Vector3(0, 180, 20),
    //     new Vector3(0, 180, -15),
    //     new Vector3(0, 180, 5),
    //     new Vector3(0, 180, -3),
    //    new Vector3(0,180,0)
        -40,
        35,
        -20,
        15,
        -5,
        3,
        0

};



    [Header("Dash")]
    [SerializeField] private Image dashImage;

    [SerializeField] private Image dashCooldownIN;
    [SerializeField] private CanvasGroup dashCooldownGroup;
    private float moveDashX = 30;
    private bool isDashing = false;
    private Vector3 dashPressScale1 = new Vector3(1.5f, .65f, 1f);
    private Vector3 dashPressScale2 = new Vector3(.9f, 1.1f, 1f);
    private Vector3 dashIconSmallScale = new Vector3(.85f, .9f, 1);


    private float dashCooldownDur = .9f;

    [Header("Drop")]
    [SerializeField] private Image dropImage;

    [SerializeField] private Image dropCooldownIN;
    [SerializeField] private CanvasGroup dropCooldownGroup;
    private float dropCooldownDur = 1.3f;

    private Vector3 dropPressScale1 = new Vector3(1.2f, .8f, 1f);
    private Vector3 dropPressScale2 = new Vector3(.85f, 1.1f, 1);
    private Vector3 dropPressScale3 = new Vector3(.9f, .9f, .9f);


    [Header("Sequences")]
    private Sequence flipLeftSeq;
    private Sequence flipRightSeq;
    private Sequence dashSeq;
    private Sequence dropSeq;

    void OnEnable()
    {
        player.UiEvents.OnHandleFlip += DoFlipSeq;
        player.UiEvents.OnDashUI += DashSeq;
        player.UiEvents.OnDropUI += DropSeq;
        player.UiEvents.OnFinishDashAndDropCooldown += FinishCooldowns;
        dashCooldownIN.color = colorSO.disabledButtonColorFull;
        dashCooldownGroup.alpha = 0;
        dropCooldownIN.color = colorSO.disabledButtonColorFull;
        dropCooldownGroup.alpha = 0;

    }
    void OnDisable()
    {
        player.UiEvents.OnHandleFlip -= DoFlipSeq;
        player.UiEvents.OnDashUI -= DashSeq;
        player.UiEvents.OnDropUI -= DropSeq;
        player.UiEvents.OnFinishDashAndDropCooldown -= FinishCooldowns;
    }

    public void RedoCooldownColors()
    {
        dashCooldownIN.color = colorSO.disabledButtonColorFull;
        dropCooldownIN.color = colorSO.disabledButtonColorFull;
    }



    private void DoFlipSeq(bool isRight, bool released)
    {
        if (released)
        {
            ReleaseFlip(isRight);
        }
        else
        {
            PressFlip(isRight);
        }


    }

    private void PressFlip(bool isRight)
    {
        ref Sequence seq = ref isRight ? ref flipRightSeq : ref flipLeftSeq;
        Image img = isRight ? flipRightImage : flipLeftImage;
        Vector3 scale = Vector3.one * 1.2f;
        int flip = isRight ? -1 : 1;
        if (isRight) scale.x *= -1;

        img.color = colorSO.normalButtonColorFull;
        if (seq.isAlive) seq.Stop();

        // store the new sequence back into the same ref
        seq = Sequence.Create()
            .Group(Tween.Rotation(img.rectTransform, Vector3.forward * 45 * flip, 0.6f, Ease.InSine))
            .Group(Tween.Scale(img.rectTransform, scale, 0.35f));

    }

    private void ReleaseFlip(bool isRight)
    {
        ref Sequence seq = ref isRight ? ref flipRightSeq : ref flipLeftSeq;
        RectTransform rect = isRight ? flipRightImage.rectTransform : flipLeftImage.rectTransform;
        Image img = isRight ? flipRightImage : flipLeftImage;
        int flip = isRight ? 1 : -1;
        Vector3 scale = Vector3.one;

        if (isRight) scale.x = -1;

        img.color = colorSO.normalButtonColor;
        if (seq.isAlive) seq.Stop();

        seq = Sequence.Create()
            .Group(Tween.Scale(rect, scale, 0.2f))
            .Group(Tween.Rotation(rect, Vector3.forward * rotations[0] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[1] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[2] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[3] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[4] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[5] * flip, .15f, Ease.OutSine))
            .Chain(Tween.Rotation(rect, Vector3.forward * rotations[6] * flip, .15f, Ease.OutSine));
    }


    private void DashSeq(bool press)
    {
        if (press)
        {
            if (dashSeq.isAlive) dashSeq.Complete();
            isDashing = true;

            dashImage.color = colorSO.highlightButtonColor;
            dashSeq = Sequence.Create()
           .Group(Tween.UIAnchoredPositionX(dashImage.rectTransform, 25, 0.2f, Ease.OutSine))
           .Group(Tween.Scale(dashImage.rectTransform, dashPressScale1, 0.2f, Ease.OutSine))
            .Chain(Tween.UIAnchoredPositionX(dashImage.rectTransform, 35, 0.2f, Ease.OutSine))
           .Group(Tween.Scale(dashImage.rectTransform, dashPressScale2, 0.2f, Ease.OutSine));

        }
        else if (isDashing)
        {
            if (dashSeq.isAlive) dashSeq.Complete();
            isDashing = false;
            dashCooldownIN.fillAmount = 1;
            // dashCooldownGroup.gameObject.SetActive(true);

            dashSeq = Sequence.Create()
            .Group(
                Tween.Custom(
                    target: dashCooldownGroup,
                    0,
                    1f,
                    .2f,
                    (target, val) => target.alpha = val
                )
            )
           .Group(Tween.UIAnchoredPositionX(dashImage.rectTransform, 0, 0.35f, Ease.OutSine))
           .Group(Tween.Scale(dashImage.rectTransform, dashIconSmallScale, 0.35f))
           .Group(Tween.Color(dashImage, colorSO.disabledButtonColor, 0.35f))

           .Group(Tween.UIFillAmount(dashCooldownIN, 0, dashCooldownDur))

           .Chain(Tween.Scale(dashImage.rectTransform, 1, 0.15f))
           .Group(Tween.Color(dashImage, colorSO.normalButtonColor, 0.15f))
           .Group(
                Tween.Custom(
                    target: dashCooldownGroup,
                    1f,
                    0f,
                    .15f,
                    (target, val) => target.alpha = val
                )
            );
            //    .ChainCallback(() =>
            //    {
            //        dashCooldownGroup.gameObject.SetActive(false);
            //    });


        }
    }

    private void DropSeq()
    {
        if (dropSeq.isAlive) dropSeq.Complete();

        dropCooldownIN.fillAmount = 1;


        dropImage.color = colorSO.highlightButtonColor;
        dropSeq = Sequence.Create()
       .Group(Tween.UIAnchoredPositionY(dropImage.rectTransform, -30, 0.2f, Ease.OutSine))
       .Group(Tween.Scale(dropImage.rectTransform, dropPressScale1, 0.3f, Ease.OutSine))

    .Chain(Tween.UIAnchoredPositionY(dropImage.rectTransform, 0, 0.3f, Ease.OutSine))
       .Group(Tween.Scale(dropImage.rectTransform, dropPressScale2, 0.2f, Ease.InSine))
       .Group(Tween.Color(dropImage, colorSO.disabledButtonColor, 0.2f))
       .Group(
                Tween.Custom(
                    target: dropCooldownGroup,
                    0,
                    1f,
                    .2f,
                    (target, val) => target.alpha = val
                )
            )
        .Group(Tween.UIFillAmount(dropCooldownIN, 0, dropCooldownDur - .3f))
        .Insert(.6f,
        Tween.Scale(dropImage.rectTransform, dropPressScale3, 0.25f, Ease.InOutSine)

        )
        .Chain(Tween.Scale(dropImage.rectTransform, 1, 0.15f))
        .Group(Tween.Color(dropImage, colorSO.normalButtonColor, 0.15f))
         .Group(
                Tween.Custom(
                    target: dropCooldownGroup,
                    1f,
                    0f,
                    .15f,
                    (target, val) => target.alpha = val
                )
            );



    }

    private void FinishCooldowns(bool finishDash, bool finishDrop)
    {
        if (finishDash)
        {
            if (isDashing)
            {
                DashSeq(false);
                dashSeq.Complete();

            }
            else
            {
                if (dashSeq.isAlive) dashSeq.Complete();
            }
        }
        if (finishDrop)
        {
            if (dropSeq.isAlive) dropSeq.Complete();
        }

        if (isColorPicker)
        {
            RedoCooldownColors();
        }
    }


}
