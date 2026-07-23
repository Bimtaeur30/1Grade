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
        [SerializeField] private float screenPadding = 10f;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            Close();
        }

        private void Update()
        {
            if (canvasGroup.alpha == 0)
                return;

            Vector2 mousePos = playerInput.MousePosition;
            scannerCursor.position = mousePos;

            Vector2 desiredPos = mousePos + itemInfoOffset;
            itemInfoRect.position = ClampToScreen(desiredPos);
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

        private Vector2 ClampToScreen(Vector2 screenPos)
        {
            Vector3[] corners = new Vector3[4];
            itemInfoRect.GetWorldCorners(corners);

            float width = corners[2].x - corners[0].x;
            float height = corners[2].y - corners[0].y;

            Vector2 pivot = itemInfoRect.pivot;
            float left = screenPos.x - width * pivot.x;
            float right = screenPos.x + width * (1f - pivot.x);
            float bottom = screenPos.y - height * pivot.y;
            float top = screenPos.y + height * (1f - pivot.y);

            float offsetX = 0f;
            float offsetY = 0f;

            if (left < screenPadding)
                offsetX = screenPadding - left;
            else if (right > Screen.width - screenPadding)
                offsetX = (Screen.width - screenPadding) - right;

            if (bottom < screenPadding)
                offsetY = screenPadding - bottom;
            else if (top > Screen.height - screenPadding)
                offsetY = (Screen.height - screenPadding) - top;

            return screenPos + new Vector2(offsetX, offsetY);
        }
    }
}