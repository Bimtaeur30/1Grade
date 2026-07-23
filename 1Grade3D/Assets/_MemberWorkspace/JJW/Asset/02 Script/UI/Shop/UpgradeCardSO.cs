using System;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public enum UpgradeType {MaxWeight, RunSpeed, ScanCooldown }

    [Serializable]
    public class LevelData
    {
        public int amount;
        public int cost;
        public string description;
    }

    [CreateAssetMenu(fileName = "UpgradeCardSO", menuName = "Item/UpgradeCardSO", order = 0)]
    public class UpgradeCardSO : ScriptableObject
    {
        [field: SerializeField] public string CardName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public UpgradeType Type { get; private set; }
        [field: SerializeField] public LevelData[] LevelDatas { get; private set; }
    }
}