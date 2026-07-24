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

        [SerializeField] private float exploreTimeLimit = 30f;//임시
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
        private void Start()
        {
            StartTurn();
        }

        private void Update()
        {
            if (CurrentState == GameState.Exploring)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f) EndTurn();
            }
        }

        public void EndTurn() //타이머 안끝나도 상점갈수 있으니까 그냥 퍼블릭으로 둠 ㅇㅇ 상점 갈때 호출
        {
            CurrentState = GameState.Shopping;
            eventChannel.RaiseEvent(TurnEvents.TurnEndEvent);
        }

        public void CloseShop()//상점에서 아이템 고르면 실행
        {
            eventChannel.RaiseEvent(FlowEvents.ShopCloseEvent);
            StartStorm();
        }

        public void OpenShop()//정산 완료 버트 누를때
        {
            Debug.Log("Open Shop");
            eventChannel.RaiseEvent(FlowEvents.ShopOpenEvent);
        }

        private void StartStorm()
        {
            CurrentState = GameState.Storm;
            eventChannel.RaiseEvent(FlowEvents.StormStartEvent);
        }
        
        
        

        public void EndStorm()//지한이가 호출(폭풍연출 끝나면)
        {
            CurrentState = GameState.Exploring;
            eventChannel.RaiseEvent(FlowEvents.StormEndEvent);
            GameData.Instance.CurrentTurnLevel++;
            StartTurn();
        }

        private void StartTurn()
        {
            CurrentState = GameState.Exploring;
            _timer = exploreTimeLimit;
            eventChannel.RaiseEvent(TurnEvents.TurnStartEvent);
        }
    }
}