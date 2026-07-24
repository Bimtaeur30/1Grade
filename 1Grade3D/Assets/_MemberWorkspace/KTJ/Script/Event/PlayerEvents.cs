using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;

public static class PlayerEvents
{
    public static readonly ItemEquipEvent ItemEquipEvent = new();
    public static readonly RunSpeedUpgradeEvent RunSpeedUpgradeEvent = new();
    public static readonly MaxWeightUpgradeEvent MaxWeightUpgradeEvent = new();
    public static readonly ScanCooltimeUpgradeEvent ScanCooltimeUpgradeEvent = new();
    public static readonly ItemDigEvent ItemDigEvent = new();
    public static readonly ScannerEvent ScannerEvent = new();
}

public class ItemEquipEvent : GameEvent
{
    public ItemSO Item { get; private set; }
    public ScalePlateEnum ScalePlateEnum { get; private set; }
    public bool IsAccepted { get; private set; }

    public ItemEquipEvent Init(ItemSO item, ScalePlateEnum scalePlateEnum)
    {
        Item = item;
        ScalePlateEnum = scalePlateEnum;
        IsAccepted = false;
        return this;
    }

    public void Accept()
    {
        IsAccepted = true;
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
    public int GroundCeilNumber { get; set; }
    public ItemDigEvent Init(int groundCeilNumber)
    {
        GroundCeilNumber = groundCeilNumber;
        return this;
    }
}

public class ScannerEvent : GameEvent
{
    public bool IsStart { get; private set; }

    public ScannerEvent Init(bool isStart)
    {
        IsStart = isStart;
        return this;
    }
}
