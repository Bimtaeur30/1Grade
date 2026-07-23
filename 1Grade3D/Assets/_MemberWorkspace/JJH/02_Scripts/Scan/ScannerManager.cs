using _MemberWorkspace.JJH._02_Scripts.Map;
using GameLib.EventChannelSystem;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private EventChannelSO playerChannel;
        [SerializeField] private ScannerUI scannerUI;
        [SerializeField] private PlayerStat playerStat;

        [SerializeField] private float scanDuration = 5f;
        [SerializeField] private float chargeDuration = 0.15f;
        [SerializeField] private float scanCoolTime = 5f;

        private bool _isScanning;
        private bool _isCharging;

        private float _remainTime;
        private float _chargeTime;
        private float _nextScanTime;

        private GroundItem _currentItem;
        private SpriteScanEffect _currentScanEffect;

        private void Awake()
        {
            playerInput.OnScannerKeyPressed += UseScanner;
            playerInput.OnScannerKeyReleased += StopScanner;
            playerStat.ScanCooltimeChanged += ScanCoolTimeChange;
        }

        private void OnDestroy()
        {
            playerInput.OnScannerKeyPressed -= UseScanner;
            playerInput.OnScannerKeyReleased -= StopScanner;
            playerStat.ScanCooltimeChanged -= ScanCoolTimeChange;
        }

        private void Update()
        {
            if (!_isScanning)
            {
                UpdateCoolTime();
                return;
            }

            if (_isCharging)
                UpdateCharge();
            else
            {
                UpdateTimer();

                if (!_isScanning)
                    return;
            }

            UpdateHover();
        }

        private void ScanCoolTimeChange(int coolTime)
        {
            scanCoolTime = coolTime;
        }

        private void UseScanner()
        {
            if (_isScanning)
                return;

            if (Time.unscaledTime < _nextScanTime)
                return;

            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(true));

            _isScanning = true;
            _isCharging = true;
            _chargeTime = 0f;
            _remainTime = scanDuration;

            Time.timeScale = 0.5f;

            scannerUI.Open();
            scannerUI.SetGauge(0);
            scannerUI.HideCoolTime();
        }

        private void StopScanner()
        {
            if (!_isScanning)
                return;

            _nextScanTime = Time.unscaledTime + scanCoolTime;

            scannerUI.SetCoolTime(0f);
            scannerUI.ShowCoolTime();

            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(false));

            _isScanning = false;
            _isCharging = false;

            ClearCurrentHover();

            Time.timeScale = 1f;

            scannerUI.HideItemInfo();
            scannerUI.Close();
        }

        private void UpdateCharge()
        {
            _chargeTime += Time.unscaledDeltaTime;
            scannerUI.SetGauge(_chargeTime / chargeDuration);

            if (_chargeTime >= chargeDuration)
            {
                _isCharging = false;
                scannerUI.SetGauge(1);
            }
        }

        private void UpdateTimer()
        {
            _remainTime -= Time.unscaledDeltaTime;

            scannerUI.SetGauge(_remainTime / scanDuration);

            if (_remainTime <= 0)
                StopScanner();
        }

        private void UpdateHover()
        {
            GroundItem item = playerInput.GetGroundItem();

            if (item == null)
            {
                if (_currentItem != null)
                    ClearCurrentHover();

                return;
            }

            if (_currentItem == item)
                return;

            if (_currentItem != null)
                ClearCurrentHover();

            _currentItem = item;
            item.SetOutline(10f);
            scannerUI.HideItemInfo();

            _currentScanEffect =
                item.GetComponentInChildren<SpriteScanEffect>(true);

            if (_currentScanEffect == null)
            {
                scannerUI.SetItemInfo(item);
                return;
            }

            GroundItem scanningItem = item;
            _currentScanEffect.PlayScan(() =>
            {
                if (_isScanning && _currentItem == scanningItem)
                    scannerUI.SetItemInfo(scanningItem);
            });
        }

        private void UpdateCoolTime()
        {
            if (Time.unscaledTime >= _nextScanTime)
            {
                scannerUI.HideCoolTime();
                return;
            }

            scannerUI.ShowCoolTime();

            float remain = _nextScanTime - Time.unscaledTime;
            float progress = 1f - (remain / scanCoolTime);

            scannerUI.SetCoolTime(progress);
        }

        private void ClearCurrentHover()
        {
            if (_currentItem != null)
                _currentItem.SetOutline(0f);

            if (_currentScanEffect != null)
                _currentScanEffect.ShowOriginal();

            _currentScanEffect = null;
            _currentItem = null;
            scannerUI.HideItemInfo();
        }
    }
}
