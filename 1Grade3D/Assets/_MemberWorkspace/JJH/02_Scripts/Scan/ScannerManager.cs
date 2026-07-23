using _MemberWorkspace.JJH._02_Scripts.Map;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private EventChannelSO playerChannel;
        [SerializeField] private ScannerUI scannerUI;

        [SerializeField] private float scanDuration = 5f;

        private bool _isScanning;
        private float _remainTime;
        private GroundTile _currentTile;

        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                UseScanner();

            if (!_isScanning)
                return;

            UpdateTimer();
            UpdateHover();
        }

        public void UseScanner()
        {
            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(true));

            _isScanning = true;
            _remainTime = scanDuration;

            scannerUI.Open();
            scannerUI.SetGauge(1);
        }

        private void StopScanner()
        {
            playerChannel.RaiseEvent(PlayerEvents.ScannerEvent.Init(false));

            _isScanning = false;
            _currentTile = null;

            scannerUI.HideItemInfo();
            scannerUI.Close();
        }

        private void UpdateTimer()
        {
            _remainTime -= Time.deltaTime;

            scannerUI.SetGauge(_remainTime / scanDuration);

            if (_remainTime <= 0)
                StopScanner();
        }

        private void UpdateHover()
        {
            GroundTile tile = playerInput.GetGroundTile();

            if (tile == null || !tile.HasItem)
            {
                if (_currentTile != null)
                {
                    _currentTile = null;
                    scannerUI.HideItemInfo();
                }

                return;
            }

            if (_currentTile == tile)
                return;

            _currentTile = tile;

            scannerUI.SetItemInfo(tile.Item);
        }
    }
}