using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTriggerTester : MonoBehaviour
{
    private Animator animator;

    [Header("Animation Triggers")]
    public bool TurnLeft;
    public bool TurnRight;
    public bool Unsheath;
    public bool Idle;
    public bool SwingDownLeft;
    public bool SwingDownRight;
    public bool Smoke;

    // private void OnValidate()
    // {
    //     // Ensure the Animator component is assigned
    //     if (animator == null)
    //     {
    //         animator = GetComponent<Animator>();
    //     }

    //     // Check each bool and set the corresponding trigger if true
    //     if (TurnLeft)
    //     {
    //         SetTrigger("TurnLeft", ref TurnLeft);
    //     }
    //     if (TurnRight)
    //     {
    //         SetTrigger("TurnRight", ref TurnRight);
    //     }
    //     if (Unsheath)
    //     {
    //         SetTrigger("Unsheath", ref Unsheath);
    //     }
    //     if (Idle)
    //     {
    //         SetTrigger("Idle", ref Idle);
    //     }
    //     if (SwingDownLeft)
    //     {
    //         SetTrigger("SwingDownLeft", ref SwingDownLeft);
    //     }
    //     if (SwingDownRight)
    //     {
    //         SetTrigger("SwingDownRight", ref SwingDownRight);
    //     }

    //     if (Smoke)
    //     {
    //         SetTrigger("Smoke", ref Smoke);
    //     }
    // }
    [SerializeField] private string triggerName;
    void Start()
    {
        GetComponent<Animator>().SetTrigger(triggerName);
    }

    private void SetTrigger(string triggerName, ref bool triggerBool)
    {
        if (animator != null && triggerBool)
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"Triggered: {triggerName}");
            triggerBool = false; // Reset the bool after setting the trigger
        }
    }
}