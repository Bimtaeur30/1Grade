using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Item
{
    public enum ItemGrade { Common,Normal,Legendary }

    [CreateAssetMenu(fileName = "ItemSO", menuName = "Item/ItemSO", order = 0)]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public ItemGrade Grade { get; private set; }
        [field: SerializeField] public int MaxDurability { get; private set; }//시작 내구도 체력
        [field: SerializeField] public float Weight { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
    }
}