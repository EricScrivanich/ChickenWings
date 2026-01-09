using UnityEngine;

public interface IBoxSliderListener
{
    /// <summary>
    /// Called when a float-based slider value changes.
    /// </summary>
    void OnBoxSliderValueChanged(int type, float value);

    /// <summary>
    /// Called when a Time slider value changes (ushort).
    /// </summary>
    void OnBoxSliderTimeValueChanged(int type, ushort value);

    /// <summary>
    /// Called when a Byte slider value changes (0-100 percentage).
    /// </summary>
    void OnBoxSliderByteValueChanged(int type, byte value);

    /// <summary>
    /// Called when a BoxTypeChanger value changes.
    /// </summary>
    void OnBoxTypeChanged(int type, int value);
}

/// <summary>
/// Interface for UI elements that can be selected/deselected with only one selected at a time.
/// </summary>
public interface ISelectableUI
{
    /// <summary>
    /// Called when this UI element is selected or deselected.
    /// </summary>
    void SetSelected(bool selected);

    /// <summary>
    /// Returns whether this UI element is currently selected.
    /// </summary>
    bool IsSelected { get; }
}
