using UnityEngine;

public interface IBoxSliderListener
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnBoxSliderValueChanged(int type, float valueSliderHorizontal);
    void OnBoxTypeChanged(int type, int value);


    // Update is called once per frame
}
