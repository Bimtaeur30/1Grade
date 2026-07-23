using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using UnityEngine;

public static class PlayerEvents
{
    public static readonly ItemEquipEvent ItemEquipEvent = new();
    public static readonly RunSpeedUpgradeEvent RunSpeedUpgradeEvent = new();
    public static readonly MaxWeightUpgradeEvent MaxWeightUpgradeEvent = new();
    public static readonly ScanCooltimeUpgradeEvent ScanCooltimeUpgradeEvent = new();
    public static readonly ItemDigEvent ItemDigEvent = new();
}

public class ItemEquipEvent : GameEvent
{
    public ItemSO Item { get; private set; }
    public ItemEquipEvent Init(ItemSO item)
    {
        Item = item;
        return this;
    }
}
public class RunSpeedUpgradeEvent : GameEvent
{
    public int Amount { get; private set; }
    public RunSpeedUpgradeEvent Init(int amount)
    {
        Amount = amount;
        return this;
    }
}
public class MaxWeightUpgradeEvent : GameEvent
{
    public int Amount { get; private set; }
    public MaxWeightUpgradeEvent Init(int amount)
    {
        Amount = amount;
        return this;
    }
}
public class ScanCooltimeUpgradeEvent : GameEvent
{
    public int Amount { get; private set; }
    public ScanCooltimeUpgradeEvent Init(int amount)
    {
        Amount = amount;
        return this;
    }
}
public class ItemDigEvent : GameEvent
{
    public int GroundCeilNumber { get; private set; }
    public ItemDigEvent Init(int groundCeilNumber)
    {
        GroundCeilNumber = groundCeilNumber;
        return this;
    }
}