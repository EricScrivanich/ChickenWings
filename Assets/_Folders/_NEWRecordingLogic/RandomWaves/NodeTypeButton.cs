using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeTypeButton : MonoBehaviour
{
    private NodeTreeCanvas.NodeType nodeType;
    private NodeTreeCanvas canvas;

    public void Initialize(NodeTreeCanvas owner, NodeTreeCanvas.NodeType type)
    {
        canvas = owner;
        nodeType = type;

        // Set label text
        var text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = type.ToString().Replace("_", " ");
    }

    // Hook this to the Button onClick
    public void OnPressed()
    {
        canvas.AddNewNodeType(nodeType);
    }
}