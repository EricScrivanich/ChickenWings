using System;
using UnityEngine;

/// <summary>
/// Serializable default values for node parameters.
/// Attach to each node prefab to configure its defaults in the Inspector.
/// </summary>
[Serializable]
public class NodeDefaults : MonoBehaviour
{
    [Header("Node Type")]
    [SerializeField] private NodeTreeCanvas.NodeType nodeType;

    [Header("Wave Defaults")]
    [SerializeField] private WaveDefaults waveDefaults = new();

    [Header("Loop Defaults")]
    [SerializeField] private LoopDefaults loopDefaults = new();

    [Header("Delay Defaults")]
    [SerializeField] private DelayDefaults delayDefaults = new();

    [Header("Branch Defaults")]
    [SerializeField] private BranchDefaults branchDefaults = new();

    public NodeTreeCanvas.NodeType NodeType => nodeType;
    public WaveDefaults Wave => waveDefaults;
    public LoopDefaults Loop => loopDefaults;
    public DelayDefaults Delay => delayDefaults;
    public BranchDefaults Branch => branchDefaults;

    /// <summary>
    /// Creates and initializes the appropriate parameter data based on node type.
    /// </summary>
    public NodeParameterData CreateParameterData(NodeUI owner, NodeTreeCanvas canvas)
    {
        NodeParameterData data = nodeType switch
        {
            NodeTreeCanvas.NodeType.Wave => CreateWaveData(),
            NodeTreeCanvas.NodeType.Loop => CreateLoopData(),
            NodeTreeCanvas.NodeType.Delay => CreateDelayData(),
            NodeTreeCanvas.NodeType.Branch => CreateBranchData(),
            _ => null
        };

        data?.Initialize(owner, canvas);
        return data;
    }

    private WaveParameterData CreateWaveData()
    {
        return new WaveParameterData
        {
            DefaultWaveId = waveDefaults.defaultWaveId,
            MinWaveId = waveDefaults.minWaveId,
            MaxWaveId = waveDefaults.maxWaveId
        };
    }

    private LoopParameterData CreateLoopData()
    {
        return new LoopParameterData
        {
            DefaultMinLoops = loopDefaults.defaultMinLoops,
            DefaultMaxLoops = loopDefaults.defaultMaxLoops,
            DefaultLoopChance = loopDefaults.defaultLoopChance,
            MinLoopsMin = loopDefaults.minLoopsMin,
            MinLoopsMax = loopDefaults.minLoopsMax,
            MaxLoopsMin = loopDefaults.maxLoopsMin,
            MaxLoopsMax = loopDefaults.maxLoopsMax
        };
    }

    private DelayParameterData CreateDelayData()
    {
        return new DelayParameterData
        {
            DefaultMinDelay = delayDefaults.defaultMinDelay,
            DefaultMaxDelay = delayDefaults.defaultMaxDelay,
            MinDelayMin = delayDefaults.minDelayMin,
            MinDelayMax = delayDefaults.minDelayMax,
            MaxDelayMin = delayDefaults.maxDelayMin,
            MaxDelayMax = delayDefaults.maxDelayMax
        };
    }

    private BranchParameterData CreateBranchData()
    {
        return new BranchParameterData
        {
            DefaultChance = branchDefaults.defaultChance
        };
    }
}

[Serializable]
public class WaveDefaults
{
    [Tooltip("Default Wave ID value")]
    public short defaultWaveId = 0;

    [Tooltip("Minimum allowed Wave ID")]
    public short minWaveId = 0;

    [Tooltip("Maximum allowed Wave ID")]
    public short maxWaveId = 5;
}

[Serializable]
public class LoopDefaults
{
    [Tooltip("Default minimum loops")]
    public ushort defaultMinLoops = 1;

    [Tooltip("Default maximum loops")]
    public ushort defaultMaxLoops = 3;

    [Tooltip("Default loop chance (0-100)")]
    [Range(0, 100)]
    public byte defaultLoopChance = 50;

    [Tooltip("Minimum value for Min Loops")]
    public ushort minLoopsMin = 0;

    [Tooltip("Maximum value for Min Loops")]
    public ushort minLoopsMax = 1000;

    [Tooltip("Minimum value for Max Loops")]
    public ushort maxLoopsMin = 0;

    [Tooltip("Maximum value for Max Loops")]
    public ushort maxLoopsMax = 1000;
}

[Serializable]
public class DelayDefaults
{
    [Tooltip("Default minimum delay")]
    public ushort defaultMinDelay = 0;

    [Tooltip("Default maximum delay")]
    public ushort defaultMaxDelay = 100;

    [Tooltip("Minimum value for Min Delay")]
    public ushort minDelayMin = 0;

    [Tooltip("Maximum value for Min Delay")]
    public ushort minDelayMax = 500;

    [Tooltip("Minimum value for Max Delay")]
    public ushort maxDelayMin = 0;

    [Tooltip("Maximum value for Max Delay")]
    public ushort maxDelayMax = 500;
}

[Serializable]
public class BranchDefaults
{
    [Tooltip("Default chance for new branch connections (0-100)")]
    [Range(0, 100)]
    public byte defaultChance = 50;
}
