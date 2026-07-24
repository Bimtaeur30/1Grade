using System;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using DG.Tweening;
using GameLib.EventChannelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UpgradeCardSO upgradeCardSO;
        [SerializeField] private MoneyChecker moneyChecker;
        [SerializeField] private FailBuyUX failBuyUX;
        [SerializeField] private EventChannelSO eventChannelSO;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Ease ease;

        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image upgradeIcon;
        [SerializeField] private GameObject shopUI;

        [Header("등장 연출")]
        [SerializeField] private float entranceStartScale = 0.7f;

        [Header("호버 연출")]
        [SerializeField] private float hoverScale = 1.07f;
        [SerializeField] private float hoverDuration = 0.15f;

        [Header("아이들 플로팅")]
        [SerializeField] private float idleFloatDistance = 12f;
        [SerializeField] private float idleFloatDuration = 1.5f;

        [Header("구매 펀치")]
        [SerializeField] private float punchScale = 0.2f;
        [SerializeField] private float punchDuration = 0.25f;

        [Header("퇴장 연출")]
        [SerializeField] private float exitPopScale = 1.15f;
        [SerializeField] private float exitPopDuration = 0.12f;
        [SerializeField] private float unselectedFadeScale = 0.85f;

        private const int MaxLevel = 5;

        private RectTransform _rectTransform;
        private Vector2 _originalAnchoredPosition;
        private bool _positionCached;

        
        private Tween _idleTween;
        private Tween _hoverTween;
        private Sequence _entranceSequence;
        private Sequence _exitSequence;

        private bool _isHoverEnabled;

        public event Action<UpgradeCardUI> OnBought;

        private void Awake()
        {
            CacheOriginalPosition();
        }

        private void OnDisable()
        {
            KillCardTweens(); 
        }

        private void CacheOriginalPosition()
        {
            if (_positionCached) return;

            _positionCached = true;
            _rectTransform = (RectTransform)transform;
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            Refresh();
        }

        private int GetCurrentLevel()
        {
            return upgradeCardSO.Type switch
            {
                UpgradeType.MaxWeight => GameData.Instance.CurrentMaxWeightLevel,
                UpgradeType.RunSpeed => GameData.Instance.CurrentRunSpeedLevel,
                UpgradeType.ScanCooldown => GameData.Instance.CurrentScanCooldownLevel,
                _ => 0
            };
        }

        private void IncreaseCurrentLevel()
        {
            switch (upgradeCardSO.Type)
            {
                case UpgradeType.MaxWeight:
                    GameData.Instance.CurrentMaxWeightLevel++;
                    break;
                case UpgradeType.RunSpeed:
                    GameData.Instance.CurrentRunSpeedLevel++;
                    break;
                case UpgradeType.ScanCooldown:
                    GameData.Instance.CurrentScanCooldownLevel++;
                    break;
            }
        }

        private void RaiseUpgradeEvent(int amount)
        {
            switch (upgradeCardSO.Type)
            {
                case UpgradeType.MaxWeight:
                    eventChannelSO.RaiseEvent(PlayerEvents.MaxWeightUpgradeEvent.Init(amount));
                    break;
                case UpgradeType.RunSpeed:
                    eventChannelSO.RaiseEvent(PlayerEvents.RunSpeedUpgradeEvent.Init(amount));
                    break;
                case UpgradeType.ScanCooldown:
                    eventChannelSO.RaiseEvent(PlayerEvents.ScanCooltimeUpgradeEvent.Init(amount));
                    break;
            }
        }

        private void Refresh()
        {
            int currentLevel = GetCurrentLevel();

            if (currentLevel >= MaxLevel)
            {
                descriptionText.text = "더이상 강화할수 없습니다.";
                priceText.text = "";
                levelText.text = "MAX";
                return;
            }

            descriptionText.text = upgradeCardSO.LevelDatas[currentLevel-1].description;//2렙부터니까
            priceText.text = upgradeCardSO.LevelDatas[currentLevel-1].cost.ToString();//2렙부터니까
            levelText.text = (currentLevel+1).ToString();
        }

        public void Buy()
        {
            int currentLevel = GetCurrentLevel();
            if (currentLevel >= MaxLevel) return;

            if (!moneyChecker.CheckMoney(upgradeCardSO.LevelDatas[currentLevel-1].cost))//2렙부터니까
            {
                return;
            }

            int amount = upgradeCardSO.LevelDatas[currentLevel-1].amount;//2렙부터니까

            IncreaseCurrentLevel();
            RaiseUpgradeEvent(amount);

            OnBought?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isHoverEnabled) return;

            AnimateHoverScale(hoverScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isHoverEnabled) return;

            AnimateHoverScale(1f);
        }

        private void AnimateHoverScale(float targetScale)
        {
            _hoverTween?.Kill();
            _hoverTween = _rectTransform.DOScale(targetScale, hoverDuration).SetEase(Ease.OutQuad);
        }

        public void PlayEntrance(float distance, float duration)
        {
            CacheOriginalPosition();
            KillCardTweens();

            _isHoverEnabled = false;

            _rectTransform.localScale = Vector3.one * entranceStartScale;
            canvasGroup.alpha = 0;

            Vector2 targetPos = _originalAnchoredPosition + new Vector2(0, distance);

            _entranceSequence = DOTween.Sequence();
            _entranceSequence.Join(_rectTransform.DOAnchorPos(targetPos, duration).SetEase(ease));
            _entranceSequence.Join(_rectTransform.DOScale(1f, duration).SetEase(Ease.OutBack));
            _entranceSequence.Join(canvasGroup.DOFade(1, duration));
            _entranceSequence.OnComplete(() =>
            {
                _entranceSequence = null;
                _isHoverEnabled = true;
                StartIdleFloating(targetPos); 
            });
        }

        private void StartIdleFloating(Vector2 basePosition)
        {
            _idleTween?.Kill();

            _idleTween = _rectTransform
                .DOAnchorPosY(basePosition.y + idleFloatDistance, idleFloatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(UnityEngine.Random.Range(0f, idleFloatDuration));
        }

        public void PlayUnselectedFadeOut(float duration)
        {
            CacheOriginalPosition();
            KillCardTweens();

            _isHoverEnabled = false;

            canvasGroup.DOFade(0, duration);
            _rectTransform.DOScale(unselectedFadeScale, duration).SetEase(Ease.InQuad);
        }

        public void PlaySelectedDrop(float dropDistance, float dropDuration)
        {
            CacheOriginalPosition();
            KillCardTweens();

            _isHoverEnabled = false;

            _exitSequence = DOTween.Sequence();
            _exitSequence.Append(_rectTransform.DOPunchScale(Vector3.one * punchScale, punchDuration, 6, 0.8f));
            _exitSequence.Append(_rectTransform.DOScale(exitPopScale, exitPopDuration).SetEase(Ease.OutQuad));
            _exitSequence.Append(_rectTransform.DOScale(0f, dropDuration).SetEase(Ease.InBack));
            _exitSequence.Join(_rectTransform
                .DOAnchorPosY(_originalAnchoredPosition.y - dropDistance, dropDuration)
                .SetEase(Ease.InQuad));
            _exitSequence.Join(canvasGroup.DOFade(0, dropDuration));
            _exitSequence.OnComplete(() =>
            {
                _exitSequence = null;
                shopUI.SetActive(false);
                GameFlowManager.Instance.CloseShop();//스톰 연출로 넘어가기
            });
        }

        public void ResetVisual()
        {
            CacheOriginalPosition();
            KillCardTweens();

            _isHoverEnabled = false;
            canvasGroup.alpha = 1;
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = Vector3.one;
        }

        private void KillCardTweens()
        {
            _idleTween?.Kill();
            _hoverTween?.Kill();
            _entranceSequence?.Kill();
            _exitSequence?.Kill();

            _idleTween = null;
            _hoverTween = null;
            _entranceSequence = null;
            _exitSequence = null;

            _rectTransform.DOKill();
            canvasGroup.DOKill();
        }
    }
}