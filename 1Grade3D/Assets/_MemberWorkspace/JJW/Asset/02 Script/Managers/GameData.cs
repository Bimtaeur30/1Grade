using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class GameData : MonoBehaviour
    {
        private static GameData _instance;

        //제일먼저 돌아야함
        public static GameData Instance
        {
            get
            {
                if (_instance == null) _instance = FindFirstObjectByType<GameData>();
                return _instance;
            }
        }

        public int CurrentLevel { get; set; } = 1;
        public int CurrentMoney { get; set; } = 1000;
        
        public int CurrentMaxWeightLevel { get; set; } = 1;
        public int CurrentRunSpeedLevel { get; set; } = 1;
        public int CurrentScanCooldownLevel { get; set; } = 1;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject); return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}