using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all node parameter data. Each node type has its own implementation.
/// </summary>
[Serializable]
public abstract class NodeParameterData
{
    protected NodeUI ownerNode;
    protected NodeTreeCanvas canvas;
    protected List<GameObject> activeUIElements = new();

    public virtual void Initialize(NodeUI owner, NodeTreeCanvas canvas)
    {
        this.ownerNode = owner;
        this.canvas = canvas;
    }

    /// <summary>
    /// Creates UI elements for editing this node's parameters.
    /// </summary>
    public abstract void CreateUI(Transform parent);

    /// <summary>
    /// Clears all UI elements created by this data.
    /// </summary>
    public virtual void ClearUI()
    {
        foreach (var element in activeUIElements)
        {
            if (element != null)
                UnityEngine.Object.Destroy(element);
        }
        activeUIElements.Clear();
    }

    /// <summary>
    /// Resets all data to default values.
    /// </summary>
    public abstract void ResetToDefaults();

    /// <summary>
    /// Called when this node is being deleted. Clean up any references.
    /// </summary>
    public virtual void OnNodeDeleted()
    {
        ClearUI();
    }

    /// <summary>
    /// Called when a connection is added from this node's exit.
    /// Override in BranchParameterData to track exit connections.
    /// </summary>
    public virtual void OnExitConnectionAdded(NodeUI targetNode) { }

    /// <summary>
    /// Called when a connection from this node's exit is removed.
    /// Override in BranchParameterData to remove associated chance value.
    /// </summary>
    public virtual void OnExitConnectionRemoved(NodeUI targetNode) { }

    /// <summary>
    /// Called when a loop-back connection is added from this node.
    /// Override in LoopParameterData to track the loop-back target.
    /// </summary>
    public virtual void OnLoopBackConnectionAdded(NodeUI targetNode) { }

    /// <summary>
    /// Called when a loop-back connection from this node is removed.
    /// Override in LoopParameterData to clear the loop-back target.
    /// </summary>
    public virtual void OnLoopBackConnectionRemoved(NodeUI targetNode) { }
}

/// <summary>
/// Parameter data for Wave nodes.
/// Stores a single Wave ID value.
/// </summary>
[Serializable]
public class WaveParameterData : NodeParameterData, IBoxSliderListener
{
    [SerializeField] private short waveId;
    [SerializeField] private short defaultWaveId = 0;
    [SerializeField] private short minWaveId = 0;
    [SerializeField] private short maxWaveId = 5;

    public short WaveId => waveId;

    // Setters for initialization from NodeDefaults
    public short DefaultWaveId { set => defaultWaveId = value; }
    public short MinWaveId { set => minWaveId = value; }
    public short MaxWaveId { set => maxWaveId = value; }

    private BoxTypeChanger waveIdChanger;

    public override void Initialize(NodeUI owner, NodeTreeCanvas canvas)
    {
        base.Initialize(owner, canvas);
        waveId = defaultWaveId;
    }

    public void SetRange(short min, short max)
    {
        minWaveId = min;
        maxWaveId = max;
        waveId = (short)Mathf.Clamp(waveId, minWaveId, maxWaveId);
    }

    public override void CreateUI(Transform parent)
    {
        if (canvas.BoxTypeChangerPrefab == null) return;

        GameObject obj = UnityEngine.Object.Instantiate(canvas.BoxTypeChangerPrefab, parent);
        activeUIElements.Add(obj);

        waveIdChanger = obj.GetComponent<BoxTypeChanger>();
        if (waveIdChanger != null)
        {
            // Generate names array for the range
            string[] names = new string[maxWaveId - minWaveId + 1];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = (minWaveId + i).ToString();
            }

            waveIdChanger.Initialize(this);
            waveIdChanger.SetData("Wave ID: ", 0, waveId - minWaveId, names);
        }
    }

    public override void ResetToDefaults()
    {
        waveId = defaultWaveId;
    }

    public void OnBoxSliderValueChanged(int type, float value) { }
    public void OnBoxSliderTimeValueChanged(int type, ushort value) { }
    public void OnBoxSliderByteValueChanged(int type, byte value) { }

    public void OnBoxTypeChanged(int type, int value)
    {
        if (type == 0)
        {
            waveId = (short)Mathf.Clamp(minWaveId + value, minWaveId, maxWaveId);
        }
    }
}

/// <summary>
/// Parameter data for Loop nodes.
/// Stores Min Loops, Max Loops, Loop Chance, and Loop-Back Target.
/// </summary>
[Serializable]
public class LoopParameterData : NodeParameterData, IBoxSliderListener
{
    private const int TYPE_MIN_LOOPS = 0;
    private const int TYPE_MAX_LOOPS = 1;
    private const int TYPE_LOOP_CHANCE = 2;

