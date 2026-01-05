using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions; // UILineRenderer

public class NodeTreeCanvas : MonoBehaviour
{
    [Header("Window")]
    public RectTransform windowRect;
    public RectTransform lineParent;

    [Header("Prefabs")]
    public NodeUI nodePrefab;
    public UILineRenderer linePrefab;

    [Header("Settings")]
    public float nodeLineYOffset;
    public enum NodeType
    {
        Wave,
        Loop,
        Delay,
        Branch
    }

    [Header("Node Type UI")]
    public GameObject nodeTypeDropdown;                 // Your existing dropdown window
    public Transform nodeTypeButtonParent;              // Has VerticalLayoutGroup
    public NodeTypeButton nodeTypeButtonPrefab;

    [Header("Node Types")]
    public List<NodeType> availableNodeTypes = new();
    public List<NodeUI> nodeTypePrefabs = new();



    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private readonly List<NodeUI> nodes = new();
    private readonly List<NodeConnection> connections = new();

    private NodeUI connectionSource;

    // Called by your + button
    public void AddNode()
    {
        nodeTypeDropdown.SetActive(true);
        PopulateNodeTypeButtons();
    }

    private void PopulateNodeTypeButtons()
    {
        // Clear old buttons
        foreach (Transform child in nodeTypeButtonParent)
            Destroy(child.gameObject);

        foreach (var type in availableNodeTypes)
        {
            NodeTypeButton button = Instantiate(
                nodeTypeButtonPrefab,
                nodeTypeButtonParent
            );

            button.Initialize(this, type);
        }
    }

    public void AddNewNodeType(NodeType type)
    {
        nodeTypeDropdown.SetActive(false);

        NodeUI prefab = nodeTypePrefabs[(int)type];

        NodeUI node = Instantiate(prefab, windowRect);
        node.Initialize(this);
        node.SetNodeType(type);

        node.Rect.anchoredPosition = Vector2.zero;
        nodes.Add(node);
    }

    // Called by a node's "connect" button
    public void BeginConnection(NodeUI source)
    {
        connectionSource = source;

        foreach (var node in nodes)
        {
            if (node != source)
                node.SetHighlight(true);
        }
    }

    // Called when clicking a highlighted node
    public void TryCompleteConnection(NodeUI target)
    {
        if (connectionSource == null || target == connectionSource)
            return;

        UILineRenderer line = Instantiate(linePrefab, lineParent);

        NodeConnection connection = new NodeConnection
        {
            from = connectionSource,
            to = target,
            line = line
        };

        connections.Add(connection);

        UpdateLine(connection);

        EndConnectionMode();
    }

    private void EndConnectionMode()
    {
        foreach (var node in nodes)
            node.SetHighlight(false);

        connectionSource = null;
    }

    public void UpdateConnectionsFor(NodeUI node)
    {
        foreach (var c in connections)
        {
            if (c.from == node || c.to == node)
                UpdateLine(c);
        }
    }

    private void UpdateLine(NodeConnection c)
    {
        Vector2 from = c.from.Rect.anchoredPosition + Vector2.up * nodeLineYOffset;
        Vector2 to = c.to.Rect.anchoredPosition + Vector2.up * nodeLineYOffset;

        c.line.Points = new Vector2[]
        {
            from,
            to
        };
    }

    private class NodeConnection
    {
        public NodeUI from;
        public NodeUI to;
        public UILineRenderer line;
    }
}