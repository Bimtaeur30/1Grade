using GameLib.EventChannelSystem;
using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;

    [field: SerializeField] public int MaxWeight { get; private set; }
    [field: SerializeField] public int RunSpeed { get; private set; }

    public event Action<int> RunSpeedChanged;

    private void Awake()
    {
        PlayerChannel.AddListener<MaxWeightUpgradeEvent>(HandleMaxWeightUpgradeEvent);
        PlayerChannel.AddListener<RunSpeedUpgradeEvent>(HandleRunSpeedUpgradeEvent);
    }

    private void HandleRunSpeedUpgradeEvent(RunSpeedUpgradeEvent @event)
    {
        RunSpeed += @event.Amount;
        RunSpeedChanged?.Invoke(RunSpeed);
    }

    private void HandleMaxWeightUpgradeEvent(MaxWeightUpgradeEvent @event)
    {
        MaxWeight += @event.Amount;
    }
}