    [SerializeField] private ushort minLoops;
    [SerializeField] private ushort maxLoops;
    [SerializeField] private byte loopChance; // 0-100 as percentage
    [SerializeField] private NodeUI loopBackTarget; // The node this loop iterates back to

    [Header("Defaults")]
    [SerializeField] private ushort defaultMinLoops = 1;
    [SerializeField] private ushort defaultMaxLoops = 3;
    [SerializeField] private byte defaultLoopChance = 50;

    [Header("Constraints")]
    [SerializeField] private ushort minLoopsMin = 0;
    [SerializeField] private ushort minLoopsMax = 1000;
    [SerializeField] private ushort maxLoopsMin = 0;
    [SerializeField] private ushort maxLoopsMax = 1000;

    public ushort MinLoops => minLoops;
    public ushort MaxLoops => maxLoops;
    public byte LoopChance => loopChance;
    public NodeUI LoopBackTarget => loopBackTarget;

    // Setters for initialization from NodeDefaults
    public ushort DefaultMinLoops { set => defaultMinLoops = value; }
    public ushort DefaultMaxLoops { set => defaultMaxLoops = value; }
    public byte DefaultLoopChance { set => defaultLoopChance = value; }
    public ushort MinLoopsMin { set => minLoopsMin = value; }
    public ushort MinLoopsMax { set => minLoopsMax = value; }
    public ushort MaxLoopsMin { set => maxLoopsMin = value; }
    public ushort MaxLoopsMax { set => maxLoopsMax = value; }

    private BoxTypeChanger minLoopsChanger;
    private BoxTypeChanger maxLoopsChanger;
    private BoxSlider loopChanceSlider;

    public override void Initialize(NodeUI owner, NodeTreeCanvas canvas)
    {
        base.Initialize(owner, canvas);
        minLoops = defaultMinLoops;
        maxLoops = defaultMaxLoops;
        loopChance = defaultLoopChance;
        loopBackTarget = null;
    }

    public override void CreateUI(Transform parent)
    {
        // Generate names arrays for loop counts


        // Min Loops BoxTypeChanger
        if (canvas.BoxTypeChangerPrefab != null)
        {
            GameObject minObj = UnityEngine.Object.Instantiate(canvas.BoxTypeChangerPrefab, parent);
            activeUIElements.Add(minObj);

            minLoopsChanger = minObj.GetComponent<BoxTypeChanger>();
            if (minLoopsChanger != null)
            {
                minLoopsChanger.Initialize(this);
                minLoopsChanger.SetData("Min Loops: ", TYPE_MIN_LOOPS, minLoops - minLoopsMin, null, true);
                minLoopsChanger.minTypeIndex = minLoopsMin;
                minLoopsChanger.maxTypeIndex = maxLoops - maxLoopsMin;
            }

            // Max Loops BoxTypeChanger
            GameObject maxObj = UnityEngine.Object.Instantiate(canvas.BoxTypeChangerPrefab, parent);
            activeUIElements.Add(maxObj);

            maxLoopsChanger = maxObj.GetComponent<BoxTypeChanger>();
            if (maxLoopsChanger != null)
            {
                maxLoopsChanger.Initialize(this);
                maxLoopsChanger.SetData("Max Loops: ", TYPE_MAX_LOOPS, maxLoops - maxLoopsMin, null, true);
                maxLoopsChanger.minTypeIndex = minLoops - minLoopsMin;
                maxLoopsChanger.maxTypeIndex = maxLoopsMax;
            }
        }

        // Loop Chance BoxSlider (Byte type)
        if (canvas.BoxSliderPrefab != null)
        {
            GameObject chanceObj = UnityEngine.Object.Instantiate(canvas.BoxSliderPrefab, parent);
            activeUIElements.Add(chanceObj);

            loopChanceSlider = chanceObj.GetComponent<BoxSlider>();
            if (loopChanceSlider != null)
            {
                loopChanceSlider.SetByteRange(0, 100);
                loopChanceSlider.Initialize(this, TYPE_LOOP_CHANCE, BoxSlider.BoxSliderType.Byte, "Loop Chance");
                loopChanceSlider.SetByteValue(loopChance);
            }
        }
    }

    public override void ResetToDefaults()
    {
        minLoops = defaultMinLoops;
        maxLoops = defaultMaxLoops;
        loopChance = defaultLoopChance;
    }

    public void OnBoxSliderValueChanged(int type, float value) { }
    public void OnBoxSliderTimeValueChanged(int type, ushort value) { }

