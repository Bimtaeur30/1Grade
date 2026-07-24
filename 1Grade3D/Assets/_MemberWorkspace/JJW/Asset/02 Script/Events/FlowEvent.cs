using GameLib.EventChannelSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script.Events
{
    public static class FlowEvent
    {
        public static readonly StormStartEvent StormStartEvent = new();
        public static readonly StormEndEvent StormEndEvent = new();
    }

    public class StormStartEvent : GameEvent { }
    public class StormEndEvent : GameEvent { }
}