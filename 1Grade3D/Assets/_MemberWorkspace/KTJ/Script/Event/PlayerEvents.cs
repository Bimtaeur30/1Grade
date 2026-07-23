using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using UnityEngine;

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