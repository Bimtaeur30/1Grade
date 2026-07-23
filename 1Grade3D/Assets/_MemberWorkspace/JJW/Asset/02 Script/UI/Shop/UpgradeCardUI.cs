using System;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using DG.Tweening;
using GameLib.EventChannelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class UpgradeCardUI : MonoBehaviour
    {
        [SerializeField] private UpgradeCardSO upgradeCardSO;
        [SerializeField] private MoneyChecker moneyChecker;
        [SerializeField] private EventChannelSO eventChannelSO;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Ease ease;

        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image upgradeIcon;
        [SerializeField] private GameObject shopUI;

        private const int MaxLevel = 5;

        private RectTransform _rectTransform;
        private Vector2 _originalAnchoredPosition;
        private bool _positionCached;

        public event Action<UpgradeCardUI> OnBought;

        private void Awake()
        {
            CacheOriginalPosition();
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
                priceText.text = "-";
                levelText.text = "MAX";
                return;
            }

            descriptionText.text = upgradeCardSO.LevelDatas[currentLevel-1].description;//2렙부터니까
            priceText.text = upgradeCardSO.LevelDatas[currentLevel-1].cost.ToString();//2렙부터니까
            levelText.text = currentLevel.ToString();
        }

        public void Buy()
        {
            int currentLevel = GetCurrentLevel();
            if (currentLevel >= MaxLevel) return;

            if (!moneyChecker.CheckMoney(upgradeCardSO.LevelDatas[currentLevel-1].cost))//2렙부터니까
                return;

            int amount = upgradeCardSO.LevelDatas[currentLevel-1].amount;//2렙부터니까

            IncreaseCurrentLevel();
            RaiseUpgradeEvent(amount);

            OnBought?.Invoke(this);
        }

        public void PlayEntrance(float distance, float duration)
        {
            CacheOriginalPosition();

            _rectTransform.anchoredPosition = _originalAnchoredPosition - new Vector2(0, distance);
            canvasGroup.alpha = 0;

            _rectTransform.DOAnchorPos(_originalAnchoredPosition, duration).SetEase(ease);
            canvasGroup.DOFade(1, duration);
        }

        public void PlayUnselectedFadeOut(float duration)
        {
            canvasGroup.DOFade(0, duration);
        }

        public void PlaySelectedDrop(float dropDistance, float dropDuration)
        {
            CacheOriginalPosition();

            _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y - dropDistance, dropDuration)
                .OnComplete(() => shopUI.SetActive(false));
        }

        public void ResetVisual()
        {
            CacheOriginalPosition();

            canvasGroup.DOKill();
            _rectTransform.DOKill();
            canvasGroup.alpha = 1;
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
    }
}