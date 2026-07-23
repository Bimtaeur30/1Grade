using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;

        [SerializeField] private RectTransform scannerCursor;
        [SerializeField] private RectTransform itemInfoRect;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image scannerImage;
        [SerializeField] private Image scannerGauge;

        [SerializeField] private ItemInfoUI itemInfoUI;

        [SerializeField] private Vector2 itemInfoOffset = new(25f, -25f);

        private void Awake()
        {
            Close();
        }

        private void Update()
        {
            if (canvasGroup.alpha == 0)
                return;

            Debug.Log(playerInput.MousePosition);
            Vector2 mousePos = playerInput.MousePosition;
            scannerCursor.position = mousePos;
            itemInfoRect.position = mousePos + itemInfoOffset;
        }

        public void Open()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            HideItemInfo();
        }

        public void SetGauge(float value)
        {
            scannerGauge.fillAmount = Mathf.Clamp01(value);
        }

        public void SetItemInfo(ItemSO item)
        {
            itemInfoUI.Show(item);
        }

        public void HideItemInfo()
        {
            itemInfoUI.Hide();
        }
    }
}