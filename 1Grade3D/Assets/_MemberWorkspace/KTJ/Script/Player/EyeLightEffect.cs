using GameLib.EventChannelSystem;
using System;
using UnityEngine;

public class EyeLightEffect : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;
    [SerializeField] private GameObject LightObject;

    private void Awake()
    {
        PlayerChannel.AddListener<ScannerEvent>(HandleScannerEvent);
    }
    private void OnDestroy()
    {
        PlayerChannel.RemoveListener<ScannerEvent>(HandleScannerEvent);
    }
    private void HandleScannerEvent(ScannerEvent @event)
    {
        LightObject.SetActive(@event.IsStart);
    }
}
