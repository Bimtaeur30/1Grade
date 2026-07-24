using _MemberWorkspace.JJH._02_Scripts.Map;
using System.Collections;
using TMPro;
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
        [SerializeField] private Slider coolTimeSlider;
        [SerializeField] private CanvasGroup coolTimeCanvasGroup;
        [SerializeField] private TMP_Text scanTxt;

        [SerializeField] private ItemInfoUI itemInfoUI;

        [Header("Cool Time UI")]
        [SerializeField, Min(0.01f)] private float coolTimeFadeDuration = 0.25f;

        [Header("CRT Animation")]
        [SerializeField] private RectTransform effectRoot;
        [SerializeField, Min(0.01f)] private float powerLineDuration = 0.12f;
        [SerializeField, Min(0.01f)] private float powerExpandDuration = 0.32f;
        [SerializeField, Min(0.01f)] private float powerSettleDuration = 0.12f;
        [SerializeField, Min(0.01f)] private float closeFadeDuration = 0.25f;
        [SerializeField, Min(0f)] private float noiseJitter = 3f;

        private Coroutine transitionCoroutine;
        private Coroutine coolTimeFadeCoroutine;
        private Vector2 initialAnchoredPosition;
        private Vector3 initialScale;

        private void Awake()
        {
            if (effectRoot == null)
            {
                effectRoot = canvasGroup.transform as RectTransform;
            }

            if (effectRoot != null)
            {
                initialAnchoredPosition = effectRoot.anchoredPosition;
                initialScale = effectRoot.localScale;
            }

            InitializeCoolTimeUI();
            CloseImmediately();
        }

        private void Update()
        {
            if (canvasGroup.alpha == 0)
                return;

            scannerTrans.position = playerInput.MousePosition;
        }

        public void Open()
        {
            StopTransition();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            transitionCoroutine = StartCoroutine(PowerOnRoutine());
        }

        public void Close()
        {
            StopTransition();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            HideItemInfo();
            transitionCoroutine = StartCoroutine(FadeOutRoutine());
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

        #region Slider

        public void SetCoolTime(float value)
        {
            coolTimeSlider.value = Mathf.Clamp01(value);
        }

        public void SetCoolTimeCharging()
        {
            if (scanTxt != null)
            {
                scanTxt.text = "스캔 충전중...";
            }
        }

        public void SetCoolTimeReady()
        {
            SetCoolTime(1f);

            if (scanTxt != null)
            {
                scanTxt.text = "스캔 준비완료!";
            }
        }

        public void FadeInCoolTime()
        {
            StartCoolTimeFade(1f);
        }

        public void FadeOutCoolTime()
        {
            StartCoolTimeFade(0f);
        }

        #endregion

        private IEnumerator PowerOnRoutine()
        {
            if (effectRoot == null)
            {
                canvasGroup.alpha = 1f;
                transitionCoroutine = null;
                yield break;
            }

            effectRoot.anchoredPosition = initialAnchoredPosition;
            effectRoot.localScale = new Vector3(
                initialScale.x * 1.08f,
                initialScale.y * 0.015f,
                initialScale.z);

            float elapsedTime = 0f;

            while (elapsedTime < powerLineDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Random.Range(0.35f, 1f);
                ApplyNoiseJitter(1f);
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < powerExpandDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / powerExpandDuration);
                float easedTime = 1f - Mathf.Pow(1f - normalizedTime, 3f);

                effectRoot.localScale = new Vector3(
                    Mathf.Lerp(initialScale.x * 1.06f, initialScale.x, easedTime),
                    Mathf.Lerp(initialScale.y * 0.015f, initialScale.y * 1.04f, easedTime),
                    initialScale.z);
                canvasGroup.alpha =
                    Mathf.Clamp01(easedTime + Random.Range(-0.18f, 0.12f));
                ApplyNoiseJitter(1f - normalizedTime);
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < powerSettleDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / powerSettleDuration);

                effectRoot.localScale = new Vector3(
                    initialScale.x,
                    Mathf.Lerp(initialScale.y * 1.04f, initialScale.y, normalizedTime),
                    initialScale.z);
                canvasGroup.alpha = Mathf.Lerp(0.85f, 1f, normalizedTime);
                ApplyNoiseJitter((1f - normalizedTime) * 0.25f);
                yield return null;
            }

            ResetEffectRoot();
            canvasGroup.alpha = 1f;
            transitionCoroutine = null;
        }

        private IEnumerator FadeOutRoutine()
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            ResetEffectRoot();

            while (elapsedTime < closeFadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / closeFadeDuration);
                canvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    0f,
                    Mathf.SmoothStep(0f, 1f, normalizedTime));
                yield return null;
            }

            canvasGroup.alpha = 0f;
            transitionCoroutine = null;
        }

        private void ApplyNoiseJitter(float strength)
        {
            effectRoot.anchoredPosition = initialAnchoredPosition +
                Random.insideUnitCircle * (noiseJitter * strength);
        }

        private void CloseImmediately()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            ResetEffectRoot();
            HideItemInfo();
        }

        private void ResetEffectRoot()
        {
            if (effectRoot == null)
            {
                return;
            }

            effectRoot.anchoredPosition = initialAnchoredPosition;
            effectRoot.localScale = initialScale;
        }

        private void StopTransition()
        {
            if (transitionCoroutine == null)
            {
                return;
            }

            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        private void InitializeCoolTimeUI()
        {
            if (coolTimeSlider == null)
            {
                return;
            }

            coolTimeSlider.gameObject.SetActive(true);

            if (coolTimeCanvasGroup == null)
            {
                coolTimeCanvasGroup =
                    coolTimeSlider.GetComponent<CanvasGroup>();
            }

            if (scanTxt == null)
            {
                scanTxt =
                    coolTimeSlider.GetComponentInChildren<TMP_Text>(true);
            }

            if (coolTimeCanvasGroup == null)
            {
                Debug.LogError(
                    "ScannerUI: ScannerCoolTimeUI에 CanvasGroup이 없습니다.",
                    this);
                return;
            }

            coolTimeCanvasGroup.alpha = 1f;
            coolTimeCanvasGroup.interactable = false;
            coolTimeCanvasGroup.blocksRaycasts = false;
            SetCoolTimeReady();
        }

        private void StartCoolTimeFade(float targetAlpha)
        {
            if (coolTimeCanvasGroup == null)
            {
                return;
            }

            if (coolTimeFadeCoroutine != null)
            {
                StopCoroutine(coolTimeFadeCoroutine);
            }

            coolTimeFadeCoroutine =
                StartCoroutine(CoolTimeFadeRoutine(targetAlpha));
        }

        private IEnumerator CoolTimeFadeRoutine(float targetAlpha)
        {
            float startAlpha = coolTimeCanvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < coolTimeFadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / coolTimeFadeDuration);
                coolTimeCanvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    Mathf.SmoothStep(0f, 1f, normalizedTime));
                yield return null;
            }

            coolTimeCanvasGroup.alpha = targetAlpha;
            coolTimeFadeCoroutine = null;
        }
    }
}
