using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script.Events
{
    public class SettlementEvent : GameEvent
    {
        public readonly List<ItemSO> items;

        public SettlementEvent(List<ItemSO> items)
        {
            this.items = items;
        }
    }
}