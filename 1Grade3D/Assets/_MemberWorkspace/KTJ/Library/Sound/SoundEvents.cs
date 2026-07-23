using GameLib.EventChannelSystem;
using UnityEngine;

public sealed class PlaySoundEvent : GameEvent
{
    public SoundClipSO ClipData { get; }
    public Transform Trans { get; }
    public int ChannelNumber { get; }

    public PlaySoundEvent(SoundClipSO clipData, Transform trans = null, int channelNumber = 0)
    {
        ClipData = clipData;
        Trans = trans;
        ChannelNumber = channelNumber;
    }
}

public sealed class StopSoundEvent : GameEvent
{
    public int ChannelNumber { get; }

    public StopSoundEvent(int channelNumber)
    {
        ChannelNumber = channelNumber;
    }
}
