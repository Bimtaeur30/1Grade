using UnityEngine;
using GameLib.EventChannelSystem;
using _MemberWorkspace.JJW.Asset._02_Script.Events;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public enum GameState { Exploring, Storm, Shopping }//Exploring이게 인게임 플레이중일때

    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }
        public GameState CurrentState { get; private set; }

        [SerializeField] private float exploreTimeLimit = 30f;
        [SerializeField] private EventChannelSO eventChannel;

        private float _timer;

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
                if (_timer <= 0f) EndTurn();
            }
        }

        public void EndTurn() //타이머 안끝나도 상점갈수 있으니까 그냥 퍼블릭으로 둠 ㅇㅇ
        {
            CurrentState = GameState.Shopping;
            eventChannel.RaiseEvent(new TurnEndEvent());
        }

        public void StartStorm()
        {
            CurrentState = GameState.Storm;
            eventChannel.RaiseEvent(new StormStartEvent());
        }

        public void EndStorm()
        {
            CurrentState = GameState.Exploring;
            eventChannel.RaiseEvent(new StormEndEvent());
            GameData.Instance.CurrentTurnLevel++;
            StartTurn();
        }

        private void StartTurn()
        {
            CurrentState = GameState.Exploring;
            _timer = exploreTimeLimit;
            eventChannel.RaiseEvent(new TurnStartEvent());
        }
    }
}