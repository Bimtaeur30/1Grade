using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ScaleItemEquipTest : MonoBehaviour
{
    [SerializeField] private EventChannelSO playerChannel;
    [SerializeField] private ItemSO testItem;

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.tKey.wasPressedThisFrame)
        {
            return;
        }

        if (playerChannel == null || testItem == null)
        {
            Debug.LogError(
                "ScaleItemEquipTest: PlayerChannel 또는 TestItem이 지정되지 않았습니다.",
                this);
            return;
        }

        ScalePlateEnum randomPlate = Random.value < 0.5f
            ? ScalePlateEnum.Left
            : ScalePlateEnum.Right;

        playerChannel.RaiseEvent(
            PlayerEvents.ItemEquipEvent.Init(testItem, randomPlate));

        Debug.Log(
            $"Scale 테스트 아이템 추가: {testItem.Name} → {randomPlate}",
            this);
    }
}
