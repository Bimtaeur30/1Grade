using _MemberWorkspace.JJW.Asset._02_Script.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ItemInfoUI : MonoBehaviour
    {
        [SerializeField] private Image icon;

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text gradeText;
        [SerializeField] private TMP_Text durabilityText;
        [SerializeField] private TMP_Text weightText;
        [SerializeField] private TMP_Text priceText;

        public void Show(ItemSO item)
        {
            gameObject.SetActive(true);

            icon.sprite = item.Icon;
            nameText.text = item.Name;
            descriptionText.text = item.Description;
            gradeText.text = item.Grade.ToString();
            durabilityText.text = item.MaxDurability.ToString();
            weightText.text = item.Weight.ToString("F1");
            priceText.text = item.Price.ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}