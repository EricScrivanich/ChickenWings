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
    public RectTransform parameterWindow;


    [Header("Buttons")]
    public Button openParameterWindowButton;
    public GameObject lineConnectionDeleteButton;

    [Header("Prefabs")]
    public NodeUI nodePrefab;
    public UILineRenderer linePrefab;
    public LineConnectionPoint lineConnectionPointPrefab;

    [Header("Parameter UI Prefabs")]
    public GameObject BoxSliderPrefab;
    public GameObject BoxTypeChangerPrefab;


    public enum NodeType
    {
        Wave,
        Loop,
        Delay,
        Branch,
        Start,
        End
    }

    public enum ConnectionPointType
    {
        Entrance,
        Exit,
        LoopBack  // Only used by Loop nodes
    }

    [Header("Node Type UI")]
    public GameObject nodeTypeDropdown;                 // Your existing dropdown window
    public Transform nodeTypeButtonParent;              // Has VerticalLayoutGroup
    public NodeTypeButton nodeTypeButtonPrefab;

    [Header("Node Types")]
    public List<NodeType> availableNodeTypes = new();
    public List<NodeUI> nodeTypePrefabs = new();

    public NodeUI startNode;
    public NodeUI endNode;



    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color nodeUnselectedColor = Color.white;
    public Color nodeSelectedColor = Color.cyan;
    public Color nodeDraggingColor = Color.gray;
    public Color existingConnectionHighlightColor = Color.green;

    [Header("Connection Colors")]
    [Tooltip("Colors for branch exit connections. Index determines color.")]
    public List<Color> connectionColors = new()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    private readonly List<NodeUI> nodes = new();
    private readonly List<NodeConnection> connections = new();

    // Track nodes that already have connections during connection mode
    private readonly List<NodeUI> existingConnectionNodes = new();

    private NodeUI connectionSource;
    private ConnectionPointType connectionPointType;
    private NodeUI selectedNode;
    private bool showingAddNodeDropdown = false;

    // Node ID management
    private ushort nextNodeId = 0;

    // Parameter window management
    private NodeUI currentParameterNode;
    public NodeUI CurrentParameterNode => currentParameterNode;

    // LineConnectionPoint selection management
    private LineConnectionPoint selectedLineConnectionPoint;
    public LineConnectionPoint SelectedLineConnectionPoint => selectedLineConnectionPoint;

    // ISelectableUI selection management (for BoxSlider, BoxTypeChanger, etc.)
    private ISelectableUI selectedSelectableUI;
    public static NodeTreeCanvas Instance { get; private set; }



    public void SelectNode(NodeUI node)
    {
        if (selectedNode == node)
            return;

        // Deselect any selected connection point when selecting a node
        DeselectCurrentConnectionPoint();

        if (selectedNode != null)
            selectedNode.SetSelected(false);

        selectedNode = node;

        if (selectedNode != null)
        {
            selectedNode.SetSelected(true);

            // If parameter window is open, show parameters for the new node
            if (parameterWindow.gameObject.activeSelf)
            {
                ShowParametersForNode(node);
            }
        }
    }

    public void SelectConnectionPoint(LineConnectionPoint connectionPoint)
    {
        if (selectedLineConnectionPoint == connectionPoint)
            return;

        // Deselect any selected node when selecting a connection point
        DeselectCurrentNode();

        if (selectedLineConnectionPoint != null)
            selectedLineConnectionPoint.SetSelected(false);

        selectedLineConnectionPoint = connectionPoint;

        if (selectedLineConnectionPoint != null)
        {
            selectedLineConnectionPoint.SetSelected(true);
            if (lineConnectionDeleteButton != null)
                lineConnectionDeleteButton.SetActive(true);
        }
    }

    public void DeselectCurrentConnectionPoint()
    {
        if (selectedLineConnectionPoint != null)
        {
            selectedLineConnectionPoint.SetSelected(false);
            selectedLineConnectionPoint = null;
        }

        if (lineConnectionDeleteButton != null)
            lineConnectionDeleteButton.SetActive(false);
    }

    /// <summary>
    /// Called by LineConnectionDeleteButton to delete the selected connection.
    /// </summary>
    public void DeleteSelectedConnectionPoint()
    {
        if (selectedLineConnectionPoint != null)
        {
            DeleteConnectionPoint(selectedLineConnectionPoint);
            selectedLineConnectionPoint = null;

            if (lineConnectionDeleteButton != null)
                lineConnectionDeleteButton.SetActive(false);
        }
    }

    public void DeselectCurrentNode()
    {
        if (selectedNode != null)
        {
            selectedNode.SetSelected(false);
            selectedNode = null;
        }
    }

    /// <summary>
    /// Selects an ISelectableUI element, deselecting any previously selected one.
    /// </summary>
    public void SelectSelectableUI(ISelectableUI selectable)
    {
        if (selectedSelectableUI == selectable)
            return;

        if (selectedSelectableUI != null)
            selectedSelectableUI.SetSelected(false);

        selectedSelectableUI = selectable;

        if (selectedSelectableUI != null)
            selectedSelectableUI.SetSelected(true);
    }

    /// <summary>
    /// Deselects the currently selected ISelectableUI element.
    /// </summary>
    public void DeselectCurrentSelectableUI()
    {
        if (selectedSelectableUI != null)
        {
            selectedSelectableUI.SetSelected(false);
            selectedSelectableUI = null;
        }
    }

    // Called by your + button
    public void AddNode()
    {
        if (showingAddNodeDropdown)
        {
            nodeTypeDropdown.SetActive(false);
            showingAddNodeDropdown = false;
            return;

        }
        showingAddNodeDropdown = true;
        nodeTypeDropdown.SetActive(true);
        PopulateNodeTypeButtons();
    }
    void Awake()
    {
        Instance = this;

        // Initialize start node
        startNode.Initialize(this);
        startNode.SetNodeType(NodeType.Start);
        nodes.Add(startNode);

        // Initialize end node
        if (endNode != null)
        {
            endNode.Initialize(this);
            endNode.SetNodeType(NodeType.End);
            nodes.Add(endNode);
        }
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
        showingAddNodeDropdown = false;

        NodeUI prefab = nodeTypePrefabs[(int)type];

        NodeUI node = Instantiate(prefab, windowRect);
        node.Initialize(this);
        node.SetNodeType(type);

        node.Rect.anchoredPosition = Vector2.zero;
        nodes.Add(node);
    }

    // Called by a node's entrance, exit, or loop-back connect button
    public void BeginConnection(NodeUI source, ConnectionPointType pointType)
    {
        // If we already have a connection source
        if (connectionSource != null)
        {
            // Same node, same button - reset
            if (connectionSource == source && connectionPointType == pointType)
            {
                ResetConnectionMode();
                return;
            }

            // Same node, different button - switch
            if (connectionSource == source && connectionPointType != pointType)
            {
                connectionSource.ClearConnectionHighlights();
                ClearExistingConnectionHighlights();
                connectionPointType = pointType;
                source.SetConnectionHighlight(pointType);
                HighlightExistingConnections(source, pointType);
                return;
            }

            // Different node - transfer to new node
            connectionSource.ClearConnectionHighlights();
            ClearExistingConnectionHighlights();
        }

        // Check if this is a branch exit - if so, check color limit
        if (pointType == ConnectionPointType.Exit && source.NodeType == NodeType.Branch)
        {
            int currentExitCount = GetExitConnectionCount(source);
            if (currentExitCount >= connectionColors.Count)
            {
                // Too many connections, exit without doing anything
                return;
            }
        }

        connectionSource = source;
        connectionPointType = pointType;

        // Find existing connections for this source/point combo
        HighlightExistingConnections(source, pointType);

        foreach (var node in nodes)
        {
            if (node != source)
            {
                // Check if this node already has a connection from source
                if (existingConnectionNodes.Contains(node))
                {
                    node.SetExistingConnectionHighlight(true);
                }
                else
                {
                    node.SetHighlight(true);
                }
            }
        }
    }

    private void HighlightExistingConnections(NodeUI source, ConnectionPointType pointType)
    {
        existingConnectionNodes.Clear();

        foreach (var c in connections)
        {
            // Connections are always stored as exit/loopback -> entrance
            // So we need to check both directions based on the point type
            if (pointType == ConnectionPointType.Exit || pointType == ConnectionPointType.LoopBack)
            {
                // Source is the exit/loopback side
                if (c.from == source && c.fromPoint == pointType)
                {
                    existingConnectionNodes.Add(c.to);
                }
            }
            else if (pointType == ConnectionPointType.Entrance)
            {
                // Source is the entrance side, check if it's the 'to' node in any connection
                if (c.to == source)
                {
                    existingConnectionNodes.Add(c.from);
                }
            }
        }
    }

    private void ClearExistingConnectionHighlights()
    {
        foreach (var node in existingConnectionNodes)
        {
            node.SetExistingConnectionHighlight(false);
        }
        existingConnectionNodes.Clear();
    }

    private void ResetConnectionMode()
    {
        if (connectionSource != null)
        {
            connectionSource.ClearConnectionHighlights();
        }

        ClearExistingConnectionHighlights();

        foreach (var node in nodes)
        {
            node.SetHighlight(false);
            node.SetExistingConnectionHighlight(false);
        }

        connectionSource = null;
    }

    // Called when clicking on the connection source node itself (not on entrance/exit buttons)
    public bool TryResetIfConnectionSource(NodeUI node)
    {
        if (connectionSource == node)
        {
            ResetConnectionMode();
            return true;
        }
        return false;
    }

    public bool IsInConnectionMode => connectionSource != null;

    // Called when clicking a highlighted node
    public void TryCompleteConnection(NodeUI target)
    {
        if (connectionSource == null || target == connectionSource)
            return;

        // If target already has a connection from source, just exit connection mode
        if (existingConnectionNodes.Contains(target))
        {
            EndConnectionMode();
            return;
        }

        // Check for duplicate connection (normalized check)
        if (ConnectionExistsBetween(connectionSource, target, connectionPointType))
        {
            EndConnectionMode();
            return;
        }

        // Determine if this is a branch exit (allows multiple connections)
        // Need to check the normalized 'from' node for branch status
        NodeUI normalizedFromNode = (connectionPointType == ConnectionPointType.Exit ||
                                     connectionPointType == ConnectionPointType.LoopBack)
                                     ? connectionSource : target;
        ConnectionPointType normalizedFromPoint = (connectionPointType == ConnectionPointType.Exit ||
                                                   connectionPointType == ConnectionPointType.LoopBack)
                                                   ? connectionPointType : ConnectionPointType.Exit;

        bool isBranchExit = normalizedFromPoint == ConnectionPointType.Exit &&
                           normalizedFromNode.NodeType == NodeType.Branch;

        // For non-branch exits, replace existing connection if one exists
        if (!isBranchExit)
        {
            RemoveExistingConnectionsFrom(connectionSource, connectionPointType);
        }

        // Create connection with LineConnectionPoint
        CreateConnectionWithLinePoint(connectionSource, target, connectionPointType);

        EndConnectionMode();
    }

    /// <summary>
    /// Checks if a connection already exists between two nodes (in normalized form).
    /// </summary>
    private bool ConnectionExistsBetween(NodeUI source, NodeUI target, ConnectionPointType sourcePointType)
    {
        // Normalize to determine actual from/to
        NodeUI fromNode;
        NodeUI toNode;
        ConnectionPointType fromPointType;

        if (sourcePointType == ConnectionPointType.Exit || sourcePointType == ConnectionPointType.LoopBack)
        {
            fromNode = source;
            toNode = target;
            fromPointType = sourcePointType;
        }
        else
        {
            fromNode = target;
            toNode = source;
            fromPointType = ConnectionPointType.Exit;
        }

        // Check if this exact connection exists
        foreach (var c in connections)
        {
            if (c.from == fromNode && c.to == toNode && c.fromPoint == fromPointType)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveExistingConnectionsFrom(NodeUI source, ConnectionPointType pointType)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var c = connections[i];
            // Connections are always stored as exit/loopback -> entrance
            if (pointType == ConnectionPointType.Exit || pointType == ConnectionPointType.LoopBack)
            {
                if (c.from == source && c.fromPoint == pointType)
                {
                    RemoveConnection(c);
                }
            }
            else if (pointType == ConnectionPointType.Entrance)
            {
                // If connecting from entrance, remove connections where source is the 'to' node
                if (c.to == source)
                {
                    RemoveConnection(c);
                }
            }
        }
    }

    private void CreateConnectionWithLinePoint(NodeUI source, NodeUI target, ConnectionPointType sourcePointType)
    {
        // NORMALIZE: Connections are always stored as exit/loopback -> entrance
        // Determine the actual 'from' and 'to' nodes based on connection direction
        NodeUI fromNode;
        NodeUI toNode;
        ConnectionPointType fromPointType;
        ConnectionPointType toPointType = ConnectionPointType.Entrance;

        if (sourcePointType == ConnectionPointType.Exit || sourcePointType == ConnectionPointType.LoopBack)
        {
            // Source is exit/loopback, target is entrance - already normalized
            fromNode = source;
            toNode = target;
            fromPointType = sourcePointType;
        }
        else
        {
            // Source is entrance, target should be exit - swap to normalize
            fromNode = target;
            toNode = source;
            fromPointType = ConnectionPointType.Exit;
        }

        // Validate that fromNode has exit/loopback and toNode has entrance
        if (fromPointType == ConnectionPointType.Exit && !fromNode.HasExitPoint)
            return;
        if (fromPointType == ConnectionPointType.LoopBack && !fromNode.HasLoopBackPoint)
            return;
        if (!toNode.HasEntrancePoint)
            return;

        // Calculate connection index for coloring (only matters for branch exits)
        int connectionIndex = 0;
        if (fromPointType == ConnectionPointType.Exit && fromNode.NodeType == NodeType.Branch)
        {
            connectionIndex = GetExitConnectionCount(fromNode);
        }

        // Create the LineConnectionPoint
        LineConnectionPoint linePoint = Instantiate(lineConnectionPointPrefab, windowRect);
        linePoint.Initialize(this, fromNode, toNode, fromPointType, toPointType, connectionIndex);

        // Create lines from source to connection point, and from connection point to target
        UILineRenderer lineToPoint = Instantiate(linePrefab, lineParent);
        UILineRenderer lineFromPoint = Instantiate(linePrefab, lineParent);

        NodeConnection connection = new NodeConnection
        {
            from = fromNode,
            to = toNode,
            fromPoint = fromPointType,
            toPoint = toPointType,
            lineToConnectionPoint = lineToPoint,
            lineFromConnectionPoint = lineFromPoint,
            connectionPoint = linePoint
        };

        connections.Add(connection);
        UpdateLineWithConnectionPoint(connection);

        // Notify parameter data about connections
        if (fromPointType == ConnectionPointType.LoopBack && fromNode.ParameterData != null)
        {
            fromNode.ParameterData.OnLoopBackConnectionAdded(toNode);
        }
        else if (fromPointType == ConnectionPointType.Exit && fromNode.ParameterData != null)
        {
            fromNode.ParameterData.OnExitConnectionAdded(toNode);
        }
    }

    public int GetExitConnectionCount(NodeUI node)
    {
        int count = 0;
        foreach (var c in connections)
        {
            if (c.from == node && c.fromPoint == ConnectionPointType.Exit)
            {
                count++;
            }
        }
        return count;
    }

    public Color GetConnectionColor(int index)
    {
        if (index >= 0 && index < connectionColors.Count)
        {
            return connectionColors[index];
        }
        return Color.white;
    }

    private void EndConnectionMode()
    {
        ClearExistingConnectionHighlights();

        foreach (var node in nodes)
        {
            node.SetHighlight(false);
            node.SetExistingConnectionHighlight(false);
            node.ClearConnectionHighlights();
        }

        connectionSource = null;
    }

    public void UpdateConnectionsFor(NodeUI node)
    {
        foreach (var c in connections)
        {
            if (c.from == node || c.to == node)
                UpdateLineWithConnectionPoint(c);
        }
    }

    public void UpdateLinesForConnectionPoint(LineConnectionPoint connectionPoint)
    {
        foreach (var c in connections)
        {
            if (c.connectionPoint == connectionPoint)
            {
                UpdateLineWithConnectionPoint(c);
                break;
            }
        }
    }

    /// <summary>
    /// Removes a specific connection and notifies parameter data.
    /// </summary>
    private void RemoveConnection(NodeConnection connection)
    {
        if (connection == null) return;

        // Notify about loop-back connection removal
        if (connection.fromPoint == ConnectionPointType.LoopBack && connection.from.ParameterData != null)
        {
            connection.from.ParameterData.OnLoopBackConnectionRemoved(connection.to);
        }
        // Notify about exit connection removal
        else if (connection.fromPoint == ConnectionPointType.Exit && connection.from.ParameterData != null)
        {
            connection.from.ParameterData.OnExitConnectionRemoved(connection.to);
        }
        if (connection.toPoint == ConnectionPointType.Entrance && connection.to.ParameterData != null)
        {
            connection.to.ParameterData.OnExitConnectionRemoved(connection.from);
        }

        // Destroy the lines and connection point
        if (connection.lineToConnectionPoint != null)
        {
            Destroy(connection.lineToConnectionPoint.gameObject);
        }
        if (connection.lineFromConnectionPoint != null)
        {
            Destroy(connection.lineFromConnectionPoint.gameObject);
        }
        if (connection.connectionPoint != null)
        {
            Destroy(connection.connectionPoint.gameObject);
        }

        connections.Remove(connection);

        // Recolor remaining connections from the same branch exit
        if (connection.fromPoint == ConnectionPointType.Exit &&
            connection.from.NodeType == NodeType.Branch)
        {
            RecolorBranchExitConnections(connection.from);
        }
    }

    private void RecolorBranchExitConnections(NodeUI branchNode)
    {
        int index = 0;
        foreach (var c in connections)
        {
            if (c.from == branchNode && c.fromPoint == ConnectionPointType.Exit)
            {
                c.connectionPoint.SetConnectionIndex(index);
                index++;
            }
        }
    }

    /// <summary>
    /// Deletes a connection via its LineConnectionPoint.
    /// Called by LineConnectionPoint on right-click.
    /// </summary>
    public void DeleteConnectionPoint(LineConnectionPoint connectionPoint)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (connections[i].connectionPoint == connectionPoint)
            {
                RemoveConnection(connections[i]);
                break;
            }
        }
    }

    /// <summary>
    /// Removes all connections involving a node (for node deletion).
    /// </summary>
    public void RemoveAllConnectionsFor(NodeUI node)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var c = connections[i];
            if (c.from == node || c.to == node)
            {
                RemoveConnection(c);
            }
        }
    }

    /// <summary>
    /// Gets all exit connections from a node.
    /// </summary>
    public List<NodeUI> GetExitConnectionTargets(NodeUI node)
    {
        List<NodeUI> targets = new();
        foreach (var c in connections)
        {
            if (c.from == node && c.fromPoint == ConnectionPointType.Exit)
            {
                targets.Add(c.to);
            }
            if (c.to == node && c.toPoint == ConnectionPointType.Entrance)
            {
                targets.Add(c.from);
            }
        }
        return targets;
    }

    /// <summary>
    /// Gets the loop-back target for a Loop node.
    /// </summary>
    public NodeUI GetLoopBackTarget(NodeUI node)
    {
        foreach (var c in connections)
        {
            if (c.from == node && c.fromPoint == ConnectionPointType.LoopBack)
            {
                return c.to;
            }
        }
        return null;
    }

    public void DeleteNode()
    {
        if (currentParameterNode != null)
        {
            DeleteNode(currentParameterNode);
            CloseParameterWindow();
        }
    }
    private void DeleteNode(NodeUI node)
    {
        if (node == null || node == startNode || node == endNode) return; // Don't delete start or end nodes

        // Remove all connections
        RemoveAllConnectionsFor(node);

        // Clear parameter window if showing this node
        if (currentParameterNode == node)
        {
            CloseParameterWindow();
        }

        // Deselect if selected
        if (selectedNode == node)
        {
            DeselectCurrentNode();
        }

        // Reset connection mode if this was the source
        if (connectionSource == node)
        {
            ResetConnectionMode();
        }

        // Notify parameter data
        node.OnDelete();

        // Remove from list and destroy
        nodes.Remove(node);
        Destroy(node.gameObject);
    }

    private void UpdateLineWithConnectionPoint(NodeConnection c)
    {
        if (c.connectionPoint == null) return;

        Vector2 from = GetPointPosition(c.from, c.fromPoint);
        Vector2 connectionPointPos = c.connectionPoint.GetPosition();
        Vector2 to = GetPointPosition(c.to, c.toPoint);

        // Line from source to connection point
        if (c.lineToConnectionPoint != null)
        {
            c.lineToConnectionPoint.Points = new Vector2[] { from, connectionPointPos };
        }

        // Line from connection point to target
        if (c.lineFromConnectionPoint != null)
        {
            c.lineFromConnectionPoint.Points = new Vector2[] { connectionPointPos, to };
        }
    }

    private Vector2 GetPointPosition(NodeUI node, ConnectionPointType pointType)
    {
        return pointType switch
        {
            ConnectionPointType.Entrance => node.GetEntrancePoint(),
            ConnectionPointType.Exit => node.GetExitPoint(),
            ConnectionPointType.LoopBack => node.GetLoopBackPoint(),
            _ => node.GetExitPoint()
        };
    }

    /// <summary>
    /// Gets the next unique node ID.
    /// </summary>
    public ushort GetNextNodeId()
    {
        return nextNodeId++;
    }

    /// <summary>
    /// Shows parameters for the specified node in the parameter window.
    /// </summary>
    public void ShowParametersForNode(NodeUI node)
    {
        // Clear previous node's UI
        if (currentParameterNode != null && currentParameterNode.ParameterData != null)
        {
            currentParameterNode.ParameterData.ClearUI();
        }

        currentParameterNode = node;

        // Activate window if not already
        if (!parameterWindow.gameObject.activeSelf)
        {
            parameterWindow.gameObject.SetActive(true);
        }

        // Create UI for new node
        if (node != null && node.ParameterData != null)
        {
            node.ParameterData.CreateUI(parameterWindow);
        }
    }

    /// <summary>
    /// Refreshes the parameter window for the current node.
    /// Used when branch connections change.
    /// </summary>
    public void RefreshParameterWindow()
    {
        if (currentParameterNode != null)
        {
            ShowParametersForNode(currentParameterNode);
        }
    }

    public void OpenParameterWindow()
    {
        if (selectedNode == null) return;
        ShowParametersForNode(selectedNode);
        parameterWindow.gameObject.SetActive(true);
        openParameterWindowButton.gameObject.SetActive(false);
    }

    public void CloseParameterWindow()
    {
        // Clear current node's UI
        if (currentParameterNode != null && currentParameterNode.ParameterData != null)
        {
            currentParameterNode.ParameterData.ClearUI();
        }

        currentParameterNode = null;
        parameterWindow.gameObject.SetActive(false);
        openParameterWindowButton.gameObject.SetActive(true);

    }

    private class NodeConnection
    {
        public NodeUI from;
        public NodeUI to;
        public ConnectionPointType fromPoint;
        public ConnectionPointType toPoint;
        public UILineRenderer lineToConnectionPoint;
        public UILineRenderer lineFromConnectionPoint;
        public LineConnectionPoint connectionPoint;
    }
}