    public void OnBoxSliderByteValueChanged(int type, byte value)
    {
        if (type == TYPE_LOOP_CHANCE)
        {
            loopChance = value;
        }
    }

    public void OnBoxTypeChanged(int type, int value)
    {
        if (type == TYPE_MIN_LOOPS)
        {
            minLoops = (ushort)Mathf.Clamp(minLoopsMin + value, minLoopsMin, Mathf.Min(maxLoops, minLoopsMax));
            maxLoopsChanger.minTypeIndex = minLoops;
        }
        else if (type == TYPE_MAX_LOOPS)
        {
            maxLoops = (ushort)Mathf.Clamp(maxLoopsMin + value, Mathf.Max(minLoops, maxLoopsMin), maxLoopsMax);
            minLoopsChanger.maxTypeIndex = maxLoops;
        }
    }

    public override void OnLoopBackConnectionAdded(NodeUI targetNode)
    {
        loopBackTarget = targetNode;

        // If parameter window is showing this node, refresh UI
        if (canvas.CurrentParameterNode == ownerNode)
        {
            canvas.RefreshParameterWindow();
        }
    }

    public override void OnLoopBackConnectionRemoved(NodeUI targetNode)
    {
        if (loopBackTarget == targetNode)
        {
            loopBackTarget = null;

            // If parameter window is showing this node, refresh UI
            if (canvas.CurrentParameterNode == ownerNode)
            {
                canvas.RefreshParameterWindow();
            }
        }
    }

    public override void OnNodeDeleted()
    {
        base.OnNodeDeleted();
        loopBackTarget = null;
    }
}

/// <summary>
/// Parameter data for Delay nodes.
/// Stores Min Delay and Max Delay as time values.
/// </summary>
[Serializable]
public class DelayParameterData : NodeParameterData, IBoxSliderListener
{
    private const int TYPE_MIN_DELAY = 0;
    private const int TYPE_MAX_DELAY = 1;

    [SerializeField] private ushort minDelay;
    [SerializeField] private ushort maxDelay;

    [Header("Defaults")]
    [SerializeField] private ushort defaultMinDelay = 0;
    [SerializeField] private ushort defaultMaxDelay = 100;

    [Header("Constraints")]
    [SerializeField] private ushort minDelayMin = 0;
    [SerializeField] private ushort minDelayMax = 500;
    [SerializeField] private ushort maxDelayMin = 0;
    [SerializeField] private ushort maxDelayMax = 500;

    public ushort MinDelay => minDelay;
    public ushort MaxDelay => maxDelay;

    // Setters for initialization from NodeDefaults
    public ushort DefaultMinDelay { set => defaultMinDelay = value; }
    public ushort DefaultMaxDelay { set => defaultMaxDelay = value; }
    public ushort MinDelayMin { set => minDelayMin = value; }
    public ushort MinDelayMax { set => minDelayMax = value; }
    public ushort MaxDelayMin { set => maxDelayMin = value; }
    public ushort MaxDelayMax { set => maxDelayMax = value; }

    private BoxSlider minDelaySlider;
    private BoxSlider maxDelaySlider;

    public override void Initialize(NodeUI owner, NodeTreeCanvas canvas)
    {
        base.Initialize(owner, canvas);
        minDelay = defaultMinDelay;
        maxDelay = defaultMaxDelay;
    }

    public override void CreateUI(Transform parent)
    {
        if (canvas.BoxSliderPrefab == null) return;

        // Min Delay BoxSlider (Time type)
        GameObject minObj = UnityEngine.Object.Instantiate(canvas.BoxSliderPrefab, parent);
        activeUIElements.Add(minObj);

        minDelaySlider = minObj.GetComponent<BoxSlider>();
        if (minDelaySlider != null)
        {
            minDelaySlider.SetTimeRange(minDelayMin, maxDelay);
            minDelaySlider.Initialize(this, TYPE_MIN_DELAY, BoxSlider.BoxSliderType.Time, "Min Delay");
            minDelaySlider.SetTimeValue(minDelay);
        }

        // Max Delay BoxSlider (Time type)
        GameObject maxObj = UnityEngine.Object.Instantiate(canvas.BoxSliderPrefab, parent);
        activeUIElements.Add(maxObj);

        maxDelaySlider = maxObj.GetComponent<BoxSlider>();
        if (maxDelaySlider != null)
        {
            maxDelaySlider.SetTimeRange(minDelay, maxDelayMax);
            maxDelaySlider.Initialize(this, TYPE_MAX_DELAY, BoxSlider.BoxSliderType.Time, "Max Delay");
            maxDelaySlider.SetTimeValue(maxDelay);
        }
    }

