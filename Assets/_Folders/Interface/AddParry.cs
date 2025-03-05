using UnityEngine;

public class AddParry : MonoBehaviour
{
    [SerializeField] private PlayerID player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other) {

        IParryable parryableAttack = other.gameObject.GetComponent<IParryable>();

        if (parryableAttack != null)
        {
            parryableAttack.Parry();
            player.events.OnSuccesfulParry?.Invoke();
        }
    }
}
