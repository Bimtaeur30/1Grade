using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; }

        public int CurrentLevel { get; set; } = 1;
        public int CurrentMoney { get; set; } = 0;
        
        public int CurrentMaxWeightLevel { get; set; } = 1;
        public int CurrentRunSpeedLevel { get; set; } = 1;
        public int CurrentScanCooldownLevel { get; set; } = 1;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}