using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Image cctvImage;
        [SerializeField] private Image mouseGauge;

        [SerializeField] private ItemInfoUI itemInfoUI;

        public void Open()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 1;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void SetGauge(float value)
        {
            mouseGauge.fillAmount = Mathf.Clamp01(value);
        }

        public void ShowItem(ItemSO item)
        {
            itemInfoUI.Show(item);
        }

        public void HideItemInfo()
        {
            itemInfoUI.Hide();
        }
    }
}