    public override void ResetToDefaults()
    {
        minDelay = defaultMinDelay;
        maxDelay = defaultMaxDelay;
    }

    public void OnBoxSliderValueChanged(int type, float value) { }
    public void OnBoxSliderByteValueChanged(int type, byte value) { }

    public void OnBoxSliderTimeValueChanged(int type, ushort value)
    {
        if (type == TYPE_MIN_DELAY)
        {
            minDelay = (ushort)Mathf.Clamp(value, minDelayMin, Mathf.Min(maxDelay, minDelayMax));
            maxDelaySlider.SetTimeRange(value, maxDelayMax);
        }
        else if (type == TYPE_MAX_DELAY)
        {
            maxDelay = (ushort)Mathf.Clamp(value, Mathf.Max(minDelay, maxDelayMin), maxDelayMax);
            minDelaySlider.SetTimeRange(minDelayMin, value);
        }
    }

    public void OnBoxTypeChanged(int type, int value) { }
}

/// <summary>
/// Parameter data for Branch nodes.
/// Dynamically stores a chance value (0-100) for each exit connection.
/// </summary>
[Serializable]
public class BranchParameterData : NodeParameterData, IBoxSliderListener
{
    [Serializable]
    public class BranchConnection
    {
        public NodeUI targetNode;
        public byte chance; // 0-100
    }

    [SerializeField] private List<BranchConnection> exitConnections = new();
    [SerializeField] private byte defaultChance = 50;

    // Setter for initialization from NodeDefaults
    public byte DefaultChance { set => defaultChance = value; }

    public IReadOnlyList<BranchConnection> ExitConnections => exitConnections;

    private List<BoxSlider> chanceSliders = new();

    public override void Initialize(NodeUI owner, NodeTreeCanvas canvas)
    {
        base.Initialize(owner, canvas);
        exitConnections.Clear();
    }

    public override void CreateUI(Transform parent)
    {
        if (canvas.BoxSliderPrefab == null) return;

        chanceSliders.Clear();

        for (int i = 0; i < exitConnections.Count; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(canvas.BoxSliderPrefab, parent);
            activeUIElements.Add(obj);

            BoxSlider slider = obj.GetComponent<BoxSlider>();
            if (slider != null)
            {
                string title = "Branch " + (i + 1) + " Chance";
                slider.SetByteRange(0, 100);
                slider.Initialize(this, i, BoxSlider.BoxSliderType.Byte, title);
                slider.SetByteValue(exitConnections[i].chance);
                chanceSliders.Add(slider);
            }
        }
    }

    public override void ClearUI()
    {
        base.ClearUI();
        chanceSliders.Clear();
    }

    public override void ResetToDefaults()
    {
        foreach (var connection in exitConnections)
        {
            connection.chance = defaultChance;
        }
    }

    public override void OnExitConnectionAdded(NodeUI targetNode)
    {
        // Check if connection already exists
        foreach (var conn in exitConnections)
        {
            if (conn.targetNode == targetNode)
                return;
        }

        exitConnections.Add(new BranchConnection
        {
            targetNode = targetNode,
            chance = defaultChance
        });

        // If parameter window is showing this node, refresh UI
        if (canvas.CurrentParameterNode == ownerNode)
        {
            canvas.RefreshParameterWindow();
        }
    }

    public override void OnExitConnectionRemoved(NodeUI targetNode)
    {
        for (int i = exitConnections.Count - 1; i >= 0; i--)
        {
            if (exitConnections[i].targetNode == targetNode)
            {
                exitConnections.RemoveAt(i);
                break;
            }
        }

        // If parameter window is showing this node, refresh UI
        if (canvas.CurrentParameterNode == ownerNode)
        {
            canvas.RefreshParameterWindow();
        }
    }

    public override void OnNodeDeleted()
    {
        base.OnNodeDeleted();
        exitConnections.Clear();
    }

    public void OnBoxSliderValueChanged(int type, float value) { }
    public void OnBoxSliderTimeValueChanged(int type, ushort value) { }

    public void OnBoxSliderByteValueChanged(int type, byte value)
    {
        if (type >= 0 && type < exitConnections.Count)
        {
            exitConnections[type].chance = value;
        }
    }

    public void OnBoxTypeChanged(int type, int value) { }

    public int GetConnectionIndex(NodeUI targetNode)
    {
        for (int i = 0; i < exitConnections.Count; i++)
        {
            if (exitConnections[i].targetNode == targetNode)
                return i;
        }
        return -1;
    }
}
