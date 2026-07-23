using System;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script
{
    public enum GameState { Exploring, Storm, Shopping}

    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }
        public GameState CurrentState { get; private set; }
        
        [SerializeField] private float exploreTimeLimit = 30f;
        private float _timer;

        public event Action OnExploreStart;
        public event Action OnExploreEnd;
        
        public event Action OnStormStart;
        public event Action OnStormEnd;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (CurrentState == GameState.Exploring)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f) EndExploring();
            }
        }

        public void EndExploring()//타이머 안끝나도 상점갈수 있으니까 그냥 퍼블릭으로 둠 ㅇㅇ
        {
            CurrentState = GameState.Shopping;
            OnExploreEnd?.Invoke();
        }

        public void StartStorm()
        {
            CurrentState = GameState.Storm;
            OnStormStart?.Invoke();
        }

        public void EndStorm()
        {
            CurrentState = GameState.Exploring;
            OnStormEnd?.Invoke();
            GameData.Instance.CurrentLevel++;
            StartExploring();
        }

        private void StartExploring()
        {
            CurrentState = GameState.Exploring;
            _timer = exploreTimeLimit;
            OnExploreStart?.Invoke();
        }
    }
}