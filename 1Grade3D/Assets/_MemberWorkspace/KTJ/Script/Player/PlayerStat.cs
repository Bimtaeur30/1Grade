using GameLib.EventChannelSystem;
using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;

    [field: SerializeField] public int MaxWeight { get; private set; }
    [field: SerializeField] public int RunSpeed { get; private set; }
    [field: SerializeField] public int ScanCooltime { get; private set; }

    public event Action<int> MaxWeightChanged;
    public event Action<int> RunSpeedChanged;
    public event Action<int> ScanCooltimeChanged;

    private void Awake()
    {
        PlayerChannel.AddListener<MaxWeightUpgradeEvent>(HandleMaxWeightUpgradeEvent);
        PlayerChannel.AddListener<RunSpeedUpgradeEvent>(HandleRunSpeedUpgradeEvent);
        PlayerChannel.AddListener<ScanCooltimeUpgradeEvent>(HandleScanCooltimeUpgradeEvent);
    }

    private void Start()
    {
        RunSpeedChanged?.Invoke(RunSpeed);
        MaxWeightChanged?.Invoke(MaxWeight);
        ScanCooltimeChanged?.Invoke(ScanCooltime);
    }

    private void HandleRunSpeedUpgradeEvent(RunSpeedUpgradeEvent @event)
    {
        RunSpeed += @event.Amount;
        RunSpeedChanged?.Invoke(RunSpeed);
    }

    private void HandleMaxWeightUpgradeEvent(MaxWeightUpgradeEvent @event)
    {
        MaxWeight += @event.Amount;
        MaxWeightChanged?.Invoke(MaxWeight);
    }

    private void HandleScanCooltimeUpgradeEvent(ScanCooltimeUpgradeEvent @event)
    {
        ScanCooltime -= @event.Amount;
        ScanCooltimeChanged?.Invoke(ScanCooltime);
    }
}
