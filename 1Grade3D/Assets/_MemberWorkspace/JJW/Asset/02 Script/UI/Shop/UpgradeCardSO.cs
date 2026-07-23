using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    [CreateAssetMenu(fileName = "UpgradeCardSO", menuName = "Item/UpgradeCardSO", order = 0)]
    public class UpgradeCardSO : ScriptableObject
    {
        [field: SerializeField] public string CardName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        // [field: SerializeField] public UpgradeType Type { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public float UpgradeAmount { get; private set; }
    }
}