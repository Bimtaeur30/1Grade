using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;

public static class PlayerEvents
{
    public static readonly ItemEquipEvent ItemEquipEvent = new();
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

public class ScannerEvent : GameEvent
{
    public bool IsStart { get; private set; }

    public ScannerEvent Init(bool isStart)
    {
        IsStart = isStart;
        return this;
    }
}