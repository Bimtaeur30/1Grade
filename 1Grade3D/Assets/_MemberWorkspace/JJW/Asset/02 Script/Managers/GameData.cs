using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class GameData : MonoBehaviour
    {
        private static GameData _instance;

        //다른 오브젝트의 OnEnable이 이 스크립트의 Awake보다 먼저 돌 수 있다.
        //그때도 null이 아니도록 아직 대입 전이면 직접 찾아온다.
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