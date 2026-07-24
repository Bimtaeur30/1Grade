using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class FailBuyUX : MonoBehaviour
    {
        [Header("좌우 흔들림")]
        [SerializeField] private float shakeDuration = 0.4f;
        [SerializeField] private float shakeStrength = 20f; //좌우 흔들림 세기(px)
        [SerializeField] private int shakeVibrato = 20;

        [Header("빨간 깜빡임")]
        [SerializeField] private Color failColor = Color.red;
        [SerializeField] private int blinkCount = 3;
        [SerializeField] private float blinkInterval = 0.1f;
        
        [SerializeField] private TextMeshProUGUI target;

        private Sequence _sequence;

        public void HandleFailBuy()
        {
            if (target == null) return;

            _sequence?.Complete();

            RectTransform rect = target.rectTransform;
            Vector2 originalPos = rect.anchoredPosition;
            Color originalColor = target.color;

            _sequence = DOTween.Sequence();

            _sequence.Join(rect.DOShakeAnchorPos(
                shakeDuration,
                new Vector2(shakeStrength, 0f),
                shakeVibrato,
                90f,
                snapping: false,
                fadeOut: true));

            _sequence.Join(DOTween.To(
                    () => 0f,
                    t => target.color = Color.Lerp(originalColor, failColor, t),
                    1f,
                    blinkInterval)
                .SetLoops(blinkCount * 2, LoopType.Yoyo));

            _sequence.OnComplete(() =>
            {
                rect.anchoredPosition = originalPos;
                target.color = originalColor;
                _sequence = null;
            });
        }
    }
}
