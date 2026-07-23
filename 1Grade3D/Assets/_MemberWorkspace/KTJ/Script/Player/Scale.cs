using GameLib.EventChannelSystem;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;

public enum ScalePlateEnum
{
    Left, Right
}

public class Scale : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;
    [SerializeField] private ScalePlate LeftScalePlate;
    [SerializeField] private ScalePlate RightScalePlate;

    private ItemSO currentItem;

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
        if (@event.Item == null)
        {
            Debug.LogError("Scale: ItemEquipEvent에 Item이 없습니다.", this);
            return;
        }

        currentItem = @event.Item;

        ScalePlate targetPlate = @event.ScalePlateEnum switch
        {
            ScalePlateEnum.Left => LeftScalePlate,
            ScalePlateEnum.Right => RightScalePlate,
            _ => null
        };

        if (targetPlate == null)
        {
            Debug.LogError(
                $"Scale: {@event.ScalePlateEnum} ScalePlate가 지정되지 않았습니다.",
                this);
            return;
        }

        targetPlate.AddItem(currentItem);
    }
}
