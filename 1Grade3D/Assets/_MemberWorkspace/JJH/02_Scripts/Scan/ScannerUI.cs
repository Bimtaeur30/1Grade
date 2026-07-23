using _MemberWorkspace.JJH._02_Scripts.Map;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;

        [SerializeField] private RectTransform scannerTrans;
        [SerializeField] private ScanUICanvas itemInfoCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image scannerGauge;

        [SerializeField] private ItemInfoUI itemInfoUI;

        private void Awake()
        {
            Close();
        }

        private void Update()
        {
            if (canvasGroup.alpha == 0)
                return;

            scannerTrans.position = playerInput.MousePosition;
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

        public void SetItemInfo(GroundItem item)
        {
            itemInfoCanvas.SetTarget(item.transform);
            itemInfoUI.Show(item.Item);
        }

        public void HideItemInfo()
        {
            itemInfoCanvas.SetTarget(null);
            itemInfoUI.Hide();
        }
    }
}