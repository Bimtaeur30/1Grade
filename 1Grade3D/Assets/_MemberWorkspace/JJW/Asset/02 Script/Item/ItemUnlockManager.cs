using System;
using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using GameLib.EventChannelSystem;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Item
{
    [Serializable]
    public class ItemList
    {
        public List<ItemSO> itemList;
        public int spawnWeight;
    }

    public class ItemUnlockManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;

        [SerializeField] private ItemList commonItems;
        [SerializeField] private ItemList normalItems;
        [SerializeField] private ItemList legendaryItems;

        [SerializeField] private int baseUnlockItemCount = 5;
        [SerializeField] private int perTurnUnlockCount = 2;

        [SerializeField] private int basePickItemCount = 5;
        [SerializeField] private int perTurnPickItemCount = 1;

        public List<ItemSO> UnlockedItems { get; private set; } = new();//해금된 아이템들임 나중에 필요할 수도 있어서 퍼블릭해놓음

        private ItemGrade _currentGrade = ItemGrade.Common;

        private void OnEnable()
        {
            eventChannel.AddListener<StormStartEvent>(HandleStormStart);
        }

        private void OnDisable()
        {
            if (eventChannel != null)
                eventChannel.RemoveListener<StormStartEvent>(HandleStormStart);
        }

        private void Start()
        {
            UnlockItems(GetUnlockCount());
        }

        private int GetUnlockCount()
        {
            return baseUnlockItemCount + perTurnUnlockCount * (GameData.Instance.CurrentTurnLevel - 1);
        }

        private void UnlockItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ItemSO item = UnlockNextItem();
                if (item == null) break; // 모든 등급 다 소진됨
            }
        }

        private ItemSO UnlockNextItem()
        {
            List<ItemSO> pool = GetPoolByGrade(_currentGrade);
            List<ItemSO> remaining = pool.FindAll(item => !UnlockedItems.Contains(item));

            if (remaining.Count == 0)
            {
                if (!TryAdvanceGrade()) return null;
                return UnlockNextItem();
            }

            ItemSO picked = remaining[UnityEngine.Random.Range(0, remaining.Count)];
            UnlockedItems.Add(picked);
            return picked;
        }

        private bool TryAdvanceGrade()
        {
            switch (_currentGrade)
            {
                case ItemGrade.Common:
                    _currentGrade = ItemGrade.Normal;
                    return true;
                case ItemGrade.Normal:
                    _currentGrade = ItemGrade.Legendary;
                    return true;
                default:
                    return false;
            }
        }

        private List<ItemSO> GetPoolByGrade(ItemGrade grade)
        {
            return grade switch
            {
                ItemGrade.Common => commonItems.itemList,
                ItemGrade.Normal => normalItems.itemList,
                ItemGrade.Legendary => legendaryItems.itemList,
                _ => commonItems.itemList
            };
        }

        private void HandleStormStart(StormStartEvent evt)
        {
            int pickCount = GetPickCount();
            List<ItemSO> pickedItems = PickWeightedRandomItems(pickCount);

            eventChannel.RaiseEvent(FlowEvents.StormStartEvent.Init(pickedItems));

            UnlockItems(perTurnUnlockCount);
        }

        private int GetPickCount()
        {
            return basePickItemCount + perTurnPickItemCount * (GameData.Instance.CurrentTurnLevel - 1);
        }

        private List<ItemSO> PickWeightedRandomItems(int count)
        {
            List<ItemSO> result = new();

            for (int i = 0; i < count; i++)
            {
                ItemGrade grade = PickGradeByWeight();
                List<ItemSO> unlockedInGrade = UnlockedItems.FindAll(item => item.Grade == grade);

                if (unlockedInGrade.Count == 0) continue; // 해당 등급에 해금된 게 없으면 스킵할거

                ItemSO picked = unlockedInGrade[UnityEngine.Random.Range(0, unlockedInGrade.Count)];
                result.Add(picked);
            }

            return result;
        }

        private ItemGrade PickGradeByWeight()
        {
            int totalWeight = commonItems.spawnWeight + normalItems.spawnWeight + legendaryItems.spawnWeight;
            int roll = UnityEngine.Random.Range(0, totalWeight);

            if (roll < commonItems.spawnWeight) return ItemGrade.Common;
            roll -= commonItems.spawnWeight;

            if (roll < normalItems.spawnWeight) return ItemGrade.Normal;

            return ItemGrade.Legendary;
        }
    }
}