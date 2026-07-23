using _MemberWorkspace.JJW.Asset._02_Script.Item;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ItemInfoUI : MonoBehaviour
    {
        [Serializable]
        public struct ItemInfoUIs
        {
            public Image icon;
            public TMP_Text nameText;
            public TMP_Text descriptionText;
            public TMP_Text gradeText;
            public TMP_Text durabilityText;
            public TMP_Text weightText;
            public TMP_Text priceText;
        }

        [SerializeField] private ItemInfoUIs itemInfoUI;

        public void Show(ItemSO item)
        {
            gameObject.SetActive(true);

            itemInfoUI.icon.sprite = item.Icon;
            itemInfoUI.nameText.text = item.Name;
            itemInfoUI.descriptionText.text = item.Description;
            itemInfoUI.gradeText.text = item.Grade.ToString();
            itemInfoUI.durabilityText.text = item.MaxDurability.ToString();
            itemInfoUI.weightText.text = item.Weight.ToString("F1");
            itemInfoUI.priceText.text = item.Price.ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}