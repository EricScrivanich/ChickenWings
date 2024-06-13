using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePositioning : MonoBehaviour
{
    [SerializeField] private RectTransform ropeObject;
    private RectTransform pos;
    private void Awake() {
        pos = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    private void OnEnable() {
        ropeObject.anchoredPosition = pos.anchoredPosition;
    }
}
