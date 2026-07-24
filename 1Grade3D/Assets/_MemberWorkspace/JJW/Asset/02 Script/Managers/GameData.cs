using System;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class GameData : MonoBehaviour
    {
        [SerializeField] private int startMoneyAmount = 50;
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

        public int CurrentTurnLevel { get; set; } = 1; //현재 턴 레벨은 1로 시작할거
        public int CurrentMoney {
            get=> startMoneyAmount;
            set
            {
                if (value == startMoneyAmount) return;
                OnMoneyValueChanged?.Invoke(startMoneyAmount,value);
                startMoneyAmount = value;
            }
        }

        public event Action<int,int> OnMoneyValueChanged;//before, after
        
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