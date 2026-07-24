using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script.Events
{
    public static class SettlementEvents 
    {
        public static readonly SettlementEvent SettlementEvent = new(new List<ItemSO>());
    }

    public class SettlementEvent : GameEvent
    {
        public List<ItemSO> Items { get; private set; }

        public SettlementEvent Init(List<ItemSO> items)//태준이가 초기화 해서 호출
        {
            Items = items;
            return this;
        }

        public SettlementEvent(List<ItemSO> items)
        {
            Items = items;
        }
    }
}