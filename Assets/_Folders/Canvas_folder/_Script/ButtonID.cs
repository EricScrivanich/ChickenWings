using UnityEngine;

public class ButtonID : MonoBehaviour
{
    public enum ButtonType
    {
        Flip,
        Dash,
        Drop,
        Ammo
    }

    [field: SerializeField] public ButtonType buttonType { get; private set; }

}
