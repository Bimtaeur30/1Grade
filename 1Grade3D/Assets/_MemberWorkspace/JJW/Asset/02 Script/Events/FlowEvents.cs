using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script.Events
{
    public static class FlowEvents
    {
        public static readonly StormStartEvent StormStartEvent = new();
        public static readonly StormEndEvent StormEndEvent = new();
        public static readonly ShopOpenEvent ShopOpenEvent = new();
        public static readonly ShopCloseEvent ShopCloseEvent = new();
    }

    public class StormStartEvent : GameEvent
    {
        public StormStartEvent Init(List<ItemSO> itemList)
        {
            ItemList =  itemList;
            return this;
        }

        public List<ItemSO> ItemList { get;private set; }
    }
    public class StormEndEvent : GameEvent { }
    public class ShopOpenEvent : GameEvent { }
    public class ShopCloseEvent : GameEvent { }
}