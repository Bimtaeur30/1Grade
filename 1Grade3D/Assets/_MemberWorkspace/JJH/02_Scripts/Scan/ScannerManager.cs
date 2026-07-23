using _MemberWorkspace.JJH._02_Scripts.Map;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScannerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private ScannerUI scannerUI;

        [SerializeField] private float scanDuration = 10f;

        public bool IsScanning { get; private set; }

        private float _remainTime;
        private GroundTile _currentTile;

        private void Update()
        {
            if (!IsScanning)
                return;

            UpdateTimer();
            UpdateHover();
        }

        public void UseScanner()
        {
            IsScanning = true;
            _remainTime = scanDuration;

            scannerUI.Open();
            scannerUI.SetGauge(1);
        }

        private void StopScanner()
        {
            IsScanning = false;
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

            scannerUI.ShowItem(tile.Item);
        }
    }
}