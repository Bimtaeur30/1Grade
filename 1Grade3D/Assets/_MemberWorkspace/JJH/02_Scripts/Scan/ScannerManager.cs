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
        [SerializeField] private float chargeDuration = 0.3f;

        private bool _isScanning;
        private bool _isCharging;
        private float _remainTime;
        private float _chargeTime;

        private GroundItem _currentItem;

        private void Awake()
        {
            playerInput.OnScannerKeyPressed += UseScanner;
            //playerStat.ScanCooltimeChanged += ScanDurationChange;
        }

        private void OnDestroy()
        {
            playerInput.OnScannerKeyPressed -= UseScanner;
            //playerStat.ScanCooltimeChanged += ScanDurationChange;
        }

        private void Update()
        {
            if (!_isScanning)
                return;

            if (_isCharging)
            {
                UpdateCharge();
            }
            else
            {
                UpdateTimer();

                if (!_isScanning)
                    return;
            }

            UpdateHover();
        }

        private void ScanDurationChange(int duration)
        {
            scanDuration = duration;
        }

        private void UseScanner()
        {
            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(true));

            _isScanning = true;
            _isCharging = true;
            _chargeTime = 0f;
            _remainTime = scanDuration;

            Time.timeScale = 0.5f;

            scannerUI.Open();
            scannerUI.SetGauge(0);
        }

        private void StopScanner()
        {
            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(false));

            _isScanning = false;
            _isCharging = false;
            if (_currentItem != null)
                _currentItem.SetOutline(0f);
            _currentItem = null;

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
                {
                    _currentItem.SetOutline(0f);
                    _currentItem = null;
                    scannerUI.HideItemInfo();
                }
                return;
            }

            if (_currentItem == item)
                return;

            if (_currentItem != null)
                _currentItem.SetOutline(0f);

            _currentItem = item;
            item.SetOutline(10f);
            scannerUI.SetItemInfo(item);
        }
    }
}