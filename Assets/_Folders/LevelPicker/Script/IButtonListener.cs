using UnityEngine;


public interface IButtonListener
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Press(int index, ButtonTouchByIndex.ButtonType buttonType);
    void GetText(string text, ButtonTouchByIndex.ButtonType buttonType);

}
