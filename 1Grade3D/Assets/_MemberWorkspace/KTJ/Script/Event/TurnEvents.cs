using GameLib.EventChannelSystem;
using UnityEngine;

public static class TurnEvents
{
    public static readonly TurnStartEvent TurnStartEvent = new();
    public static readonly TurnEndEvent TurnEndEvent = new();
}

public class TurnStartEvent : GameEvent { }
public class TurnEndEvent : GameEvent { }