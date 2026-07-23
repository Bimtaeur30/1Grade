using GameLib.EventChannelSystem;
using System;
using UnityEngine;

public class Scale : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;

    private void Awake()
    {
        PlayerChannel.AddListener<ItemEquipEvent>(HandleItemEquipEvent);
    }
    private void OnDestroy()
    {
        PlayerChannel.RemoveListener<ItemEquipEvent>(HandleItemEquipEvent);
    }

    private void HandleItemEquipEvent(ItemEquipEvent @event)
    {

    }
}
